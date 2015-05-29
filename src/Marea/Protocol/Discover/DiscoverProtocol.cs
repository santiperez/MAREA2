using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// Discover protocol implementation
    /// </summary>
    public class DiscoverProtocol : IProtocol
    {
        /// <summary>
        /// Service Container instance.
        /// </summary>
        private ServiceContainer container;

        /// <summary> 
        /// Timer to retransmit discovers when needed. 
        /// </summary>
        protected Dictionary<String, Timer> discoverTimers;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DiscoverProtocol(ServiceContainer container)
        {
            this.container = container;
            this.discoverTimers = new Dictionary<string, Timer>();
        }

        /// <summary>
        /// Start sending Discover messages for a given service.
        /// </summary>
        public void StartDiscovering(String serviceAddress)
        {
            Timer discoverTimer = null;
            lock (discoverTimers)
            {
                if (!discoverTimers.TryGetValue(serviceAddress, out discoverTimer))
                {
                    // TODO: set correct timer period
                    discoverTimer = new Timer(new TimerCallback(Discover), new Discover(serviceAddress), 0, 3000);
                    discoverTimers.Add(serviceAddress, discoverTimer);
                }
            }
        }
        /// <summary>
        /// Stop sending Discover messages for the given service.
        /// </summary>
        public bool StopDiscovering(string serviceAddress)
        {
            Timer discoverTimer = null;
            lock (discoverTimers)
            {
                if (discoverTimers.TryGetValue(serviceAddress, out discoverTimer))
                {
                    discoverTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    discoverTimer = null;
                    discoverTimers.Remove(serviceAddress);
                    return true;

                }
                else
                {
                    MareaAddress mad = new MareaAddress(serviceAddress);
                    foreach (KeyValuePair<string, Timer> timer in discoverTimers)
                    {
                        MareaAddress qad = new MareaAddress(timer.Key);
                        if (qad.isQueryAddress())
                        {
                            if (QueryManager.MareaAddressMatchesWithQueryAddress(mad, qad) && discoverTimers.TryGetValue(timer.Key, out discoverTimer))
                            {
                                return true;
                            }
                        }

                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Method used by the discover message retransmission Timer.
        /// </summary>
        private void Discover(Object stateObject)
        {
            container.SendMessage(container.network.Broadcast, (Discover)stateObject);
        }

        /// <summary>
        /// Recognizes a petiton for publish a marea service
        /// </summary>
        void DiscoverProcess(Message m)
        {
            Discover discoverMessage = (Discover)m;
            MareaAddress mareaAddress = new MareaAddress(discoverMessage.mareaAddress);

            if (mareaAddress.isQueryAddress())
            {
                Dictionary<MareaAddress, IService> matchingServices = container.serviceManager.queryManager.GetRealServicesFromQuery(mareaAddress);
                foreach (MareaAddress mad in matchingServices.Keys)
                {
                        PublishService(mad);
                }
            }
            else
            {
                if (container.serviceManager.runningServices.ContainsKey(mareaAddress))
                    PublishService(mareaAddress);
            }
        }

        /// <summary>
        /// Anounces a service by the given MareaAdress sending a publish message
        /// </summary>
        public void PublishService(MareaAddress mareaAddress)
        {
            Publish publish = new Publish(mareaAddress.GetServiceAddress(), container.network.Control);
            container.SendMessage(container.network.Broadcast, publish);
        }

        /// <summary>
        /// Recognizes a remoteService by the incoming publish message.
        /// </summary>
        void PublishProcess(Message m)
        {

            Publish publish = (Publish)m;
            MareaAddress mad = new MareaAddress(publish.mareaAddress);

            ConsumerServiceInfo consumerServiceInfo = null;
            IService consumerService = null;
            ServiceImplementation serviceImpl = null;

            //UniqueMareaAddress->The remoteProducer has to be replaced by the appropiate proxy.
            serviceImpl = (from impl in container.serviceManager.GetImplementations()
                           from cInfo in impl.ConsumerServices
                           where cInfo.Attribute == mad.ToString()
                           select impl).FirstOrDefault();

            //QueryAddress->The remoteProducer has to be replaced by the appropiate proxy.
            if (serviceImpl == null)
            {
                serviceImpl = (from impl in container.serviceManager.GetImplementations()
                               from cInfo in impl.ConsumerServices
                               where QueryManager.MareaAddressMatchesWithQueryAddress(mad, new MareaAddress(cInfo.Attribute))
                               select impl).FirstOrDefault();
            }


            if (serviceImpl != null)
                consumerServiceInfo = serviceImpl.ConsumerServices.FirstOrDefault();

            if (consumerServiceInfo != null)
            {
                lock (container.serviceManager.proxies)
                {
                    //Add a empty remote producer with the control address
                    if (!container.serviceManager.proxies.TryGetValue(mad, out consumerService))
                    {
                        //This means that is a remoteProducer
                        //if (container.serviceManager.GetImplementation(mad.GetService()).ConsumerServices.Length == 0)
                        //{
                            consumerService = (IService)Activator.CreateInstance(consumerServiceInfo.ProxyType, container, new ServiceAddress(mad), publish.control);
                            ((RemoteProducer)consumerService).UnsubscribePrimitive = container.subscribeProtocol.Unsubscribe;
                            container.serviceManager.proxies[mad] = consumerService;
                            lock (container.serviceManager.services)
                                container.serviceManager.services[mad] = consumerService;
                            //NEW
                            container.serviceManager.queryManager.SubscribeServiceToExistentQueries(mad, consumerService);
                        //}

                        var runningServices = from s in container.serviceManager.runningServices
                                              where s.Key.GetService() == serviceImpl.Type.FullName
                                              select s;

                        if (runningServices.Count() > 0)
                        {
                            //PROTOCOL: Stop Discover
                            //This only has to be done if the LocateService attribute is not a QueryAddress
                            //if (StopDiscovering(publish.mareaAddress))
                            //{

                                //All the services that are consumers of the created proxy and which are "running" (but not started) in the local node,
                                // should set the field of the producer (proxy).
                                foreach (KeyValuePair<MareaAddress, IService> kvpService in runningServices)
                                {
                                    //NEW SETVALUE and start commented
                                    //The producer (proxy) should be setted in the consumer
                                    //consumerServiceInfo.Field.SetValue(kvpService.Value, consumerService);
                                    //The consumer should subscribe to the primitives of the proxy (psedudo-producer service) 
                                    container.subscribeProtocol.SubscribePrimitives(mad, consumerService, kvpService.Key);
                                    //The consumer should be started
                                    //kvpService.Value.Start(container, new ServiceAddress(mad));
                                }

                            //}
                        }

                    }
                    else if (typeof(RemoteProducer).IsAssignableFrom(container.serviceManager.proxies[mad].GetType()))
                    {
                        //OK!!! Do nothing
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }

        /// <summary>
        /// Unpublish a service by the given MareaAdress sending a unpublish message
        /// </summary>
        public void Unpublish(MareaAddress mareaAddress, TransportAddress transportAddress)
        {
            Unpublish unpublish = new Unpublish(mareaAddress.GetServiceAddress(), transportAddress);
            container.SendMessage(transportAddress, unpublish);
        }


        /// <summary>
        /// Deletes a remoteProducer by the incoming unpublish message.
        /// </summary>
        void UnpublishProcess(Message m)
        {
            Unpublish unpublish = (Unpublish)m;
            MareaAddress mad = new MareaAddress(unpublish.mareaAddress);
            IService service = null;

            lock (container.serviceManager.proxies)
            {
                if (container.serviceManager.proxies.TryGetValue(mad, out service))
                {
                    container.serviceManager.proxies.Remove(mad);
                }
            }

            lock (container.serviceManager.services)
            {
                if (container.serviceManager.services.TryGetValue(mad, out service))
                {
                    container.serviceManager.queryManager.UnsubscribeServiceFromExistentQueries(mad, service);
                    container.serviceManager.services.Remove(mad);

                    //This only has to be done if the LocateService attribute is not a QueryAddress
                    //StartDiscovering(mad.GetServiceAddress());
                }
            }
        }

        /// <summary>
        /// Registers Discover protocol messages (publish, unpublish, discover)
        /// </summary>
        public void Start()
        {
            container.RegisterMessage(typeof(Publish), this.PublishProcess);

            container.RegisterMessage(typeof(Unpublish), this.UnpublishProcess);

            container.RegisterMessage(typeof(Discover), this.DiscoverProcess);
        }

        /// <summary>
        /// Unregisters discover protocol messages (publish, unpublish, discover)
        /// </summary>
        public void Stop()
        {
            container.UnregisterMessage(this.PublishProcess);

            container.UnregisterMessage(this.UnpublishProcess);

            container.UnregisterMessage(this.DiscoverProcess);
        }
    }
}
