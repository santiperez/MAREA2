using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class ProtocolUtils
    {
        /// <summary>
        /// Calls primitve implementation Subscribe or Unsubscribe method by the given paremeters through reflection.
        /// </summary>
        public static void SubOrUnsubPrimitiveWithReflection(IServiceContainer container,PrimitiveType primitiveType, MareaAddress primitiveAddress, object service, String serviceMethodName, SubscribeOption option)
        {
            //Get the Primitve
            Primitive primitive = container.GetPrimitive(new ServiceAddress(primitiveAddress.GetPrimitiveAddress()));
            SubOrUnsubPrimitiveWithReflection(container, primitiveType, primitiveAddress, service, serviceMethodName, primitive, option);
        }

        /// <summary>
        /// Calls primitve implementation Subscribe or Unsubscribe method by the given paremeters and the given primitive through reflection.
        /// </summary>
        public static void SubOrUnsubPrimitiveWithReflection(IServiceContainer container,PrimitiveType primitiveType, MareaAddress primitiveAddress, object service, String serviceMethodName, Primitive primitive, SubscribeOption option)
        {
            //Pseudocode
            //container.GetPrimitive(new ServiceAddress(subscribe.name)).Subscribe(remoteConsumer.NotifyPrimitive);
            //container.GetPrimitive(new ServiceAddress(subscribe.name)).Unsubscribe(remoteConsumer.NotifyPrimitive);
            //((VariableImpl<double>)primitive).Subscribe(new ServiceAddress(remoteConsumerMad.ToString()),remoteConsumer.NotifyPrimitive);
            //((VariableImpl<double>)primitive).Unsubscribe(new ServiceAddress(remoteConsumerMad.ToString()),remoteConsumer.NotifyPrimitive);

            //With Reflection
            //Get primitive implementation generic type
            if (primitive != null)
            {
                Type genericArgumentType = primitive.GetType().GetGenericArguments()[0];
                Type primitiveGenericType = GetGenericTypeImplFromPrimitive(primitiveType, genericArgumentType);

                MethodInfo subscribeMethod = null;

                //Get Subscribe/Unsubscribe MethodInfo from the primitive implementation (i.e. VariableImpl)
                if (option == SubscribeOption.Subscribe)
                    subscribeMethod = primitiveGenericType.GetMethod("Subscribe", BindingFlags.Public | BindingFlags.Instance);
                else if (option == SubscribeOption.Unsubscribe)
                    subscribeMethod = primitiveGenericType.GetMethod("Unsubscribe", BindingFlags.Public | BindingFlags.Instance);
                else
                    throw new NotImplementedException() { };

                //Get NotifyPrimitve<T> MethodInfo from RemoteConsumer
                MethodInfo notifyPrimitiveMethod = service.GetType().GetMethod(serviceMethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                MethodInfo notifyPrimitiveGenericMethod = notifyPrimitiveMethod.MakeGenericMethod(genericArgumentType);

                //Get MakeNotifyFuncDelegate<T> from SubscribeProtocol
                MethodInfo makeNotifyFuncDelegateMethod = typeof(ProtocolUtils).GetMethod("MakeNotifyFuncDelegateFromService", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo makeNotifyFuncDelegateGenericMethod = makeNotifyFuncDelegateMethod.MakeGenericMethod(genericArgumentType);

                //Invoke Subscribe method with the following parameters:ServiceAddress sad, RemoteConsumer remoteConsumer.NotifyPrimitive;
                //The second parameter is a delegate and should be passed by creating a NotifyFunc<T> delegate with Delegate.CreateDelegate method
                subscribeMethod.Invoke(primitive, new object[] { new ServiceAddress(primitiveAddress), makeNotifyFuncDelegateGenericMethod.Invoke(null, new object[] { service, notifyPrimitiveGenericMethod }) });
            }
        }


        /// <summary>
        /// Method used to call Subscribe and Unsubscribe method through reflection.
        /// </summary>
        private static NotifyFunc<T> MakeNotifyFuncDelegateFromService<T>(object service, MethodInfo @get)
        {
            return (NotifyFunc<T>)Delegate.CreateDelegate(typeof(NotifyFunc<T>), service, @get);
        }


        /// <summary>
        /// Gets the implementation primitve type from a primitve type
        /// </summary>
        public static Type GetGenericTypeImplFromPrimitive(PrimitiveType primitiveType, Type genericType)
        {
            Type type = null;
            switch (primitiveType)
            {
                case PrimitiveType.Variable:
                    type = typeof(VariableImpl<>);
                    break;

                case PrimitiveType.Event:
                    type = typeof(EventImpl<>);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return type.MakeGenericType(genericType);
        }

        /// <summary>
        /// Builds a Delegate instance from the supplied MethodInfo object and a target to invoke against.
        /// </summary>
        public static Delegate ToDelegate(MethodInfo mi, object target)
        {
            if (mi == null) throw new ArgumentNullException("mi");

            Type delegateType;

            var typeArgs = mi.GetParameters()
                .Select(p => p.ParameterType)
                .ToList();

            // builds a delegate type
            if (mi.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(typeArgs.ToArray());

            }
            else
            {
                typeArgs.Add(mi.ReturnType);
                delegateType = Expression.GetFuncType(typeArgs.ToArray());
            }

            // creates a binded delegate if target is supplied
            var result = (target == null)
                ? Delegate.CreateDelegate(delegateType, mi)
                : Delegate.CreateDelegate(delegateType, target, mi);

            return result;
        }
    }
}
