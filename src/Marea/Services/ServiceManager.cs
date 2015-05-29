using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;

namespace Marea
{
    public class ServiceManager
    {
        // This dictionary holds the services description (IDU information)
        private Dictionary<String, ServiceDescription> servicesIDU;

        //This dictionary holds the services that can be started (known) to the container (SDU information)
        private Dictionary<String, ServiceImplementation> servicesSDU;

        //This dictionary holds the all the services
        public Dictionary<MareaAddress, IService> services;

        //This dictionary holds the all the runing services in the actual node
        public Dictionary<MareaAddress, IService> runningServices;

        //This dictionary holds the all the runing services in the actual node
        public Dictionary<MareaAddress, IService> proxies;

        //QueryManager
        public QueryManager queryManager;

        //DescriptionBuilder
        protected DescriptionBuilder descriptionBuilder;

        //ImplementationBuilder
        protected ImplementationBuilder implementationBuilder;

        //Assembly Manager
        protected AssembliesManager assembliesManager;

        //The service container.
        protected MethodInfo methodCreatePrimitiveGeneric;

        //The service container.
        protected ServiceContainer container;

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceManager(ServiceContainer container)
        {
            //Necessary to load services properly
            this.assembliesManager = AssembliesManager.Instance;

            this.container = container;
            this.servicesIDU = new Dictionary<string, ServiceDescription>();
            this.servicesSDU = new Dictionary<string, ServiceImplementation>();
            this.services = new Dictionary<MareaAddress, IService>();
            this.runningServices = new Dictionary<MareaAddress, IService>();
            this.proxies = new Dictionary<MareaAddress, IService>();
            this.queryManager = new QueryManager(container);
            this.descriptionBuilder = new DescriptionBuilder();
            this.implementationBuilder = new ImplementationBuilder();

            //MethodInfo to create primitives: Used to call dinamically CreatePrimitive<T>(ServiceAddress serviceAddress, String primitiveName)
            this.methodCreatePrimitiveGeneric = typeof(ServiceManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name == "CreatePrimitive")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                }).Select(x => x.Method)
                .First();
        }



        #region START_SERVICE

        /// <summary>
        /// Starts ServiceManager.
        /// </summary>
        public void Start()
        {
            LoadConfiguration();

            container.discoverProtocol.StartDiscovering("*/*/*/*/Marea.NodeManager");
        }

        /// <summary>
        /// Starts a new service given its name.
        /// </summary>
        public MareaAddress StartService(String service)
        {
            return StartService(container.defaultSubsystem, service);
        }

        /// <summary>
        /// Starts a new service given its name and subsystem.
        /// </summary>
        public MareaAddress StartService(String subsys, String service)
        {
            int[] instanceIds = new int[0];


            //TODO El numero de instancia deberia ser unico, no en base al nombre del servicio
            lock (runningServices)
            {
                //Get IDs of a the given service type
                instanceIds = (from mareaAddress in runningServices.Keys
                               where mareaAddress.GetService() == service
                               select Convert.ToInt32(mareaAddress.GetInstance())).ToArray();

            }
            //Get the lower ID
            int num = Enumerable.Range(0, instanceIds.Length + 1)
             .Except(instanceIds)
             .Min();

            return StartService(subsys, num.ToString(), service);
        }

        /// <summary>
        /// Starts a new service given its subsystem, name and instance.
        /// </summary>
        public MareaAddress StartService(String subsys, String instance, String serviceName)
        {
            return StartService(subsys, instance, serviceName, true);
        }

        /// <summary>
        /// Starts a new service given its subsystem, name and instance.
        /// </summary>
        private MareaAddress StartService(String subsys, String instance, String serviceName, bool start)
        {
            return StartService(subsys, container.defaultNode, instance, serviceName, start);
        }

        /// <summary>
        /// Starts a new service given its subsystem, name and instance.
        /// </summary>
        public MareaAddress StartService(String subsys, IPEndPoint node, String instance, String serviceName)
        {
            return StartService(subsys, node, instance, serviceName, true);
        }

        /// <summary>
        /// Starts a new service given its subsystem, name and instance.
        /// </summary>
        private MareaAddress StartService(String subsys, IPEndPoint node, String instance, String serviceName, bool start)
        {
            ServiceImplementation impl = null;
            ServiceDescription desc = null;
            MareaAddress mad = null;
            ServiceAddress sad = null;

            //Get the SDU and IDU of a service by a given service name
            lock (servicesSDU)
                servicesSDU.TryGetValue(serviceName, out impl);

            lock (servicesIDU)
                servicesIDU.TryGetValue(serviceName, out desc);

            //If the SDU and the IDU exist
            if (impl != null && desc != null)
            {
                //Get MareaAddress and ServiceAddress from the input parameters
                mad = new MareaAddress(subsys, node, instance, serviceName);
                sad = new ServiceAddress(mad);

                //Check if the service is running: It's present in running or services dictionaries
                if (runningServices.ContainsKey(mad) || services.ContainsKey(mad))
                {
                    throw new Exception("Service " + mad + " is already running!");
                }

                //Creates and instance of the given service type
                var service = Activator.CreateInstance(impl.Type);

                //Create the primtives
                CreateAndSetPrimitivesToServiceWithReflection((IService)service, sad, desc, impl);

                //If the service it a consumer....
                if (impl.ConsumerServices.Length > 0)
                {
                    //Get information for each of the publishers of the the consumer.
                    foreach (ConsumerServiceInfo consumerServiceInfo in impl.ConsumerServices)
                    {
                        //Get LocateService tag (Query MareaAddress)
                        MareaAddress consumerQueryMad = new MareaAddress(consumerServiceInfo.Attribute);

                        //Get the QueryService
                        QueryService queryService = queryManager.GetQuery(consumerQueryMad);

                        //Set the QueryService to the consumer
                        consumerServiceInfo.Field.SetValue(service, queryService);

                        //Find and subscribe all the producers to the query
                        int totalProducers = queryManager.SubscribeProducersToQuery(consumerQueryMad);

                        //Discover the producers through the Discovering of the LocateService tag (query or single MareaAddress)
                        if (totalProducers == 0 && !consumerQueryMad.isQueryAddress())
                            container.discoverProtocol.StartDiscovering(consumerQueryMad.ToString());
                        else if (totalProducers>0)
                            //If there are one ore more producers, subscribe primitives thorugh discover protocol
                            container.subscribeProtocol.SubscribePrimitives(consumerServiceInfo, mad);
                        
                        if(consumerQueryMad.isQueryAddress())
                            container.discoverProtocol.StartDiscovering(consumerQueryMad.ToString());

                    }
                }
                //If the service is a producer....
                if (!desc.isEmpty())
                {
                    container.discoverProtocol.PublishService(mad);
                    queryManager.SubscribeServiceToExistentQueries(sad, (IService)service);
                }

                IService s = (IService)service;

                //Add the service into the services dictionary
                lock (services)
                {
                    if (!services.ContainsKey(mad))
                        services.Add(mad, s);
                }

                //Starts the service and adds into the running dictionary
                if (start)
                {
                    if (!s.Start(container, sad))
                        throw new Exception("Service " + mad + " cannot start!");

                    lock (runningServices)
                    {
                        if (!runningServices.ContainsKey(mad))
                            runningServices.Add(mad, s);
                    }
                }
            }

            //If the SDU or the IDU or both do not exist throw an exception
            else
            {
                if (impl == null && desc == null)
                    throw new Exception("Service IDU and SDU of service " + serviceName + " not found!");
                else if (impl == null)
                    throw new Exception("Service IDU of service" + serviceName + " not found!");
                else if (desc == null)
                    throw new Exception("Service SDU of service" + serviceName + " not found!");
            }

            return mad;
        }

        /// <summary>
        /// Starts a new service given its subsystem, name and instance.
        /// </summary>
        private MareaAddress StartServiceWithoutQueries(String subsys, IPEndPoint node, String instance, String serviceName, bool start)
        {
            bool remoteProducerIsNull = false;
            ServiceImplementation impl = null;
            ServiceDescription desc = null;
            MareaAddress mad = null;
            ServiceAddress sad = null;
            IService s = null;

            if (servicesSDU.TryGetValue(serviceName, out impl))
            {
                mad = new MareaAddress(subsys, node, instance, serviceName);
                sad = new ServiceAddress(mad);

                lock (services)
                {
                    //If the service exists
                    if (services.ContainsKey(mad))
                    {
                        Service runningService = (Service)services[mad];

                        //If the service is not started
                        if (!runningServices.ContainsKey(mad))
                        {
                            //Start service and added it in the running list
                            SetConsumersFromService(mad, runningService);
                            runningService.Start(container, sad);
                            lock (runningServices)
                            {
                                if (!runningServices.ContainsKey(mad))
                                    runningServices.Add(mad, runningService);
                            }

                            return mad;
                        }
                        else
                        {
                            throw new Exception("Service " + mad + " is already running!");
                        }
                    }
                }

                //Get the ServiceDescription by the given service type (string)
                servicesIDU.TryGetValue(serviceName, out desc);

                //MethodInfo method = null;
                //MethodInfo generic = null;

                //Get fields from the service type

                var service = Activator.CreateInstance(impl.Type);

                //////////////////////////////////////////////////IDU OPERATIONS/////////////////////////////////////////////
                //Create the primtives
                CreateAndSetPrimitivesToServiceWithReflection((IService)service, sad, desc, impl);

                //////////////////////////////////////////////////SDU OPERATIONS/////////////////////////////////////////////
                //If it is a consumer
                if (impl.ConsumerServices.Length > 0)
                {
                    ServiceDescription consumerServiceDescription = null;

                    //Get information for each of the publishers of the the consumer.
                    foreach (ConsumerServiceInfo consumerServiceInfo in impl.ConsumerServices)
                    {
                        MareaAddress consumerMad = new MareaAddress(consumerServiceInfo.Attribute);
                        Type consumerType = AssembliesManager.Instance.GetTypeFromFullName(consumerServiceInfo.Field.FieldType.FullName);
                        IService consumerService = null;

                        //What happens if the publisher its null and it local
                        if (!services.ContainsKey(consumerMad) && !consumerMad.isRemote(subsys, node))
                        {
                            //Create the publisher service but do not start it
                            StartService(subsys, consumerMad.GetInstance(), consumerMad.GetService(), false);
                        }

                        //Get the publiser or remote publisher
                        if (services.ContainsKey(consumerMad))
                            consumerService = services[consumerMad];
                        //If the publiser does not exists
                        else
                        {
                            //If it is a remote service: An entry exists in the proxies dictionary
                            if (proxies.TryGetValue(consumerMad, out consumerService))
                            {
                                //If is a remoteProducer/pseudo empty service, the corresponding proxy is created 
                                if (consumerService.GetType() == typeof(RemoteProducer))
                                {
                                    consumerService = (IService)Activator.CreateInstance(consumerServiceInfo.ProxyType, container, new ServiceAddress(consumerMad.ToString()), ((RemoteProducer)consumerService).ControlAddress);
                                    ((RemoteProducer)consumerService).UnsubscribePrimitive = container.subscribeProtocol.Unsubscribe;
                                    proxies[consumerMad] = consumerService;
                                    lock (services)
                                    {
                                        services.Add(consumerMad, consumerService);
                                    }
                                }
                            }
                            else
                            {
                                remoteProducerIsNull = true;
                            }
                        }
                        //Set the publisher in the consumer
                        consumerServiceInfo.Field.SetValue(service, consumerService);

                        //If the publiser inside of the consumer it is an IDU
                        if (consumerType.IsInterface)
                            consumerServiceDescription = servicesIDU.Where(x => consumerType.IsAssignableFrom(AssembliesManager.Instance.GetTypeFromFullName(x.Key))).Select(x => ((ServiceDescription)x.Value)).FirstOrDefault();

                        //If the publiser inside of the consumer it is a SDU
                        else if (consumerType.IsClass && typeof(Service).IsAssignableFrom(consumerType))
                        {
                            consumerServiceDescription = servicesIDU[consumerType.FullName];
                        }

                        //If the publiser inside of the consumer has a description
                        if (consumerServiceDescription != null)
                        {
                            if (!remoteProducerIsNull)
                            {
                                container.subscribeProtocol.SubscribePrimitives(consumerServiceInfo, mad);
                            }
                            else
                                container.discoverProtocol.StartDiscovering(consumerMad.ToString());
                        }

                    }
                }
                //If the service is a producer
                else
                {
                    container.discoverProtocol.PublishService(mad);
                }

                s = (IService)service;
                SetConsumersFromService(mad, s);

                lock (services)
                {
                    if (!services.ContainsKey(mad))
                        services.Add(mad, s);
                }

                if (start)
                {
                    if (!remoteProducerIsNull)
                    {
                        if (!s.Start(container, sad))
                        {
                            throw new Exception("Service " + mad + " cannot start!");
                        }

                    }
                    lock (runningServices)
                    {
                        if (!runningServices.ContainsKey(mad))
                            runningServices.Add(mad, s);
                    }
                }
            }
            else
            {
                throw new Exception("Service " + serviceName + " not found!");
            }

            //TODO
            //queryManager.SubscribeServiceToExistentQueries(mad, s);
            return mad;
            //excepciones: not found, repeated, not started // Se marcan de alguna forma con [ ]
        }

        #endregion

        #region STOP_SERVICE

        /// <summary>
        /// Stops all the services.
        /// </summary>
        public void Stop()
        {
            lock (services)
            {
                foreach (MareaAddress mad in new List<MareaAddress>(runningServices.Keys))
                {
                    StopService(mad);
                }
            }
        }

        /// <summary>
        /// Stops a service given its mareaAddress.
        /// </summary>
        public bool StopService(MareaAddress serviceAddress)
        {
            bool result = false;

            IService service = null;
            ServiceDescription description = null;

            if (services.TryGetValue(serviceAddress, out service))
            {
                ConsumerServiceInfo[] consumersInfo = servicesSDU[serviceAddress.GetService()].ConsumerServices;
                IService[] remoteProducers = new RemoteProducer[0];
                int proxyMadsCount = 0;

                //If it's a consumer
                if (consumersInfo.Length > 0)
                {
                    foreach (ConsumerServiceInfo cInfo in consumersInfo)
                    {
                        MareaAddress mad = new MareaAddress(cInfo.Attribute);
                        lock (proxies)
                        {
                            remoteProducers = (IService[])proxies.Where(kpv => (kpv.Key.Equals(mad) || QueryManager.MareaAddressMatchesWithQueryAddress(kpv.Key, mad)) && typeof(RemoteProducer).IsAssignableFrom(kpv.Value.GetType())).Select(kpv => kpv.Value).ToArray();
                        }
                        description = servicesIDU[mad.GetService()];

                        foreach (IService remoteProducer in remoteProducers)
                        {
                            lock (proxies)
                            {
                                //Find remote producers. If there is more than 1 dont remove from the proxy and service lists
                                proxyMadsCount = (from proxyMad in services.Keys
                                                  where proxyMad.GetService() == serviceAddress.GetService() && proxyMad.GetSubsystem() == serviceAddress.GetSubsystem() && proxyMad.GetNode() == serviceAddress.GetNode()
                                                  select proxyMad).Count();
                                if (proxyMadsCount == 1)
                                {
                                    foreach (VariableDescription vDesc in description.variables)
                                        ((RemoteProducer)remoteProducer).Unsubscribe(new MareaAddress(((RemoteProducer)remoteProducer).id.ToString() + "/" + vDesc.Name), serviceAddress, PrimitiveType.Variable);
                                    foreach (EventDescription eDesc in description.events)
                                        ((RemoteProducer)remoteProducer).Unsubscribe(new MareaAddress(((RemoteProducer)remoteProducer).id.ToString() + "/" + eDesc.Name), serviceAddress, PrimitiveType.Event);

                                    proxies.Remove(mad);
                                }
                            }
                            lock (services)
                            {
                                if (proxyMadsCount == 1)
                                {
                                    services.Remove(mad);
                                }
                            }

                            queryManager.UnsubscribeServiceFromExistentQueries(((MareaAddress)((RemoteProducer)remoteProducer).id), remoteProducer);
                        }

                    }
                }
                //If it is a producer...
                else if (consumersInfo.Length == 0)
                {
                    //DISCOVER PROTOCOL: Unpublish service
                    container.discoverProtocol.Unpublish(serviceAddress, container.network.Broadcast);


                    description = servicesIDU[serviceAddress.GetService()];

                    //Search in the proxy list variables of the service that are used by the existent remote consumers
                    MareaAddress primitiveMad = new MareaAddress(serviceAddress);

                    foreach (VariableDescription vDesc in description.variables)
                    {
                        primitiveMad.SetPrimitive(vDesc.Name);
                        //container.subscribeProtocol.RemovePrimitiveSubscription(primitiveMad);
                    }

                    foreach (EventDescription eDesc in description.events)
                    {
                        primitiveMad.SetPrimitive(eDesc.Name);
                        //container.subscribeProtocol.RemovePrimitiveSubscription(primitiveMad);
                    }

                    container.serviceManager.queryManager.UnsubscribeServiceFromExistentQueries(serviceAddress, service);
                }

                result = ((IService)service).Stop();

                if (result)
                {
                    lock (runningServices)
                    {
                        runningServices.Remove(serviceAddress);
                    }

                    lock (services)
                    {
                        services.Remove(serviceAddress);
                    }
                }

            }
            else
                throw new Exception("Service " + serviceAddress + " cannot stop!");

            return result;
        }

        /// <summary>
        /// Stops a service given its subsystem, name and instance.
        /// </summary>
        public bool StopService(String subsys, String service, String instance)
        {
            return StopService(new MareaAddress(subsys, container.defaultNode, service, instance));
        }

        /// <summary>
        /// Stops a service given its MareaAddress.
        /// </summary>
        public bool StopServiceWithoutQueries(MareaAddress service)
        {
            bool result = false;

            ConsumerServiceInfo[] consumersInfo = servicesSDU[service.GetService()].ConsumerServices;
            IService[] remoteProducers = new RemoteProducer[0];

            if (consumersInfo.Length > 0)
            {
                foreach (ConsumerServiceInfo cInfo in consumersInfo)
                {
                    MareaAddress mad = new MareaAddress(cInfo.Attribute);
                    remoteProducers = (IService[])proxies.Where(kpv => kpv.Key.Equals(mad) && typeof(RemoteProducer).IsAssignableFrom(kpv.Value.GetType())).Select(kpv => kpv.Value).ToArray();

                    foreach (IService remoteProducer in remoteProducers)
                    {
                        ServiceDescription sDesc = servicesIDU[mad.GetService()];

                        int proxyMadsCount;
                        lock (proxies)
                        {
                            //Find remote producers. If there is more than 1 dont remove from the proxy and service lists
                            proxyMadsCount = (from proxyMad in services.Keys
                                              where proxyMad.GetService() == service.GetService() && proxyMad.GetSubsystem() == service.GetSubsystem() && proxyMad.GetNode() == service.GetNode()
                                              select proxyMad).Count();
                            if (proxyMadsCount == 1)
                            {
                                foreach (VariableDescription vDesc in sDesc.variables)
                                    ((RemoteProducer)remoteProducer).Unsubscribe(new MareaAddress(mad + "/" + vDesc.Name), service, PrimitiveType.Variable);
                                foreach (EventDescription eDesc in sDesc.events)
                                    ((RemoteProducer)remoteProducer).Unsubscribe(new MareaAddress(mad + "/" + eDesc.Name), service, PrimitiveType.Event);

                                proxies.Remove(mad);
                            }
                        }
                        lock (services)
                        {
                            if (proxyMadsCount == 1)
                            {
                                services.Remove(mad);
                            }
                        }
                    }

                }

            }
            //DISCOVER PROTOCOL: Unpublish service
            else
                container.discoverProtocol.Unpublish(service, container.network.Broadcast);

            ServiceDescription description = servicesIDU[service.GetService()];

            //Search in the proxy list variables of the service that are used by the existent remote consumers
            MareaAddress remoteBroadcastConsumerMad = new MareaAddress(service);
            remoteBroadcastConsumerMad.SetNode(((IpTransportAddress)container.network.Broadcast).ipEndPoint);

            foreach (VariableDescription vDesc in description.variables)
            {
                remoteBroadcastConsumerMad.SetPrimitive(vDesc.Name);

                lock (proxies)
                {
                    proxies.Remove(remoteBroadcastConsumerMad);
                }
            }

            MareaAddress primitiveMad = new MareaAddress(service);
            string[] consumersServiceTypes = FindConsumersFromService(service);
            foreach (EventDescription eDesc in description.events)
            {
                foreach (string consumersServiceType in consumersServiceTypes)
                {
                    primitiveMad.SetPrimitive(eDesc.Name);
                    primitiveMad.SetService(consumersServiceType);

                    MareaAddress[] remoteConsumersMad = new MareaAddress[0];

                    lock (proxies)
                    {
                        remoteConsumersMad = proxies.Where(kvp => kvp.Key.GetSubsystem() == primitiveMad.GetSubsystem()
                            && kvp.Key.GetInstance() == primitiveMad.GetInstance() && kvp.Key.GetService() == primitiveMad.GetService()
                            && kvp.Key.GetPrimitive() == primitiveMad.GetPrimitive()).Select(kvp => (MareaAddress)kvp.Key).ToArray();
                    }

                    if (remoteConsumersMad.Length > 0)
                    {
                        lock (proxies)
                        {

                            foreach (MareaAddress remoteConsumerMad in remoteConsumersMad)
                            {
                                proxies.Remove(remoteConsumerMad);
                            }
                        }
                    }
                }
            }

            lock (services)
            {
                IService s;

                if (!services.TryGetValue(service, out s))
                    return false;

                result = s.Stop();

                if (result)
                {
                    lock (runningServices)
                    {
                        runningServices.Remove(service);
                    }


                    lock (services)
                    {   //To work properly in case of a producer is stopped with a local consumer in the same MAREA instance
                        if (consumersInfo.Count() > 0)
                        {
                            services.Remove(service);
                        }
                    }
                }
            }

            return result;
        }

        #endregion

        #region PRIMITIVES

        /// <summary>
        /// Creates a primitive of the given generic type by the given ServiceAddress and primitive name
        /// </summary>
        public T CreatePrimitive<T>(ServiceAddress serviceAddress, String primitiveName) where T : Primitive
        {
            Type type = typeof(T);

            //To provide call support of Variable<>, Event<> generic types from Services (proxies)
            if (typeof(T).GetGenericTypeDefinition() == typeof(Variable<>))
            {
                type = typeof(VariableImpl<>).MakeGenericType(new System.Type[] { type.GenericTypeArguments[0] });
            }
            else if (typeof(T).GetGenericTypeDefinition() == typeof(Event<>))
            {
                type = typeof(EventImpl<>).MakeGenericType(new System.Type[] { type.GenericTypeArguments[0] });
            }

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            Type specificType = type.GetGenericTypeDefinition().MakeGenericType(new System.Type[] { type.GetGenericArguments().FirstOrDefault() });
            return (T)Activator.CreateInstance(specificType, flags, null, new object[] { serviceAddress, serviceAddress.GetServiceAddress() + '/' + primitiveName }, null);
        }

        /// <summary>
        /// Creates the primitives and set them to the given service
        /// </summary>
        private void CreateAndSetPrimitivesToServiceWithReflection(IService service, ServiceAddress mareaAddress, ServiceDescription description, ServiceImplementation implementation)
        {
            MethodInfo generic = null;

            //Create variables
            foreach (VariableDescription vDesc in description.variables)
            {
                if (vDesc.Type != null || vDesc.Name != null)
                {
                    //Type t = AssembliesManager.Instance.typesCache[vDesc.Type];
                    Type t = AssembliesManager.Instance.GetTypeFromFullName(vDesc.Type);
                    if (vDesc.Publish)
                    {
                        FieldInfo variableField = implementation.PrimitiveFields[vDesc.Name];
                        generic = methodCreatePrimitiveGeneric.MakeGenericMethod(typeof(VariableImpl<>).MakeGenericType(t));

                        var returnVariable = generic.Invoke(this, new object[] { mareaAddress, vDesc.Name });
                        variableField.SetValue(service, returnVariable);
                    }
                }
                else
                    throw new NullReferenceException("VariableDescription attributtes not found in " + mareaAddress.GetService() + " service");
            }

            //Create events
            foreach (EventDescription eDesc in description.events)
            {
                if (eDesc.Type != null || eDesc.Name != null)
                {
                    Type t = AssembliesManager.Instance.GetTypeFromFullName(eDesc.Type);
                    if (eDesc.Publish)
                    {
                        FieldInfo eventField = implementation.PrimitiveFields[eDesc.Name];
                        generic = methodCreatePrimitiveGeneric.MakeGenericMethod(typeof(EventImpl<>).MakeGenericType(t));

                        var returnEvent = generic.Invoke(this, new object[] { mareaAddress, eDesc.Name });
                        eventField.SetValue(service, returnEvent);
                    }
                }
                else
                    throw new NullReferenceException("EventDescription attributtes not found in " + mareaAddress.GetService() + " service");
            }
        }

        #endregion

        #region LOADER

        /// <summary>
        /// Loads all the DLLs in the services directory and fills ServiceManager
        /// data structures.
        /// </summary>
        public void LoadServices()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
