using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// Multicast delegate used to mange Lanes.
    /// </summary>
    public delegate void NetworkProcess(NetworkMessage networkMessage);

    /// <summary>
    /// Lane: set of references to the bindings establish between the different network architecture 
    /// elements (encoder and transports) used at a particularmoment.
    /// </summary>
    public class Lane
    {
        /// <summary>
        /// NetworkProcess multicast delegate used manage the Lane.
        /// </summary>
        public NetworkProcess NetworkHandler;
    }
}

