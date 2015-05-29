using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;

namespace Marea
{
    public enum SubscribeOption { Subscribe, Unsubscribe };

    public class SubscribeProtocol : IProtocol
    {
        /// <summary>
        ///ServiceContainer instance.
        /// </summary>
        private ServiceContainer container;

        /// <summary>
        ///Delegate used to send data through the remoteConsumer.
        /// </summary>
        private SendDataPrimitiveDelegate sendData;

        /// <summary>
        ///HasSet used to know the primitives that are actualy subscribed from the consumer to the query.
        /// </summary>
        //private HashSet<MareaAddress> subscribedPrimitives;

        /// <summary>
        ///Sends a Subscribe message.
        /// </summary>
        public SubscribeProtocol(ServiceContainer container)
        {
            this.container = container;
            this.sendData = Data;
            //this.subscribedPrimitives = new HashSet<MareaAddress>();
        }

        /// <summary>
        ///Sends a Subscribe message.
        /// </summary>
        public void SubscribePrimitive(MareaAddress address, PrimitiveType type, TransportAddress control, MareaAddress madConsumer)
        {
            List<TransportAddress> tmp = new List<TransportAddress>();

            //MAREA 1
            switch (type)
            {
                case PrimitiveType.Variable:
                    tmp.Add(container.network.Broadcast);
                    break;
                case PrimitiveType.Event:
                    tmp.Add(container.network.Control);
                    break;
                case PrimitiveType.File:
                    throw new NotImplementedException();
            }

            //Send Subscribe
            Subscribe subscribe = new Subscribe(address.GetPrimitiveAddress(), madConsumer.GetServiceAddress(), type, container.network.Control, tmp.ToArray());
            container.SendMessage(control, subscribe);
        }

        /// <summary>
        /// Proccess a Subscribe message.
        /// </summary>
        void SubscribePrimitiveProcess(Message m)
        {
            //bool isSubscribed = false;
            IService remoteConsumerService = null;
            RemoteConsumer remoteConsumer = null;

            Subscribe subscribe = (Subscribe)m;
            MareaAddress remoteProducerPrimitiveAddress = new MareaAddress(subscribe.name);
            MareaAddress remoteConsumerServiceAddress = new MareaAddress(remoteProducerPrimitiveAddress.GetPrimitiveAddress());
            remoteConsumerServiceAddress.SetInstance("*");

            if (subscribe.primitive == PrimitiveType.Event)
                remoteConsumerServiceAddress.SetNode(new MareaAddress(subscribe.requester).GetNode());
            else if (subscribe.primitive == PrimitiveType.Variable)
            {
                remoteConsumerServiceAddress.SetNode(((IpTransportAddress)subscribe.data[0]).ipEndPoint);
            }

            lock (container.serviceManager.proxies)
            {
                if (!container.serviceManager.proxies.TryGetValue(remoteConsumerServiceAddress, out remoteConsumerService))
                {
                    if (subscribe.primitive == PrimitiveType.Event)
                        remoteConsumer = new RemoteConsumer(subscribe.data[0], subscribe.control, container.serviceManager.GetDescription(remoteProducerPrimitiveAddress.GetService()), sendData);
                    else if (subscribe.primitive == PrimitiveType.Variable)
                        remoteConsumer = new RemoteBroadcastConsumer(subscribe.data[0], subscribe.control, container.serviceManager.GetDescription(remoteProducerPrimitiveAddress.GetService()), sendData);

                    container.serviceManager.proxies.Add(remoteConsumerServiceAddress, remoteConsumer);

                }

                else
                {
                    //isSubscribed = true;
                    if (subscribe.primitive == PrimitiveType.Variable && !((RemoteBroadcastConsumer)remoteConsumerService).control.Contains(subscribe.control))
                        ((RemoteBroadcastConsumer)remoteConsumerService).AddControlAddress(subscribe.control);

                    remoteConsumer = (RemoteConsumer)remoteConsumerService;
                }
            }

            SubscribeACK(remoteProducerPrimitiveAddress, subscribe.primitive, subscribe.data[0], subscribe.control);

            //if (!isSubscribed)
            //{
                //lock (subscribedPrimitives)
                //{
                    //if (!subscribedPrimitives.Contains(remoteProducerPrimitiveAddress))
                    //{
                    //Get the primitive and subscribe to the remote consumer with reflection
                    ProtocolUtils.SubOrUnsubPrimitiveWithReflection(container, subscribe.primitive, remoteProducerPrimitiveAddress, remoteConsumer, "NotifyPrimitive", SubscribeOption.Subscribe);
                    //    subscribedPrimitives.Add(remoteProducerPrimitiveAddress);
                    //}
                //}
            //}

        }

