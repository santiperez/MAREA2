using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public class DescriptionAttribute : Attribute
    {
        public String Text;

        public DescriptionAttribute(String text) { Text = text; }
    }
}
