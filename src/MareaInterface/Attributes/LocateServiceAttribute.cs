using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class LocateServiceAttribute : Attribute
    {
        public String Text;

        public LocateServiceAttribute() { }

        public LocateServiceAttribute(String text) { Text = text; }
    }
}
