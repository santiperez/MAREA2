using System;
using System.Linq;
using System.Collections.Generic;

namespace Marea
{
    public class NodeManager : IService, INodeManager
    {
        #region IService implementation

        ServiceContainer container;

        public bool Start(IServiceContainer container, ServiceAddress id)
        {
            //TODO I am the NodeManager and I can do that. Should be protected in some way.
            this.container = (ServiceContainer)container;

            return true;
        }

        public bool Stop()
        {
            return true;
        }

        #endregion

        //TODO ServiceContainer should have private operations, and IServiceContainer ONLY public operations.
        //	   Start and Stop services should be 

        #region INodeManager

        public int Add(int a, int b)
        {
            return a + b;
        }

        public MareaAddress StartService(string serviceName)
        {
            return container.StartService(serviceName);
        }

        //public MareaAddress StartService(string serviceName, string identifier, string subsystem)
        //{
        //    return container.serviceManager.StartService(serviceName, identifier, subsystem);
        //}

        public bool StopService(MareaAddress address)
        {
            return container.serviceManager.StopService(address);
        }

        public string[] GetAvailableServices()
        {
            var result =
                    from sdu in container.serviceManager.GetSDUs()
                    orderby sdu.Key
                    select sdu.Key;

            return new List<String>(result).ToArray();
        }

        public MareaAddress[] GetRunningServices()
        {
			return container.serviceManager.GetLocalServices ();
        }

        public MareaAddress[] GetKnownServices()
        {
			return container.serviceManager.services.Keys.ToArray ();
        }

		public void Shutdown() {
			container.Stop ();
		}

        #endregion

    }
	
    ///// <summary>
    ///// This is the santi-generated TestManagerProxy proxy for the producers.
    ///// </summary>
    //class NodeManagerProxy : RemoteProducer,INodeManager
    //{
    //    public NodeManagerProxy(IServiceContainer container, ServiceAddress serviceAddress, TransportAddress control)
    //        : base(control)
    //    {
    //        this.container = container;
    //        this.id = serviceAddress;
    //    }

    //    public int Add(int a, int b)
    //    {
    //        return (int)((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/Add", new object[] {a,b});
    //    }

    //    public MareaAddress StartService(string serviceName)
    //    {
    //        //TODO make it smarter
    //        return (MareaAddress)((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress,id+"/StartService", new object[] { serviceName });
    //    }

    //    //public MareaAddress StartService(string serviceName, string identifier, string subsystem)
    //    //{
    //    //    //TODO make it smarter
    //    //    return (MareaAddress)((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/StartService", new object[] { serviceName, identifier,subsystem });
    //    //}

    //    public bool StopService(MareaAddress address)
    //    {
    //        //TODO make it smarter
    //        return (bool)((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/StopService", new object[] {});
    //    }

    //    public string[] GetAvailableServices()
    //    {
    //        //TODO make it smarter
    //        return (string[])((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/GetAvailableServices", new object[] { });
    //    }

    //    public MareaAddress[] GetRunningServices()
    //    {
    //        //TODO make it smarter
    //        return (MareaAddress[])((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/GetRunningServices", new object[] { });
    //    }

    //    public MareaAddress[] GetKnownServices()
    //    {
    //        //TODO make it smarter
    //        return (MareaAddress[])((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/GetKnownServices", new object[] { });
    //    }

    //    public void Shutdown() {
    //        //TODO make it smarter
    //        ((ServiceContainer)container).RPCProtocol.CallFunction(this.ControlAddress, id + "/Shutdown", new object[] { });
    //    }
    //}

    ///// <summary>
    ///// This is the santi-generated Battery query for the consumers.
    ///// </summary>
    //class NodeManagerQuery : QueryService, INodeManager
    //{
    //    public NodeManagerQuery(IServiceContainer container, ServiceAddress serviceAddress) : base(container, serviceAddress) { }

    //    public override void AddMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        //INodeManager nodeManager = (INodeManager)service;
    //        AddMatchingServiceAddress(serviceAddress);
    //    }

    //    public override void RemoveMatchingService(ServiceAddress serviceAddress, IService service)
    //    {
    //        //INodeManager nodeManager = (INodeManager)service;
    //        RemoveMatchingServiceAddress(serviceAddress);
    //    }

    //    //TODO We need a function to get the first service or lock until one is found.

    //    public MareaAddress StartService(string serviceName)
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        return nodeManager.StartService(serviceName);
    //    }

    //    //public MareaAddress StartService(string serviceName, string identifier, string subsystem)
    //    //{
    //    //    MareaAddress mad = bindedServices.First();
    //    //    INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //    //    return nodeManager.StartService (serviceName, identifier, subsystem);
    //    //}
        
    //    public int Add(int a, int b)
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        return nodeManager.Add(a,b);
    //    }

    //    public bool StopService(MareaAddress address)
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        return nodeManager.StopService (address);
    //    }

    //    public string[] GetAvailableServices()
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        return nodeManager.GetAvailableServices ();
    //    }

    //    public MareaAddress[] GetRunningServices()
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        return nodeManager.GetRunningServices ();
    //    }

    //    public MareaAddress[] GetKnownServices()
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        return nodeManager.GetKnownServices ();
    //    }

    //    public void Shutdown() 
    //    {
    //        MareaAddress mad = bindedServices.First();
    //        INodeManager nodeManager = (INodeManager)((ServiceContainer)container).GetService(mad);
    //        nodeManager.Shutdown ();
    //    }
    //}
}