        /// <summary>
        /// Subscribes to all the primitves of the producers by the given ConsumerServiceInfo.
        /// </summary>
        public void SubscribePrimitives(ConsumerServiceInfo consumerServiceInfo, MareaAddress madConsumer)
        {
            List<KeyValuePair<MareaAddress, IService>> remoteProducer = container.serviceManager.proxies.Where(x => x.Key.GetService() == consumerServiceInfo.Field.FieldType.FullName || AssembliesManager.Instance.GetTypeFromFullName(consumerServiceInfo.Field.FieldType.FullName).IsAssignableFrom(AssembliesManager.Instance.GetTypeFromFullName(x.Key.GetService()))).ToList();
            foreach (KeyValuePair<MareaAddress, IService> remotePublisher in remoteProducer)
            {
                SubscribePrimitives(remotePublisher.Key, remotePublisher.Value, madConsumer);
            }
        }

        /// <summary>
        /// Subscribes to all the primitves of a service by the given remote MareaAddress.
        /// </summary>
        public void SubscribePrimitives(MareaAddress remoteMad, IService service, MareaAddress madConsumer)
        {
            MareaAddress mad = new MareaAddress(remoteMad);
            //SUBSCRIBE PROTOCOL: Subscribe primitives
            if (service != null)
            {
                if (typeof(RemoteProducer).IsAssignableFrom(service.GetType()) && service.GetType() != typeof(RemoteProducer))
                {
                    ServiceDescription sDescs = container.serviceManager.GetDescription(remoteMad.GetService());

                    foreach (VariableDescription vDesc in sDescs.variables)
                    {
                        mad.SetPrimitive(vDesc.Name);
                        container.subscribeProtocol.SubscribePrimitive(mad, PrimitiveType.Variable, ((RemoteProducer)service).ControlAddress, madConsumer);
                    }

                    foreach (EventDescription eDesc in sDescs.events)
                    {
                        mad.SetPrimitive(eDesc.Name);
                        container.subscribeProtocol.SubscribePrimitive(mad, PrimitiveType.Event, ((RemoteProducer)service).ControlAddress, madConsumer);
                    }
                }

            }
        }

        /// <summary>
        /// Sends SubscribeACKProcess message.
        /// </summary>
        public void SubscribeACK(MareaAddress mareaAddress, PrimitiveType primitType, TransportAddress data, TransportAddress control)
        {
            //Provide message ID for receiving primitives
            IPEndPoint ipe = mareaAddress.GetNodeAsIPEndPoint();

            //ID=> Last field of the IPAddress & Sum of the digits of the port & HashCode(mareaAddress%9999). Modulus is done in order to avoid overflow exceptions
            //The max value of the ID is 9 digits under Int32.MaxValue (10 digits)
            int id = Int32.Parse(ipe.Address.ToString().Split('.')[3] + ipe.Port.SumDigits().ToString() + Math.Abs((DateTime.Now.Ticks) % 9999));

            //Send SubscribeACK
            SubscribeACK subscribeACK = new SubscribeACK(mareaAddress.GetPrimitiveAddress(), primitType, data, id);
            container.SendMessage(control, subscribeACK);
        }

        /// <summary>
        /// Process SubscribeACKProcess message.
        /// </summary>
        void SubscribeACKProcess(Message m)
        {
            //This method only sets the data address equals to Transport.Broadcast in order to get variables and events through broadcast
            //(UDP) and the control address (TCP) respectively.
            SubscribeACK subscribeACK = (SubscribeACK)m;
            MareaAddress mad = new MareaAddress(subscribeACK.name);

            IService[] remoteProducers;
            lock (container.serviceManager.proxies)
            {
                remoteProducers = container.serviceManager.proxies.Where(kpv => kpv.Key.GetServiceAddress() == mad.GetServiceAddress() && typeof(RemoteProducer).IsAssignableFrom(kpv.Value.GetType())).Select(kpv => kpv.Value).ToArray();

                foreach (IService remoteProducer in remoteProducers)
                {
                    ((RemoteProducer)remoteProducer).DataAddress = container.network.Broadcast;
                }
            }
            //Primitive primitive = container.GetPrimitive(sad);
        }

        /// <summary>
        /// Sends a Data message.
        /// </summary>
        public void Data(TransportAddress ta, string primitiveName, object data, PrimitiveType type)
        {
            Data dataMessage = new Data(primitiveName, data, type);
            container.SendMessage(ta, dataMessage);
        }

        /// <summary>
        /// Proccess a Data message and notifies the value of the data to the remoteProducer.
        /// </summary>
        void DataProcess(Message m)
        {
            Data data = (Data)m;
            ServiceAddress primitiveMad = new ServiceAddress(data.name);

            ServiceAddress sad = new ServiceAddress(primitiveMad.GetServiceAddress());

            //This happens if the data is not necessary for any service of the current node: there is no service or proxy consumer
            if (container.serviceManager.services.ContainsKey(sad))
            {
                Primitive primitive = container.GetPrimitive(primitiveMad);

                //With Reflection
                //Get primitive implementation generic type
                Type genericArgumentType = primitive.GetType().GetGenericArguments()[0];
                Type primitiveGenericType = ProtocolUtils.GetGenericTypeImplFromPrimitive(data.primitive, genericArgumentType);

                //Get Subscribe MethodInfo from the primtive implementation (i.e. VariableImpl)
                MethodInfo notifyMethod = primitiveGenericType.GetMethod("Notify", BindingFlags.Public | BindingFlags.Instance);

                //TODO: Find an alternative with reflection
                dynamic value = data.data;
                notifyMethod.Invoke(primitive, new object[] { sad, value });
            }
        }


