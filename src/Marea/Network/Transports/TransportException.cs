using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Collections.Generic;
using System.Net.NetworkInformation;

using log4net;

namespace Marea
{
    [Serializable]
    public class TransportException : Exception
    {
        public TransportException() { }
        public TransportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class TransportSendException : TransportException
    {
        protected TransportSendException() { }
        public TransportAddress ta;
        public byte[] data;

        public TransportSendException(string message, TransportAddress ta, byte[] data, Exception innerException)
            : base(message, innerException)
        {
            this.ta = ta;
            this.data = data;
        }
    }
}
