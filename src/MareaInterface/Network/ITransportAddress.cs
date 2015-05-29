using System;

namespace Marea
{
    /// <summary>
    /// Enumerator of all the available Transport Modes
    /// </summary>
    public enum TransportMode
    {
        /// <summary>
        /// UDP Transport
        /// </summary>
        UDP,
        /// <summary>
        /// TCP Transport
        /// </summary>        
        TCP,
        /// <summary>
        /// Serial Port Transport
        /// </summary>
        Serial
    };

    /// <summary>
    /// Defines a Marea generic Transport Address
    /// </summary>
    [Serializable]
    public abstract class TransportAddress
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TransportAddress() { }
        /// <summary>
        /// The transport mode of this Transport Address.
        /// </summary>
        public TransportMode transportMode;

        /// <summary>
        /// Create a deep copy of the Transport Address.
        /// </summary>
        abstract public TransportAddress Clone();

        /// <summary>
        /// Create a deep copy of the Transport Address.
        /// </summary>
        abstract public string GetAddress();

        /// <summary>
        /// Method to check if a TransportAddress belongs to the same network.
        /// </summary>
        //abstract public bool IsSameNetwork(TransportAddress ta);

        /// <summary>
        /// Method to check if a TransportAddress is broadcast.
        /// </summary>
        //abstract public bool isBroadcast();

        /// <summary>
        /// Method to check if a TransportAddress is reliable.
        /// </summary>
        abstract public bool isReliable();
    }
}
