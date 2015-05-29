using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// Lock-Free data queue used as a pool mechanism for NetworkMessage enties.
    /// source: http://www.boyet.com/articles/LockfreeQueue.html
    /// </summary>
    public class LockFreeQueue
    {
        /// <summary>
        /// Lock-Free instance.
        /// </summary>
        private static LockFreeQueue queue;

        /// <summary>
        /// Manages LockFreeQueue throught singleton pattern.
        /// </summary>
        public static LockFreeQueue GetInstance()
        {
            if (queue == null)
                queue = new LockFreeQueue();
            return queue;
        }

        /// <summary>
        /// NetworkMessage object reference to the head.
        /// </summary>
        SingleLinkNode<NetworkMessage> head;

        /// <summary>
        /// NetworkMessage object reference to the tail.
        /// </summary>
        SingleLinkNode<NetworkMessage> tail;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LockFreeQueue()
        {
            head = new SingleLinkNode<NetworkMessage>();
            tail = head;
        }

        /// <summary>
        /// Enqueues a NetworkMessage in the LockFreeQueue.
        /// </summary>
        public void Enqueue(NetworkMessage item)
        {
            SingleLinkNode<NetworkMessage> oldTail = null;
            SingleLinkNode<NetworkMessage> oldTailNext;

            SingleLinkNode<NetworkMessage> newNode = new SingleLinkNode<NetworkMessage>();
            newNode.Item = item;

            bool newNodeWasAdded = false;
            while (!newNodeWasAdded)
            {
                oldTail = tail;
                oldTailNext = oldTail.Next;

                if (tail == oldTail)
                {
                    if (oldTailNext == null)
                        newNodeWasAdded = SyncMethods.CAS<SingleLinkNode<NetworkMessage>>(ref tail.Next, null, newNode);
                    else
                        SyncMethods.CAS<SingleLinkNode<NetworkMessage>>(ref tail, oldTail, oldTailNext);
                }
            }

            SyncMethods.CAS<SingleLinkNode<NetworkMessage>>(ref tail, oldTail, newNode);
        }

        /// <summary>
        /// Dequeues a NetworkMessage in the LockFreeQueue.
        /// </summary>
        public bool Dequeue(out NetworkMessage item)
        {
            item = default(NetworkMessage);
            SingleLinkNode<NetworkMessage> oldHead = null;

            bool haveAdvancedHead = false;
            while (!haveAdvancedHead)
            {

                oldHead = head;
                SingleLinkNode<NetworkMessage> oldTail = tail;
                SingleLinkNode<NetworkMessage> oldHeadNext = oldHead.Next;

                if (oldHead == head)
                {
                    if (oldHead == oldTail)
                    {
                        if (oldHeadNext == null)
                        {
                            item = (NetworkMessage)Activator.CreateInstance(typeof(NetworkMessage));
                            return true;
                        }
                        SyncMethods.CAS<SingleLinkNode<NetworkMessage>>(ref tail, oldTail, oldHeadNext);
                    }

                    else
                    {
                        item = oldHeadNext.Item;
                        haveAdvancedHead =
                          SyncMethods.CAS<SingleLinkNode<NetworkMessage>>(ref head, oldHead, oldHeadNext);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Dequeues a NetworkMessage in the LockFreeQueue.
        /// </summary>
        public NetworkMessage Dequeue()
        {
            NetworkMessage result;
            Dequeue(out result);
            return result;
        }
    }

    internal class SingleLinkNode<NetworkMessage>
    {
        // Note: the Next member cannot be a property since it participates in many CAS operations
        public SingleLinkNode<NetworkMessage> Next;
        public NetworkMessage Item;
    }
}
