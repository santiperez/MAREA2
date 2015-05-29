using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace Marea
{
    /// <summary>
    /// On-fly function call. Stores the result, the WaitHandle and a random value to check the caller.
    /// </summary>
    public class FunctionCall
    {
        /// <summary>
        /// The result object
        /// </summary>
        public object result;

        /// <summary>
        /// Random value to test the remote marea container correctness.
        /// </summary>
        public int randomValue;

        /// <summary>
        /// WaitHandle to wait for the function return to arrive in a message.
        /// </summary>
        public EventWaitHandle waitEvent;

        /// <summary>
        /// Static random number generator. 
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// Constructs a new function call.
        /// </summary>
        public FunctionCall()
        {
            this.waitEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
            randomValue = Random.Next();
        }
    }

    /// <summary>
    /// Remote Procedure Call protocol implementation.
    /// 
    /// The protocol has a dictionary with on-the-fly calls and
    /// it also has a dictionary with /MAD/function => Invoker of the function.
    /// 
    /// Each function call access this dictionary.
    /// IF the corresponding invoker is found
    ///     it is used to call the remote function
    /// ELSE
    ///     the invoker is built and stored in the cache
    /// the invoker executes the invocation
    /// </summary>
    public class RemoteProcedureCallProtocol : IProtocol, IRemoteProcedureCallProtocol
    {
        protected ServiceContainer container;
        protected Dictionary<string, IInvoke> invokersCache;
        protected Dictionary<int, FunctionCall> functionCalls;

        /// <summary>
        /// Constructs a new RPC protocol.
        /// </summary>
        /// <param name="container">the Marea container</param>
        public RemoteProcedureCallProtocol(ServiceContainer container)
        {
            this.container = container;
            this.invokersCache = new Dictionary<string, IInvoke>();
            this.functionCalls = new Dictionary<int, FunctionCall>();
        }

        /// <summary>
        /// Process the CallFunction message. This happens when a external function call is received.
        /// </summary>
        /// <param name="m">the message received</param>
        void CallFunctionProcess(Message m)
        {
            CallFunction callFunction = (CallFunction)m;

            IInvoke iInvoker = null;
            lock (invokersCache)
            {
                if (!invokersCache.TryGetValue(callFunction.function, out iInvoker))
                {
                    MareaAddress mad = new MareaAddress(callFunction.function);

                    IService service = container.GetService(new MareaAddress(mad.GetServiceAddress()));
                    iInvoker = BuildInvoker(service, mad.GetPrimitive());
                    invokersCache.Add(callFunction.function, iInvoker);
                }


            }
            object result = iInvoker.Invoke(callFunction.parameters);
            container.SendMessage(callFunction.reply, new ReturnFunction(callFunction.random,callFunction.functionId, result));
        }

        /// <summary>
        /// Calls a remote function. This happens when a external function call execution is finished and the remote container send this
        /// message with the result.
        /// </summary>
        /// <param name="ta">transport address of the marea remote container</param>
        /// <param name="function">service and function to call</param>
        /// <param name="parameters">parameters</param>
        /// <returns>the result from the remote function</returns>
        public object CallFunction(TransportAddress ta, string function, object[] parameters)
        {
            FunctionCall functionCall = new FunctionCall();
            int functionId = FunctionCall.Random.Next();
            lock (functionCalls)
                functionCalls.Add(functionId, functionCall);

            container.SendMessage(ta, new CallFunction(function, parameters, functionId,functionCall.randomValue,container.network.Control));
            functionCall.waitEvent.WaitOne(-1);

            return functionCall.result;
        }

        /// <summary>
        /// Process the ReturnFunction message.
        /// </summary>
        /// <param name="m"></param>
        void ReturnFunctionProcess(Message m)
        {
            ReturnFunction returnFunction = (ReturnFunction)m;
            FunctionCall functionCall = null;
            lock (functionCalls)
            {
                if (!functionCalls.TryGetValue(returnFunction.idFunction, out functionCall))
                {
                    System.Console.WriteLine("Function id " + returnFunction.idFunction + " not found");
                    return;
                }
                functionCalls.Remove(returnFunction.idFunction);
            }
            if (functionCall.randomValue == returnFunction.random)
            {
                functionCall.result = returnFunction.value;
                functionCall.waitEvent.Set();
            }
            else
            {
                System.Console.WriteLine("Random number does not match ");
                return;
            }
        }

        /// <summary>
        /// Builds an Invoker for the given service and function name.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        protected IInvoke BuildInvoker(IService service, string functionName)
        {
            IInvoke iInvoke = null;
            Type serviceType = service.GetType();

            MethodInfo methodInfoFunc = serviceType.GetMethod(functionName);
            Type returnType = methodInfoFunc.ReturnType;
            bool isVoid = returnType == typeof(void);
            ParameterInfo[] paramsInfo = methodInfoFunc.GetParameters();

            //TODO A parameter cache can improve this ???
            Type[] paramTypes = paramsInfo.Select(pInfo => pInfo.ParameterType).ToArray();
            Type type = null;

            if (isVoid)
                type = Type.GetType("Marea.InvokeAction`" + paramTypes.Length);
            else
            {
                type = Type.GetType("Marea.InvokeFunc`" + (paramTypes.Length + 1));
                paramTypes = paramTypes.Concat(new Type[] { returnType }).ToArray();
            }

            Type genericType = type.MakeGenericType(paramTypes);

            iInvoke = (IInvoke)Activator.CreateInstance(genericType);

            FieldInfo fieldInfo = genericType.GetField("f");

            Delegate del = ProtocolUtils.ToDelegate(methodInfoFunc, service);
            fieldInfo.SetValue(iInvoke, del);

            return iInvoke;
        }

        /// <summary>
        /// Starts the protocol registering the messages.
        /// </summary>
        public void Start()
        {
            container.RegisterMessage(typeof(CallFunction), this.CallFunctionProcess);
            container.RegisterMessage(typeof(ReturnFunction), this.ReturnFunctionProcess);
        }

        /// <summary>
        /// Stops the protocol unregistering the messages.
        /// </summary>
        public void Stop()
        {
            container.UnregisterMessage(this.CallFunctionProcess);
            container.UnregisterMessage(this.ReturnFunctionProcess);
        }

    }
}

