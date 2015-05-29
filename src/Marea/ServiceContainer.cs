using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MareaGen;
using System.Reflection;
using System.Net;

namespace Marea
{
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// This is the default subsystem.
        /// </summary>
        public String defaultSubsystem = "EC-UPC";

        /// <summary>
        ///This is the default node
        /// </summary>
        public IPEndPoint defaultNode;

        /// <summary>
        /// Service manager.
        /// </summary>
        public ServiceManager serviceManager;

        /// <summary>
        /// Network layer
        /// </summary>
        public Network network;

        /// <summary>
        /// Discover protocol
        /// </summary>
        public DiscoverProtocol discoverProtocol;

        /// <summary>
        /// Subscribe protocol
        /// </summary>
        public SubscribeProtocol subscribeProtocol;

        /// <summary>
        /// Subscribe protocol
        /// </summary>
        public RemoteProcedureCallProtocol RPCProtocol;

        /// <summary>
        /// MFTP protocol
        /// </summary>
        public MulticastFileTransferProtocol MFTPProtocol;

        /// <summary>
        /// MessageProcess[] to manage the different MAREA messages. The indexer keeps a reference to the 
        /// id (byte) provided by MAREAGen.
        /// </summary>
        private MessageProcess[] mareaMessagesProcess;

        /// <summary>
        /// MethodInfo 
        /// </summary>
        private MethodInfo getPrimitiveFromServiceMethod;

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceContainer()
        {
            this.network = new Network(this);
            this.serviceManager = new ServiceManager(this);
            this.mareaMessagesProcess = new MessageProcess[(int)CoderBytes.NULL];
            this.discoverProtocol = new DiscoverProtocol(this);
            this.subscribeProtocol = new SubscribeProtocol(this);
            this.RPCProtocol= new RemoteProcedureCallProtocol(this);
            this.getPrimitiveFromServiceMethod = typeof(ServiceContainer).GetMethod("GetPrimitiveFromService", BindingFlags.Public | BindingFlags.Instance);
        }

		//TODO Automate this mechanism
		public T GetProtocol<T>() where T : IProtocol {
			return (T)GetProtocol (typeof(T).FullName);
		}

		private IProtocol GetProtocol(String protocolName) {
			switch (protocolName) {
				case "Marea.IRemoteProcedureCallProtocol":
				return this.RPCProtocol;
				default:
				throw new Exception ("Protocol not known!");
			}
		}

        /// <summary>
        /// Starts the Service Container (Network layer, protocols and services)
        /// </summary>
        public void Start()
        {
            //Start protocols
            discoverProtocol.Start();
            subscribeProtocol.Start();
            RPCProtocol.Start();

            //Starts network layer
            network.Start();

            //Sets node info
            defaultNode = ((IpTransportAddress)network.Control).ipEndPoint;

            serviceManager.Start();

            serviceManager.LoadServices();
        }


        /// <summary>
        /// Stops the Service Container (Network layer, protocols and services)
        /// </summary>
        public void Stop()
        {
            //Shutdown services
            serviceManager.Stop();

            //Stops protocols
            discoverProtocol.Stop();
            subscribeProtocol.Stop();
            RPCProtocol.Stop();

            //Stops network layer
            network.Stop();
        }

        /// <summary>
        /// Receive NetworkMessage callback
        /// </summary>
        public void Receive(NetworkMessage networkMessage)
        {
            MessageProcess mp = ((MessageProcess)mareaMessagesProcess[networkMessage.Id]);
            if (mp != null)
                mp((Message)networkMessage.Object);
        }

        /// <summary>
        /// Send Message
        /// </summary>
        public void SendMessage(TransportAddress transportAddress, Message message)
        {
            network.Send(transportAddress, message);
        }

        #region PRIMITIVE
        /// <summary>
        // Gets a Primitve from a given service and primitve name
        /// <summary>
        public T GetPrimitiveFromService<T>(IService service, String primitiveName) where T : Primitive
        {
            var value = default(T);

            if (service != null && primitiveName != null)
            {
                if (service is RemoteProducer)
                {
                    PropertyInfo pInfo = service.GetType().GetProperty(primitiveName);
                    value = (T)pInfo.GetValue(service);
                }
                else
                {
                    FieldInfo field = serviceManager.GetImplementation(service.GetType().FullName).PrimitiveFields[primitiveName];
                    value = (T)field.GetValue(service);
                }
            }
            return value;
        }

        /// <summary>
        // Gets a Primitve from a given service and primitve name
        /// <summary>
        [Obsolete("GetPrimitiveFieldFromService is deprecated, please use GetPrimitiveFromService<T> or GetPrimitive instead.")]
        public Primitive GetPrimitiveFieldFromService(IService service, String primitiveName)
        {
            Primitive value = null;
            if (service != null && primitiveName != null)
            {
                if (service is RemoteProducer)
                {
                    PropertyInfo pInfo = service.GetType().GetProperty(primitiveName);
                    value = (Primitive)pInfo.GetValue(service);
                }
                else
                {
                    FieldInfo field = serviceManager.GetImplementation(service.GetType().FullName).PrimitiveFields[primitiveName];
                    value = (Primitive)field.GetValue(service);
                }
            }
            return value;
        }