#if __MonoCS__
                if(!a.Location.StartsWith(AppDomain.CurrentDomain.BaseDirectory)) 
                {
                    continue;
                }
#endif
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.GetInterface("Marea.IService") != null && t != typeof(Service) && t.GetInterface("Marea.IProxyService") == null) // && t != typeof(IProxyService)
                        {
                            ServiceImplementation impl = implementationBuilder.CreateImplemementation(t);
                            ServiceDescription desc = descriptionBuilder.CreateDescription(t);

                            lock (servicesSDU)
                            {
                                servicesSDU.Add(desc.Name, impl);
                            }

                            lock (servicesIDU)
                            {
                                servicesIDU.Add(desc.Name, desc);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {

                }
            }

        }

        /// <summary>
        /// Loads the startup.xml and starts the services there. It also     
        /// configures the default subsystem.
        /// </summary>
        public void LoadConfiguration()
        {
            //TODO
        }

        #endregion


        /// <summary>
        /// Gets all the runing services of the local node from the given type.
        /// </summary>
        public T[] GetLocalService<T>() where T : IService
        {
            return services.Values.Where(x => x.GetType() == typeof(T)).Select(x => x).OfType<T>().ToArray();
        }

		/// <summary>
		/// Gets all the local services.
		/// </summary>
		public MareaAddress[] GetLocalServices() {
			return services.Keys.ToArray ();
		}

        /// <summary>
        /// Returns the name of service classes that are consumers of a given service
        /// </summary>
        public string[] FindConsumersFromService(MareaAddress serviceMad)
        {
            //Find consumers for the service
            string[] consumersServiceTypes = (from serviceImplementation in servicesSDU.Values
                                              from consumerserviceInfo in serviceImplementation.ConsumerServices
                                              where new MareaAddress(consumerserviceInfo.Attribute).GetService() == serviceMad.GetService()
                                              select serviceImplementation.Type.FullName).ToArray();
            return consumersServiceTypes;
        }

        /// <summary>
        ///Sets the consumers for a given service 
        /// </summary>
        private bool SetConsumersFromService(MareaAddress service, IService value)
        {
            bool consumersSetted = false;

            //Find consumers for the service
            string[] consumersServiceTypes = FindConsumersFromService(service);

            //Only happens if the stopped service has possible consumers
            if (consumersServiceTypes.Length > 0)
            {
                //Find services which consume or have references to the stopped service
                IService[] consumerServices = services.Where(kvp => consumersServiceTypes.Contains(kvp.Key.GetService())).Select(kvp => kvp.Value).ToArray();

                //For each service which consumes the service set its reference
                foreach (IService consumerService in consumerServices)
                {
                    //Finds the FieldInfo of the consumer in the cache
                    ConsumerServiceInfo[] cInfos = servicesSDU[consumerService.GetType().FullName].ConsumerServices.Where(inf => new MareaAddress(inf.Attribute).GetService() == service.GetService()).Select(inf => inf).ToArray();

                    //Sets the reference to the producer
                    foreach (ConsumerServiceInfo cInfo in cInfos)
                    {
                        cInfo.Field.SetValue(consumerService, value);
                        consumersSetted = true;
                    }
                }
            }

            return consumersSetted;
        }

        /// <summary>
        /// Returns the ServiceDescription of a service class by its name
        /// </summary>
        public ServiceDescription GetDescription(String service)
        {
            try
            {
                lock (servicesIDU)
                {
                    return servicesIDU[service];

                }
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("Service " + service + " not known!");
            }
        }

        /// <summary>
        /// Returns the ServiceImplementation of a service class by its name
        /// </summary>
        public ServiceImplementation GetImplementation(String service)
        {
            try
            {
                lock (servicesSDU)
                {
                    return servicesSDU[service];

                }
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("Service " + service + " not known!");
            }
        }

        /// <summary>
        /// Returns all the ServiceImplementations
        /// </summary>
        public ServiceImplementation[] GetImplementations()
        {
            lock (servicesSDU)
            {
                return servicesSDU.Values.ToArray();

            }
        }

        /// <summary>
        /// Returns alls the avaliable services
        /// </summary>
        public Dictionary<String, ServiceImplementation> GetSDUs()
        {
            lock (servicesSDU)
            {
                return servicesSDU;

            }
        }
    }
}
