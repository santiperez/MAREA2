using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marea
{
    public class ServiceAddress : MareaAddress
    {
        //TODO THIS SHOULD BE NOT CREATED OUTSIDE nor MODIFIED...
        public ServiceAddress(MareaAddress mad) : base(mad) { }

        public ServiceAddress(string mad) : base(mad) { }
    }
}