        /// <summary>
        // Gets a Primitve from a given ServiceAddress
        /// <summary>
        public Primitive GetPrimitive(ServiceAddress primitiveAddress)
        {
            IService service;

            lock (serviceManager.services)
            {
                if (serviceManager.services.TryGetValue(new MareaAddress(primitiveAddress.GetServiceAddress()), out service))
                {
                    string primitiveName = primitiveAddress.GetPrimitive();
                    Type t = serviceManager.GetImplementation(primitiveAddress.GetService()).PrimitiveFields[primitiveName].FieldType;
                    MethodInfo getPrimitiveFromServiceGenericMethod = getPrimitiveFromServiceMethod.MakeGenericMethod(t);
                    return (Primitive)getPrimitiveFromServiceGenericMethod.Invoke(this, new object[] { service, primitiveName });
                }
                return null;
            }
        }

        /// <summary>
        // Creates a primitive of the given generic type by the given ServiceAddress and primitive name
        /// <summary>
        public T CreatePrimitive<T>(ServiceAddress serviceAddress, String primitiveName) where T : Primitive
        {
            return serviceManager.CreatePrimitive<T>(serviceAddress, primitiveName);
        }
        #endregion

        #region QUERIES
        /// <summary>
        // Gets a dictionary of all the services of the current node which match with the given query ServiceAddress
        /// <summary>
        public Dictionary<MareaAddress, IService> GetServicesFromQuery(MareaAddress queryServiceAddress)
        {
            return serviceManager.queryManager.GetServicesFromQuery(queryServiceAddress);
        }
        #endregion

        #region PROTOCOL_MESSAGE_REGISTRATION
        /// <summary>
        /// Registers a MAREA message
        /// </summary>
        public bool RegisterMessage(Type messageType, MessageProcess messageProcess)
        {
            bool registered = false;
            byte id = MareaCoder.tables.EncodeTable.Cast<DictionaryEntry>().Where(x => ((String)x.Key) == messageType.FullName).Select(x => ((EncodeData)x.Value).id).FirstOrDefault();

            if (mareaMessagesProcess[id] == null)
            {
                mareaMessagesProcess[id] = messageProcess;
                registered = true;
            }
            return registered;
        }

        /// <summary>
        /// Unregisters a MAREA message
        /// </summary>
        public bool UnregisterMessage(MessageProcess messageProcess)
        {
            bool unregistered = false;

            int id = (byte)Array.IndexOf(mareaMessagesProcess, messageProcess);

            if (id != -1 && messageProcess != null)
            {
                mareaMessagesProcess[id] = null;
                unregistered = true;
            }
            return unregistered;
        }
        #endregion

        #region SERVICE

		public T GetService<T>(string mad) {
			return GetService<T> (new MareaAddress (mad));
		}

		public T GetService<T>(MareaAddress mad) {
			return (T)GetService (mad);
		} 

        public IService GetService(string mareaAddress)
        {
            MareaAddress mad = new MareaAddress(mareaAddress);
            return GetService(mad);
        }

        public IService GetService(MareaAddress mad)
        {
            IService service;

            //If it's a local service, running service or remote producer
            if (serviceManager.runningServices.TryGetValue(mad, out service))
                return service;
            else if (serviceManager.services.TryGetValue(mad, out service))
                return service;
            //If it's a remote consumer    
            else if (serviceManager.proxies.TryGetValue(mad, out service))
                return service;

            return null;
        }

        public MareaAddress StartService(MareaAddress mareaAddress)
        {
            return serviceManager.StartService(mareaAddress.GetSubsystem(),mareaAddress.GetNodeAsIPEndPoint(),mareaAddress.GetInstance(),mareaAddress.GetService());
        }

        public MareaAddress StartService(string serviceType)
        {
            return serviceManager.StartService(serviceType);
        }

        public bool StopService(MareaAddress serviceAddress)
        {
            return serviceManager.StopService(serviceAddress);
        }

        #endregion

        #region OLD_DESIGN
        /// <summary>
        /// Gets Service.
        /// </summary>
        //public T GetService<T>(String mad)
        //{
            /*
             * if mad does not contain * 
             *   look in the services table
             *   if found
             *     return it ( can be the real local service or a proxy)
             *   else 
             *     if we know the service type
             *       if mad is local
             *         //CAUTION this proxy should be replaced by the REAL service when it starts
             *         BUILD A PROXY (type, mad)
             *         return it
             *       else
             *         BUILD A PROXY (type, mad)
             *         return it
             *     else
             *       // Difficult case because as being generic we KNOW the type
             *       DISCOVER A TYPE(type)
             *       BUILD A PROXY (type, mad)
             *       return it
             * else
             *   look in the queries table
             *   if found
             *     return it (it is a query proxy)
             *   else
             *     BUILD QUERY PROXY (type,mad)
             *     foreach local service in the table
             *       if query matches name
             *         add it as a producer of the query
             *     return the proxy    
             *   ------------------------------------------------------------------------------
             */
        //    return default(T);
        //}
        #endregion
    }
}
