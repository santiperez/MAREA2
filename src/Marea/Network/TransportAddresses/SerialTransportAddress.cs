using System;

namespace Marea
{
    /// <summary>
    /// Transport Address for the Serial protocol
    /// </summary>
    [Serializable]
    public class SerialTransportAddress : TransportAddress
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialTransportAddress() { }

        /// <summary>
        /// string used to reprent remote serial port.
        /// </summary>
        public string serialport;

        /// <summary>
        /// bool used to check if ACK should be used.
        /// </summary>
        public bool forceACK;

        /// <summary>
        /// Creates a new TransportAddress instance.
        /// </summary>
        public SerialTransportAddress(string port, bool forceACK)
        {
            transportMode = TransportMode.Serial;
            serialport = port;
            this.forceACK = forceACK;
        }

        /// <summary>
        /// Checks if this object equals to an other object.
        /// </summary>
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;
            SerialTransportAddress tm = (SerialTransportAddress)obj;
            return transportMode.Equals(tm.transportMode) && serialport.Equals(tm.serialport);
        }

        /// <summary>
        /// Gets an unique identifier of the TransportAddress. 
        /// </summary>
        public override int GetHashCode()
        {
            return serialport.GetHashCode() + transportMode.GetHashCode();
        }

        /// <summary>
        /// Gets the TransportAddress in string format(ip:port/TransportMode).
        /// </summary>
        public override string ToString()
        {
            return serialport + "/" + transportMode.ToString();
        }

        /// <summary>
        /// Method to get the IPEndpoint of the TransportAddress in a string format (ip:port).
        /// </summary>
        public override string GetAddress()
        {
            return serialport;
        }

        /// <summary>
        /// Checks if a TransportAddress belongs to the same network as the given one.
        /// </summary>
        //public override bool IsSameNetwork(TransportAddress ta)
        //{
        //    if (ta == null || GetType() != ta.GetType()) return false;
        //    SerialTransportAddress t = (SerialTransportAddress)ta;
        //    return (serialport == t.serialport);
        //}

        /// <summary>
        /// Creates a deep copy of the TransportAddress.
        /// </summary>
        public override TransportAddress Clone()
        {
            return new SerialTransportAddress((String)serialport.Clone(), forceACK);
        }

        /// <summary>
        /// Checks if an adress is broadcast.
        /// </summary>
        //public override bool isBroadcast()
        //{
        //    return Transport.Broadcast.Equals((TransportAddress)this);
        //}

        /// <summary>
        /// Checks if an adress is reliable.
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
