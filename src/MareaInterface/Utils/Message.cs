using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// Multicast delegate used to mange MAREA Messages.
    /// </summary>
    public delegate void MessageProcess(Message Message);

    /// <summary>
    ///Delegate used to send MAREA Data Messages.
    /// </summary>
    public delegate void SendDataPrimitiveDelegate(TransportAddress ta, String name, Object data, PrimitiveType type);

    /// <summary>
    /// Delegate used to unsuscribe primitives.
    /// </summary>
    public delegate void UnsubscribePrimitiveDelegate(MareaAddress mareaAddress, MareaAddress provider, PrimitiveType primitiveType, TransportAddress data, TransportAddress control);

    /// <summary>
    /// All Marea messages inherits from this class.
    /// </summary>
    [Serializable]
    public class Message
    {
        public Message() { }
    }

    /// <summary>
    /// Marea message used to publish a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class Publish : Message
    {
        public Publish() { }
        public String mareaAddress;
        public TransportAddress control;

        public Publish(String mareaAdress, TransportAddress control)
        {
            //this.name = name;
            //this.primitive = primitive;
            this.mareaAddress = mareaAdress;
            this.control = control;
        }
    }

    /// <summary>
    /// Marea message used to discover a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class Discover : Message
    {
        public Discover() { }
        public String mareaAddress;

        public Discover(String mareaAddress)
        {
            this.mareaAddress = mareaAddress;
        }

    }

    /// <summary>
    /// Marea message used to unpublish a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class Unpublish : Message
    {
        public Unpublish() { }
        public String mareaAddress;
        public TransportAddress control;

        public Unpublish(String mareaAddress, TransportAddress control)
        {
            this.mareaAddress = mareaAddress;
            this.control = control;
        }
    }

    /// <summary>
    /// Marea message used to subscribe to a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class Subscribe : Message
    {
        public Subscribe() { }
        public String name;
        public String requester;
        public PrimitiveType primitive;
        public TransportAddress control;
        public TransportAddress[] data;

        public Subscribe(String name, String requester, PrimitiveType primitive, TransportAddress control, TransportAddress[] data)
        {
            this.name = name;
            this.requester = requester;
            this.primitive = primitive;
            this.control = control;
            this.data = data;
        }
    }

    /// <summary>
    /// Marea message used to  recgonize a subscription to a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class SubscribeACK : Message
    {
        public SubscribeACK() { }
        public String name;
        public PrimitiveType primitive;
        public TransportAddress data;
        public int id;

        public SubscribeACK(String name, PrimitiveType primitive, TransportAddress data, int id)
        {
            this.name = name;
            this.primitive = primitive;
            this.data = data;
            this.id = id;
        }
    }

    /// <summary>
    /// Marea message used to subscribe to a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class DialogueError : Message
    {
        public DialogueError() { }
        public String name;
        public PrimitiveType primitive;
        public TransportAddress publisherControlAddress;

        public DialogueError(String name, PrimitiveType primitive, TransportAddress publisherControlAddress)
        {
            this.name = name;
            this.primitive = primitive;
            this.publisherControlAddress = publisherControlAddress;
        }
    }

    /// <summary>
    /// Marea message used to unsubscribe to a primitive among different service containers.
    /// </summary>
    [Serializable]
    public class Unsubscribe : Message
    {
        public Unsubscribe() { }
        public String name;
        public String provider;
        public PrimitiveType primitive;
        public TransportAddress control;
        public TransportAddress data;

        public Unsubscribe(String name, String provider, PrimitiveType primitive, TransportAddress data, TransportAddress control)
        {
            this.name = name;
            this.provider = provider;
            this.primitive = primitive;
            this.control = control;
            this.data = data;
        }
    }

    /// <summary>
    /// Marea message used to transfer event and variables values
    /// </summary>
    [Serializable]
    public class Data : Message
    {
        public Data() { }

        public Data(string name, object data, PrimitiveType primitive)
        {
            this.name = name;
            this.data = data;
            this.primitive = primitive;
        }
        public string name;
        public object data;
        public PrimitiveType primitive;
    }


    /// <summary>
    /// Marea message used to request a function call.
    /// </summary>
    [Serializable]
    public class CallFunction : Message
    {
        public CallFunction() { }
        public string function;
        public object[] parameters;
        public int functionId;
        public int random;
        public TransportAddress reply;

        public CallFunction(string function, object[] parameters, int functionId,int random, TransportAddress reply)
        {
            this.function = function;
            this.parameters = parameters;
            this.functionId = functionId;
            this.random = random;
            this.reply = reply;
        }
    }

    /// <summary>
    /// Marea message used to return the result of a function call.
    /// </summary>
    [Serializable]
    public class ReturnFunction : Message
    {
        public ReturnFunction() { }
        public int idFunction;
        public int random;
        public object value;

        public ReturnFunction(int random, int num_invocation, object value)
        {
            this.random = random;
            this.idFunction = num_invocation;
            this.value = value;
        }
    }
}
