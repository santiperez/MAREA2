#if SYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using log4net;
using System.Collections.Concurrent;

namespace Marea
{
    /// <summary>
    /// Connection used in TCPTransport.
    /// </summary>
    public class Connection:IConnection
    {
        /// <summary>
        /// Logger for the Connection.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Marea.Transport.Connection");

        /// <summary>
        /// Socket for the incoming data.
        /// </summary>
        protected Socket socket;

        /// <summary>
        /// Local port.
        /// </summary>
        protected int localPort;

        /// <summary>
        /// Thread used to manage the current context execution.
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// byte[3] which contains MAREA magic number: "MR1". This array is contained in MAREA header.
        /// </summary>
        protected static byte[] magicNumber = new byte[] { (byte)'M', (byte)'R', (byte)'1' };

        /// <summary>
        /// byte[4] which contains the size of the incoming data. This array is contained in MAREA header.
        /// </summary>
        protected byte[] mareaHeader = new byte[magicNumber.Length + sizeof(Int32)];

        /// <summary>
        /// Thread used to send data if an exception occurs.
        /// </summary>
        protected Thread thTry;

        /// <summary>
        /// ConcurrentQueue<NetworkMessage> used to store NetworkMessage entities in case an exception occurs.
        /// </summary>
        protected ConcurrentQueue<NetworkMessage> outBuffer;

        /// <summary>
        /// bool to check if the Connection is running.
        /// </summary>
        private volatile bool end;

        /// <summary>
        /// CloseConnection delegate use to close the connection.
        /// </summary>
        protected CloseConnection closeConnectionHandler;

        /// <summary>
        /// Network instance.
        /// </summary>
        private Network network;

        /// <summary>
        /// LockFreeQueue instance.
        /// </summary>
        private LockFreeQueue queue;

        /// <summary>
        /// output Lane used to store the network bindings between the network sublayers (coder and transports).
        /// </summary>
        protected Lane hint = null;

        /// <summary>
        /// Constructor from incoming Connections.
        /// </summary>
        public Connection(Network network, Socket socket, CloseConnection closeConnectionHandler)
        {
            this.socket = socket;
            this.closeConnectionHandler = closeConnectionHandler;
            this.localPort = ((IPEndPoint)socket.LocalEndPoint).Port;
            this.end = false;
            this.network = network;
            this.queue = LockFreeQueue.GetInstance();
        }
        
        
        /// <summary>
        /// Constructor from outgoing Connections.
        /// </summary>
        public Connection(Socket socket)
        {
            this.socket = socket;
            Buffer.BlockCopy(magicNumber, 0, mareaHeader, 0, magicNumber.Length);
            outBuffer = new ConcurrentQueue<NetworkMessage>();
        }

        /// <summary>
        /// Receives data through the Connection.
        /// </summary>
        public void Run()
        {
            this.thread = Thread.CurrentThread;
            IPEndPoint ip;
            int received=0;

            ip = (IPEndPoint)socket.RemoteEndPoint;

            //pool
            NetworkMessage networkMessage=queue.Dequeue();
            while (!end)
            {
                
                try
                {
                    socket.Receive(mareaHeader, mareaHeader.Length, SocketFlags.None);

                    if (mareaHeader[0] == magicNumber[0] && mareaHeader[1] == magicNumber[1] && mareaHeader[2] == magicNumber[2])
                    {
                        networkMessage.Offset= (int)(
                                       (mareaHeader[magicNumber.Length] & 0x000000FF) |
                                       (mareaHeader[magicNumber.Length+1] << 8 & 0x0000FF00) |
                                       (mareaHeader[magicNumber.Length + 2] << 16 & 0x00FF0000) |
                                       (mareaHeader[magicNumber.Length+3] << 24)
                                       );
                        received = 0;

                        while (received !=networkMessage.Offset)
                        {
                            received += socket.Receive(networkMessage.Buffer, received, networkMessage.Offset - received, SocketFlags.None);
                        }
                        
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("TCP Packet received from " + ip.Address);
                            log.Debug("Bytes received : " + received);
                        }
                        networkMessage.StatusCode = NetWorkStatusCode.OK;
                        hint = network.Receive(networkMessage, hint);
                    }
                }
                catch (ThreadAbortException)
                {
                    //It's safe to ignore this :)
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == 10004)
                    {
                        //It's safe to ignore this :)
                    }
                    else if (e.ErrorCode == 10054 || e.ErrorCode == 10053) //The remote host closes this connection
                    {
                        closeConnectionHandler(this);

                        //TODO The state is updated but not passed to the upper layers
                        networkMessage.StatusCode = NetWorkStatusCode.TransportReceivingError;
                    }
                    else
                    {
                        end = true;
                        //TODO The state is updated but not passed to the upper layers
                        networkMessage.StatusCode = NetWorkStatusCode.TransportReceivingError;
                    }
                }
                //catch (Exception ex)
                //{
                //    log.Error("Exception while receiving in Connection", ex);
                //    throw new TransportException("Exception while receiving in Connection", ex);
                //}
            }
            //Add this object back into the pool.
            queue.Enqueue(networkMessage);
        }

        /// <summary>
        /// Closes the Connection.
        /// </summary>
        public void Close()
        {
            end = true;
            socket.Close();
            if(thread!=null)
                thread.Abort();
            if(thTry!=null)
                thTry.Abort();
        }

        /// <summary>
        /// Receives data through the Connection.
        /// </summary>
        public void Send(NetworkMessage networkMessage)
        {
            IpTransportAddress ta = (IpTransportAddress)networkMessage.TransportAddress;
            int write = 0;

            if (log.IsDebugEnabled)
            {
                log.Debug("TCP Packet sent to " + ta.ipEndPoint);
                log.Debug("Bytes sent : " + networkMessage.Offset);
            }

            try
            {
                mareaHeader[magicNumber.Length] = (byte)(networkMessage.Offset);
                mareaHeader[magicNumber.Length + 1] = (byte)(networkMessage.Offset >> 8);
                mareaHeader[magicNumber.Length + 2] = (byte)(networkMessage.Offset >> 16);
                mareaHeader[magicNumber.Length + 3] = (byte)(networkMessage.Offset >> 24);
                
                socket.Send(mareaHeader);
                
                while (write < networkMessage.Offset)
                    write += socket.Send(networkMessage.Buffer, write, networkMessage.Offset - write, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (!end)
                {
                    if (ex.ErrorCode == 10054)
                    {
                        //CONNECTION IS BROKEN. THE PACKETS ARE SAVED IN THE OUTBUFFER
                        outBuffer.Enqueue(networkMessage);
                        log.Debug("Packet Queued");
                    }
                    if (thTry == null)
                    {
                        thTry = new Thread(this.TrySending);
                        thTry.Start();
                    }
                }
                else
                {
                    log.Error("Socket exception while sending in Connection", ex);
                    throw new TransportException("Socket exception while sending in AsyncConnection", ex);
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception while sending in AsyncConnection", ex);
                throw new TransportException("Socket exception while sending in AsyncConnection", ex);
            }
        }

        /// <summary>
        /// Sends the packets to the queue.
        /// </summary>
        private void TrySending()
        {
            NetworkMessage networkMessage;
            while (outBuffer.Count > 0)
            {
                if (outBuffer.TryDequeue(out networkMessage))
                    this.Send(networkMessage);
            }
            thTry = null;
        }

    }
}
#endif