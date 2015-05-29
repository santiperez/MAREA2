using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class TypeAttribute : Attribute
    {
        public String Text;

        public TypeAttribute(String text) { Text = text; }
    }
}
