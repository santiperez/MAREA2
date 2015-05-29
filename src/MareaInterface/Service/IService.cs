using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marea
{
    public interface IService
    {
        bool Start(IServiceContainer container, ServiceAddress id);
        bool Stop();
    }
}
