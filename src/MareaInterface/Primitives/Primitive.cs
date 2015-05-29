using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marea
{
    public interface Primitive
    {
        String Name { get; }
        void AddSubscriber(ManageSubscriberFunc func);
        void RemoveSubscriber(ManageSubscriberFunc func);
        int GetTotalSubscriptions();
    }
}
