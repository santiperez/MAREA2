using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// This interface marks a service as internal. 
    /// Be careful when looking throught the code, because it is looked with: t.GetInterface("Marea.IProxyService")
    /// and "Find All References" does not found it :)
    /// </summary>
    public interface IProxyService : IService
    {
    }
}