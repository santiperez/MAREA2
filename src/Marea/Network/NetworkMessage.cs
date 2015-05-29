using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Marea
{
    /// <summary>
    /// Enum used to set/get the state of a NetworkMessage.
    /// </summary>
    public enum NetWorkStatusCode { TransportSendingError, TransportReceivingError, OK }

    /// <summary>
    ///The communication between sublayers is done using a common interface. 
    ///Each of the sublayers send and receive a NetworkMessage entity to the upper and lower layers. 
    ///This common data structure, which contains all the necessary information used by the encoding
    ///and transport sublayers, is modified as it travels downward or upward the architecture.
    /// </summary>
    public class NetworkMessage
    {
        /// <summary>
        /// TransportAddres used to get/set the orgin or destionation of the NetworkMessage.
        /// </summary>
        public TransportAddress TransportAddress;

        /// <summary>
        /// int used to establish the max. size of the serialized object
        /// </summary>
        public const int MAX_SERIALIZABLE_OBJECT_SIZE = 1024 * 64;

        /// <summary>
        /// byte[] used to store the serialize object
        /// </summary>
        public byte[] Buffer;

        /// <summary>
        /// Total size of the serialized object
        /// </summary>
        public int Offset;

        /// <summary>
        /// object
        /// </summary>
        public object Object;

        /// <summary>
        /// NetWorkStatusCode enum used to set/get the state of a NetworkMessage.
        /// </summary>
        public NetWorkStatusCode StatusCode;

        /// <summary>
        /// Byte used to inform the container of a incoming MAREA message. It is provided by MAREA coder.
        /// </summary>
        public byte Id;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NetworkMessage()
        {
            this.Buffer = new byte[MAX_SERIALIZABLE_OBJECT_SIZE];
            this.Offset = 0;
            this.StatusCode = NetWorkStatusCode.OK;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NetworkMessage(TransportAddress ta, object _object)
            : this()
        {
            this.Object = _object;
            this.Offset = 0;
        }

        /// <summary>
        /// Sets the TrasnportAddress and the object.
        /// </summary>
        public void SetMessage(TransportAddress ta, object _object)
        {
            //Reuse TransportAddress if it's possible
            if (TransportAddress != ta)
                this.TransportAddress = ta;
            this.Object = _object;
            this.Offset = 0;
            this.StatusCode = NetWorkStatusCode.OK;
        }

        /// <summary>
        /// Sets the TrasnportAddress.
        /// </summary>
        public void SetTransportAddress(IPAddress ipAdress, int port, TransportMode transportMode)
        {
            if (TransportAddress != null)
            {
                IpTransportAddress ipa = (IpTransportAddress)TransportAddress;
                if (ipa.ipEndPoint.Address != ipAdress || ipa.ipEndPoint.Port != port || ipa.transportMode != transportMode)
                {
                    TransportAddress = new IpTransportAddress(ipAdress, port, transportMode);
                }
            }
            else
            {
                TransportAddress = new IpTransportAddress(ipAdress, port, transportMode);
            }
        }
    }
}
