using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// Creates the network Lanes.
    /// </summary>
    public class Router
    {
        /// <summary>
        /// Network.
        /// </summary>
        private Network network;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Router(Network network)
        {
            this.network = network;
        }

        /// <summary>
        /// Creates a output network Lane from the given TransportAddress.
        /// </summary>
        public Lane GetOutputLane(TransportAddress ta)
        {
            Lane lane = new Lane();
            lane.NetworkHandler = MareaCoder.Send;
            bool isBroadcast=ta.Equals(network.Broadcast);

            if (isBroadcast || !ta.isReliable())
            {
                lane.NetworkHandler += network.UdpTransport.Send;
            }
            else
            {
                lane.NetworkHandler += network.TcpTransport.GetChannel(ta).Send;
            }

            return lane;
        }

        /// <summary>
        /// Creates a input network Lane from the given TransportAddress.
        public Lane GetInputLane(TransportAddress ta)
        {
            Lane lane = new Lane();
            lane.NetworkHandler = MareaCoder.Receive;
            lane.NetworkHandler += network.container.Receive;

            return lane;
        }

    }
}
