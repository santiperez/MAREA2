using System;
using System.Net;

namespace Marea
{
    /// <summary>
    /// IPTransportAddress.
    /// </summary>
    [Serializable]
    public class IpTransportAddress : TransportAddress
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IpTransportAddress() { }

        /// <summary>
        /// Remote IPEndpoint.
        /// </summary>
        public IPEndPoint ipEndPoint;

        /// <summary>
        /// Creates new TransportAddress instance.
        /// </summary>
        public IpTransportAddress(IPAddress a, int port, TransportMode mode)
        {
            ipEndPoint = new IPEndPoint(a, port);
            transportMode = mode;
        }

        /// <summary>
        /// Checks if this object equals to an other object.
        /// </summary>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;
            IpTransportAddress tm = (IpTransportAddress)obj;
            return (tm.transportMode == transportMode) &&
                (tm.ipEndPoint.Address.ToString() == ipEndPoint.Address.ToString()) &&
                (tm.ipEndPoint.Port == ipEndPoint.Port);
        }

        /// <summary>
        /// Gets an unique identifier of the TransportAddress. 
        /// </summary>
        public override int GetHashCode()
        {
            return ipEndPoint.GetHashCode() + transportMode.GetHashCode();
        }

        /// <summary>
        /// Gets the TransportAddress in string format(ip:port/TransportMode).
        /// </summary>
        public override string ToString()
        {
            return ipEndPoint.ToString() + "/" + transportMode.ToString();
        }

        /// <summary>
        /// Gets the IPEndpoint of the TransportAddress in a string format (ip:port).
        /// </summary>
        public override string GetAddress()
        {
            return ipEndPoint.Address.ToString() + ":" + ipEndPoint.Port;
        }

        /// <summary>
        /// Checks if a TransportAddress belongs to the same network as the given one.
        /// </summary>
        //public override bool IsSameNetwork(TransportAddress ta)
        //{
        //    if (ta == null || GetType() != ta.GetType()) return false;
        //    IpTransportAddress t = (IpTransportAddress)ta;
        //    return Transport.IsSameNetwork(ipEndPoint.Address, t.ipEndPoint.Address);
        //}

        /// <summary>
        /// Creates a deep copy of the TransportAddress.
        /// </summary>
        public override TransportAddress Clone()
        {
            return new IpTransportAddress(new IPAddress(ipEndPoint.Address.GetAddressBytes()), ipEndPoint.Port, transportMode);
        }

        /// <summary>
        /// Checks if an adress is broadcast.
        /// </summary>
        //public override bool isBroadcast()
        //{
        //    return Transport.Broadcast.Equals((TransportAddress)this);
        //}

        /// <summary>
        /// Checks if a transport mode is reliable.
        /// </summary>
        public override bool isReliable()
        {
            TransportAddress ta = (this);
            if (ta.transportMode == TransportMode.UDP)
                return false;
            else if (ta.transportMode == TransportMode.TCP)
                return true;

            return false;
        }
    }
}
