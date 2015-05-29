using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class ServiceImplementation
    {
        public Type Type;
        public Type QueryType;
        public ConsumerServiceInfo[] ConsumerServices;
        public Dictionary<String, FieldInfo> PrimitiveFields;

        public ServiceImplementation(Type type, Type queryType, ConsumerServiceInfo[] consumerServices,Dictionary<String,FieldInfo> primitiveFields)
        {
            this.Type = type;
            this.QueryType = queryType;
            this.ConsumerServices=consumerServices;
            this.PrimitiveFields = primitiveFields;
        }
    }

    public class ConsumerServiceInfo
    {
        public String ServiceName;
        public FieldInfo Field;
        public String Attribute;
        public Type ProxyType;
        public Type QueryType;
    }
}
