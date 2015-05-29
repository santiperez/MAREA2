using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class UnitAttribute : Attribute
    {
        public String Text;

        public UnitAttribute(String text) { Text = text; }
    }
}
