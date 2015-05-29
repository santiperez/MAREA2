using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Marea;

namespace Altimeter
{
    [ServiceDefinition("This service represents an altimeter")]
    public interface IAltimeter
    {
        [Description("Altitude above sea level")]
        [Unit("meters")]
        [Publish]
        Variable<int> altitude { get; }

        void Reset(bool b);
    }
}
