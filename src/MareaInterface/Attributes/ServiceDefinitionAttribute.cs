using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class ServiceDefinitionAttribute : Attribute
    {
        public String Text;

        public ServiceDefinitionAttribute(String text) { Text = text; }
    }
}