        /// <summary>
        /// Sends an primtive unsubscribe  message.
        /// </summary>
        public void Unsubscribe(MareaAddress mareaAddress, MareaAddress provider, PrimitiveType primitiveType, TransportAddress data, TransportAddress control)
        {
            int counterInstances = container.serviceManager.services.Keys.Where(sad => sad.GetSubsystem() == mareaAddress.GetSubsystem() && sad.GetNode() == mareaAddress.GetNode() && sad.GetService() == mareaAddress.GetService()).Select(mad => mad).Count();

            if (counterInstances == 1)
            {
                IPEndPoint ipEP = provider.GetNodeAsIPEndPoint();
                IpTransportAddress ControlTA = new IpTransportAddress(ipEP.Address, ipEP.Port, TransportMode.TCP);
                Unsubscribe unsubscribe = new Unsubscribe(mareaAddress.ToString(), provider.ToString(), primitiveType, data, ControlTA);
                container.SendMessage(control, unsubscribe);
            }
        }

        /// <summary>
        /// Unsubscribes the primitive of the remoteConsumer by the given unsubscribe message.
        /// </summary>
        void UnsubscribeProcess(Message m)
        {

            bool unsubscribePrimitive = true;
            Unsubscribe unsubscribe = (Unsubscribe)m;
            ServiceAddress pad = new ServiceAddress(unsubscribe.name);

            //lock (subscribedprimitives)
            //{
            //    if (subscribedprimitives.contains((mareaaddress)pad))
            //    {
                    Primitive primitive = container.GetPrimitive(pad);
                    MareaAddress remoteConsumerMad = null;
                    RemoteConsumer remoteConsumer = null;
                    remoteConsumerMad = new MareaAddress(pad);
                    remoteConsumerMad.SetInstance("*");

                    lock (container.serviceManager.proxies)
                    {
                        if (unsubscribe.primitive == PrimitiveType.Variable)
                        {
                            //Use RemoteBroadcastConsumer in order to manage the control port list
                            remoteConsumerMad.SetNode(((IpTransportAddress)unsubscribe.data).ipEndPoint);


                            RemoteBroadcastConsumer remoteBroadcastConsumer = (RemoteBroadcastConsumer)container.serviceManager.proxies[remoteConsumerMad];
                            remoteBroadcastConsumer.Remove(unsubscribe.control);

                            //If it is a variable only unsubscribe if the control TransportAddress list is empty
                            //This means there are not more consumers subscribed to this variable 
                            //(One remoteConsumer for all the consumer of the primitve)
                            if (remoteBroadcastConsumer.control.Count > 0)
                                unsubscribePrimitive = false;

                            remoteConsumer = (RemoteConsumer)remoteBroadcastConsumer;
                        }

                        else if (unsubscribe.primitive == PrimitiveType.Event)
                        {
                            //If it is a event unsubscribe always (one RemoteConsumer for each consumer of the primitve)
                            remoteConsumerMad.SetNode(new MareaAddress(unsubscribe.provider).GetNode());
                            remoteConsumer = (RemoteConsumer)container.serviceManager.proxies[remoteConsumerMad];
                        }

                        if (primitive != null)
                        {
                            if (unsubscribePrimitive)
                            {
                                ProtocolUtils.SubOrUnsubPrimitiveWithReflection(container, unsubscribe.primitive, pad, remoteConsumer, "NotifyPrimitive", SubscribeOption.Unsubscribe);

                                //Remove the remote consumer from the proxies dictionary

                                container.serviceManager.proxies.Remove(remoteConsumerMad);
                                //subscribedPrimitives.Remove((MareaAddress)pad);
                            }
                        }
                    }
                //}
            //}
        }
        //public bool RemovePrimitiveSubscription(MareaAddress mad)
        //{
        //    lock (subscribedPrimitives)
        //        return subscribedPrimitives.Remove(mad);
        //}

        /// <summary>
        /// Registers subscription protocol messages (subscribe, subscribeACK, Data, Unsubscribe).
        /// </summary>
        public void Start()
        {
            container.RegisterMessage(typeof(Subscribe), this.SubscribePrimitiveProcess);

            container.RegisterMessage(typeof(SubscribeACK), this.SubscribeACKProcess);

            container.RegisterMessage(typeof(Data), this.DataProcess);

            container.RegisterMessage(typeof(Unsubscribe), this.UnsubscribeProcess);
        }

        /// <summary>
        /// Unregisters subscription protocol messages (subscribe, subscribeACK, Data, Unsubscribe).
        /// </summary>
        public void Stop()
        {
            container.UnregisterMessage(this.SubscribePrimitiveProcess);

            container.UnregisterMessage(this.SubscribeACKProcess);

            container.UnregisterMessage(this.DataProcess);

            container.UnregisterMessage(this.UnsubscribeProcess);
        }
    }
}
