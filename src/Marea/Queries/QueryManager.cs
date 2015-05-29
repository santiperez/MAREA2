using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Marea
{
    public class QueryManager
    {
        /// <summary>
        /// Service manager
        /// <summary>
        private ServiceContainer container;

        /// <summary>
        /// Service manager
        /// <summary>
        private Dictionary<MareaAddress, QueryService> queries;

        /// <summary>
        /// Constructor
        /// <summary>
        public QueryManager(ServiceContainer container)
        {
            this.container = container;
            this.queries = new Dictionary<MareaAddress, QueryService>();
        }

        /// <summary>
        /// Gets a dictionary of all the services of the current node which match with the given query ServiceAddress
        /// <summary>
        public Dictionary<MareaAddress, IService> GetServicesFromQuery(MareaAddress queryServiceAddress)
        {
            Dictionary<MareaAddress, IService> servicesDict;

            lock (container.serviceManager.services)
            {
                servicesDict =container.serviceManager.services.Where(serviceKVP=>MareaAddressMatchesWithQueryAddress(serviceKVP.Key,queryServiceAddress)).ToDictionary(serviceKVP => serviceKVP.Key, serviceKVP => serviceKVP.Value);
            }

            return servicesDict;
        }

        /// <summary>
        /// Gets a dictionary of all the services that are real services (not proxies) of the current node which match with the given query ServiceAddress
        /// <summary>
        public Dictionary<MareaAddress, IService> GetRealServicesFromQuery(MareaAddress queryServiceAddress)
        {
            Dictionary<MareaAddress, IService> servicesDict;

            lock (container.serviceManager.services)
            {
                servicesDict = container.serviceManager.services.Where(serviceKVP => MareaAddressMatchesWithQueryAddress(serviceKVP.Key, queryServiceAddress) && !typeof(RemoteProducer).IsAssignableFrom(serviceKVP.Value.GetType())).ToDictionary(serviceKVP => serviceKVP.Key, serviceKVP => serviceKVP.Value);
            }

            return servicesDict;
        }

        /// <summary>
        /// Gets a dictionary of all the query ServiceAddress of the current node which match with the given ServiceAddress
        /// <summary>
        public Dictionary<MareaAddress, QueryService> GetQueriesFromService(MareaAddress serviceAddress)
        {
            Dictionary<MareaAddress, QueryService> queriesDict;

            lock (queries)
            {
                queriesDict = queries.Where(queryKVP=>MareaAddressMatchesWithQueryAddress(serviceAddress, queryKVP.Key)).ToDictionary(queryKVP => queryKVP.Key, queryKVP => queryKVP.Value);
            }

            return queriesDict;
        }

        /// <summary>
        /// Check if a service MareaAddress matches with a given QueryAddress
        /// <summary>
        public static bool MareaAddressMatchesWithQueryAddress(MareaAddress serviceAddress, MareaAddress queryAdress)
        {
            string pattern = GetRegexPatternFromMareaAddress(queryAdress);
            return Regex.Match(serviceAddress.ToString(), pattern).Success;
        }

        /// <summary>
        /// Creates a regex expression from a query MareaAddress
        /// <summary>
        private static string GetRegexPatternFromMareaAddress(MareaAddress serviceAddress)
        {
            string anySubstring = @"(/.*?)";
            string result = "";

            //Subsystem
            if (serviceAddress.GetSubsystem() == "*")
                result += anySubstring;
            else
                result += @"(/\b" + serviceAddress.GetSubsystem() + @"\b)";

            //Node
            if (serviceAddress.GetNode() == "*")
                result += anySubstring;
            else
                result += @"(/\b" + serviceAddress.GetNode() + @"\b)";

            //Instance
            if (serviceAddress.GetInstance() == "*")
                result += anySubstring;
            else
                result += @"(/\b" + serviceAddress.GetInstance() + @"\b)";

            //Service
            if (serviceAddress.GetService() == "*")
                result += anySubstring;
            else
                result += @"(/\b" + serviceAddress.GetService() + @"\b)";

            //Service
            string primitive = serviceAddress.GetPrimitive();
            if (primitive != null)
            {
                if (primitive == "*")
                    result += anySubstring;
                else
                    result += @"(/\b" + primitive + @"\b)";
            }


            return result;
        }

        /// <summary>
        /// Creates a query service by the given serviceAddress
        /// <summary>
        private QueryService CreateQuery(MareaAddress serviceAddress)
        {
            QueryService queryService = (QueryService)Activator.CreateInstance(container.serviceManager.GetImplementation(serviceAddress.GetService()).QueryType, container, new ServiceAddress(serviceAddress.GetServiceAddress()));
            return queryService;
        }

        /// <summary>
        /// Gets query service by the given serviceAddress if exists in other case it is created.
        /// <summary>
        public QueryService GetQuery(MareaAddress serviceAddress)
        {
            QueryService queryService = null;

            if (serviceAddress.isQueryAddress())
            {
                lock (this.queries)
                {
                    if (!queries.TryGetValue(serviceAddress, out queryService))
                    {
                        queryService=CreateQuery(serviceAddress);
                        queries.Add(serviceAddress, queryService);
                    }
                }
            }

            return queryService;
        }

        /// <summary>
        /// Subscribes the given service to all the existent queries that match with the given ServiceAddress
        /// <summary>
        public void SubscribeServiceToExistentQueries(MareaAddress serviceAddress, IService service)
        {
            Dictionary<MareaAddress, QueryService> queriesDict = GetQueriesFromService(serviceAddress);

            foreach (QueryService query in queriesDict.Values)
            {
                //if(!query.bindedServices.Contains(serviceAddress))
                query.AddMatchingService(new ServiceAddress(serviceAddress), service);
            }
        }


        /// <summary>
        /// Unsubscribes the given service to all the existent queries that match with the given ServiceAddress
        /// <summary>
        public void UnsubscribeServiceFromExistentQueries(MareaAddress serviceAddress, IService service)
        {
            Dictionary<MareaAddress, QueryService> queriesDict = GetQueriesFromService(serviceAddress);

            foreach (QueryService query in queriesDict.Values)
            {
                query.RemoveMatchingService(new ServiceAddress(serviceAddress), service);
            }
        }

        /// <summary>
        /// Subscribes all the producers to the query given by a MareaAddress.
        /// Returns the total numbers of producers that have been subscribed to the query.
        /// <summary>
        public int SubscribeProducersToQuery(MareaAddress queryMareaAddress)
        {
            var producers = GetServicesFromQuery(queryMareaAddress);

            foreach (KeyValuePair<MareaAddress, IService> remoteProducer in producers)
            {
                SubscribeServiceToExistentQueries(remoteProducer.Key, remoteProducer.Value);
            }
            return producers.Count;
        }
    }
}