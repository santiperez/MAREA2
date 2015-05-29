
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    public delegate void ManageSubscriberFunc(Primitive variable, bool isAdding, ServiceAddress id, object notifyFunc, int totalSubscribers);
}
