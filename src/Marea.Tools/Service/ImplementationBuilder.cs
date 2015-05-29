using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Marea
{
    public class ImplementationBuilder
    {
        public ServiceImplementation CreateImplemementation(Type t)
        {
            List<ConsumerServiceInfo> consumerServices = new List<ConsumerServiceInfo>();
            Dictionary<String, FieldInfo> primitiveFieldInfos = new Dictionary<String, FieldInfo>();
            object[] objs;
            Type queryType = null;

            //Get the QueryService
            Type idu = t.GetInterfaces().Where(type => typeof(Service) != type && typeof(IService) != type).FirstOrDefault();

            //TODO: throw exception if the query service is not found (change FirstOrDefault by First)
            if (idu != null)
                queryType = AssembliesManager.Instance.GetAllTypes().Where(type => idu.IsAssignableFrom(type.Value) && !type.Value.IsInterface && typeof(QueryService).IsAssignableFrom(type.Value)).ToArray().FirstOrDefault().Value;

            FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                objs = field.GetCustomAttributes(true);
                foreach (object o in objs)
                {
                    if (o.GetType().FullName == "Marea.LocateServiceAttribute")
                    {
                        String txt = ((LocateServiceAttribute)o).Text;
                        if (txt != null)
                        {
                            ConsumerServiceInfo c = new ConsumerServiceInfo();
                            
                            MareaAddress locateMad = new MareaAddress(txt);
                            Type locateType=null;
                            string serviceTypeName=locateMad.GetService();
                            locateType=AssembliesManager.Instance.GetTypeFromFullName(serviceTypeName);
                            if(locateType==null)
                            {
                                System.Console.WriteLine("Service type "+ serviceTypeName +" not found");
                                System.Console.WriteLine("Check LocateService tag in " + t.FullName + " class");
                                System.Console.WriteLine("");
                                throw new NotImplementedException();
                            }

                            if (locateType.IsInterface)
                            {
                                System.Console.WriteLine("The LocateService tag does not offer support for interface service types");
                                System.Console.WriteLine("Check LocateService tag in "+ t.FullName+ " class");
                                System.Console.WriteLine("");
                                throw new NotImplementedException();
                            }

                            c.Attribute = txt;
                            c.Field = field;
                            c.ServiceName = field.Name;

                            //GET Proxy Type
                            //First get the IDU of the producer(i.e. IBattery) referenced and used by the consumer (i.e. IBatteryManager)
                            Type iduType = field.FieldType;
                            if (iduType != null)
                            {
                                //Get all the types which implement the IDU (i.e. IBattery). Select those that are not the IDU (i.e. IBattery) by itself and are inherited from RemoteProducer
                                KeyValuePair<string, Type>[] proxies = AssembliesManager.Instance.GetAllTypes().Where(type => iduType.IsAssignableFrom(type.Value) && !type.Value.IsInterface && typeof(RemoteProducer).IsAssignableFrom(type.Value)).ToArray();
                                if (proxies.Length > 0)
                                    c.ProxyType = proxies[0].Value;
                                else
                                {
                                    System.Console.WriteLine("Loading service implementation type: "+t.FullName);
                                    System.Console.WriteLine("");
                                    System.Console.WriteLine("* Proxy type " + c.Field.FieldType.FullName + " not found");
                                    System.Console.WriteLine("");

                                    System.Console.WriteLine("* Candidates-> Do not inherit from RemoteProducer");
                                    var proxyCandidates =AssembliesManager.Instance.GetAllTypes().Where(type => iduType.IsAssignableFrom(type.Value) && !type.Value.IsInterface).Select(kvp=>kvp.Key).ToArray();
                                    foreach (String proxyTypeName in proxyCandidates)
                                        System.Console.WriteLine("** "+proxyTypeName);
                                    System.Console.WriteLine("");

                                    System.Console.WriteLine("* Candidates-> Do not inherit from IDU "+iduType.FullName);
                                    var proxyCandidates2 = AssembliesManager.Instance.GetAllTypes().Where(type => typeof(RemoteProducer).IsAssignableFrom(type.Value)).Select(kvp => kvp.Key).ToArray();
                                    foreach (String proxyTypeName in proxyCandidates2)
                                        System.Console.WriteLine("** " + proxyTypeName);
                                    System.Console.WriteLine("");

                                    System.Console.WriteLine("Otherwise it is possible that the proxy for the " + c.Field.FieldType.FullName + " is not really implemented.");

                                    throw new NotImplementedException();
                                }
                            }
                            consumerServices.Add(c);
                        }
                    }
                }

                if (typeof(Primitive).IsAssignableFrom(field.FieldType))
                {
                    string name = Regex.Match(field.Name, "<(.*)>k__BackingField").Groups[1].Value;
                    primitiveFieldInfos.Add(name, field);
                }
            }
            return new ServiceImplementation(t, queryType, consumerServices.ToArray(), primitiveFieldInfos);
        }
    }
}
