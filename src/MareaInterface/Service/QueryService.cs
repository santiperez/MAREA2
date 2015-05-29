using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
	//TODO Tiene que ser publico?
    public abstract class QueryService
    {
        public IServiceContainer container;
        public ServiceAddress id;
        protected List<ServiceAddress> bindedServices;

        public QueryService(IServiceContainer container, ServiceAddress id)
        {
            this.container = container;
            this.id = id;
            this.bindedServices = new List<ServiceAddress>();
        }

        protected void AddMatchingServiceAddress(ServiceAddress serviceAddress)
        {
            lock (bindedServices)
            {
                if(!bindedServices.Contains(serviceAddress))
                    bindedServices.Add(serviceAddress);
            }
        }

        protected bool RemoveMatchingServiceAddress(ServiceAddress serviceAddress)
        {
            lock (bindedServices)
            {
                return bindedServices.Remove(serviceAddress);
            }
        }

        public void AddOrRemoveSubscriber(Primitive primitive, bool isAdding, ServiceAddress mareaServiceAddress, object notifyFunc, int totalSubscribers)
        {
            if (isAdding)
            {
                AddMatchingServices(container.GetServicesFromQuery(id));
            }
            else
            {
                RemoveMatchingServices(container.GetServicesFromQuery(id));
            }

        }

        public void AddMatchingServices(Dictionary<MareaAddress, IService> matchingServices)
        {
            foreach (KeyValuePair<MareaAddress, IService> kvpService in matchingServices)
            {
                AddMatchingService(new ServiceAddress(kvpService.Key), kvpService.Value);
            }
        }


        public void RemoveMatchingServices(Dictionary<MareaAddress, IService> matchingServices)
        {
            foreach (KeyValuePair<MareaAddress, IService> kvpService in matchingServices)
            {
                RemoveMatchingService(new ServiceAddress(kvpService.Key), kvpService.Value);
            }
        }
        
        public abstract void AddMatchingService(ServiceAddress serviceAddress,IService service);

        public abstract void RemoveMatchingService(ServiceAddress serviceAddress, IService service);
    }
}
