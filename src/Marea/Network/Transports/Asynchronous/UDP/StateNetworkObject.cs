#if ASYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Marea
{
    /// <summary>
    /// This class is used to pass the information to the callbacks of sockets operations.
    /// </summary>
    public class StateNetworkObject
    {
        /// <summary>
        /// Socket
        /// </summary>
        public Socket WorkSocket = null;

        /// <summary>
        /// NetworkMessage
        /// </summary>
        public NetworkMessage NetworkMessage = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public StateNetworkObject(Socket socket)
        {
            this.WorkSocket = socket;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public void setNetworkMessage(NetworkMessage networkMessage)
        {
            this.NetworkMessage = networkMessage;
        }
    }
}
#endif
