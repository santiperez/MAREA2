#if SYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using log4net;


namespace Marea
{
    /// <summary>
    /// TCPTransport.
    /// </summary>
    public class TCPTransport : ITransport
    {
        /// <summary>
        /// Logger for the TCPTransport.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Marea.Transport.TCPTransport");

        /// <summary>
        /// Thread to accept the incoming connections.
        /// </summary>
        protected Thread th;

        /// <summary>
        /// IpTransportAddress of the local Transport.
        /// </summary>
        protected IpTransportAddress address;

        /// <summary>
        /// Socket for the incoming data.
        /// </summary>
        protected Socket inSocket;

        /// <summary>
        /// ConnectionManager used to manage the incoming Connections.
        /// </summary>
        protected ConnectionManager connections;

        /// <summary>
        /// bool to check if the transport is started.
        /// </summary>
        private volatile bool isStarted = false;

        /// <summary>
        /// ConcurrentDictionary<TransportAddress, AsyncConnection> used to manage the outgoing Connections.
        /// </summary>
        protected ConcurrentDictionary<TransportAddress, Connection> outConnections;

        /// <summary>
        /// Network.
        /// </summary>
        private Network network;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TCPTransport(Network network, IpTransportAddress t)
        {
            this.network = network;
            this.address = t;
            this.connections = new ConnectionManager();
            this.outConnections = new ConcurrentDictionary<TransportAddress, Connection>();
        }

        /// <summary>
        /// Starts the TCPTransport.
        /// </summary>
        public bool Start()
        {
            if (!isStarted)
            {
                //TODO Now uses the given port as a hint for selecting one free. It could be desirable
                //     that if the asked one is not free it halts the middleware.
                for (int i = address.ipEndPoint.Port; i < 32768; i++)
                {
                    try
                    {
                        inSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        inSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                        address.ipEndPoint = new IPEndPoint(address.ipEndPoint.Address, i);
                        inSocket.Bind(address.ipEndPoint);
                        inSocket.Listen(20);
                    }
                    catch (SocketException e)
                    {
                        if (e.ErrorCode == 10048)
                        {
                            //Nothing. Try with next port.
                        }
                        else
                        {
                            log.Error(e.ToString());
                            throw new TransportException("Error in TcpTransport while tring to set up the listening socket", e);
                        }
                        continue;
                    }
                    break;
                }

                address.ipEndPoint.Port = ((IPEndPoint)inSocket.LocalEndPoint).Port;
                log.Info("Local Port = " + address.ipEndPoint.Port);

                ThreadStart ts = new ThreadStart(this.Run);
                th = new Thread(ts);
                isStarted = true;
                th.Start();
            }
            return isStarted;
        }

        /// <summary>
        /// Accepts the incoming Connections. 
        /// </summary>
        private void Run()
        {
            Socket aux;
            while (isStarted)
            {
                if (log.IsDebugEnabled)
                    log.Debug("Waiting for TCP Connections...");

                try
                {
                    aux = inSocket.Accept();
                }
                catch (Exception e)
                {
                    if (e is SocketException)
                    {
                        SocketException se = (SocketException)e;
                        if (se.ErrorCode == 10004)
                        {
                            // Accept() cancelled.
                            continue;
                        }
                    }
                    if (e is ObjectDisposedException)
                    {
                        //ObjectDisposedException should be ignored: happens when the socket is closed and disposed.
                        continue;
                    }
                    isStarted = false;
                    log.Error(e.ToString());
                    throw new TransportException("Error in TcpTransport while receiving", e);
                }

                try
                {
                    Connection c = new Connection(network,aux, connections.closeConnectionHandler);
                    connections.AddInConnection(c);
                    Thread t = new Thread(new ThreadStart(c.Run));
                    t.Start();
                }
                catch (ThreadAbortException)
                {
                    //It's safe to ignore this :)
                }

            }
        }

        /// <summary>
        /// Gets a Connection by the given TransportAddress.  
        /// </summary>
        public Connection GetChannel(TransportAddress transportAddress)
        {
            Connection outConnection = null;
            if (!outConnections.TryGetValue(transportAddress, out outConnection))
            {
                Socket outSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                outSocket.Connect(((IpTransportAddress)transportAddress).ipEndPoint);
                outConnection = new Connection(outSocket);
                outConnections.TryAdd(transportAddress, outConnection);
            }
            return outConnection;
        }

        /// <summary>
        /// Stops the TCPTransport. 
        /// </summary>
        public bool Stop()
        {
            if (isStarted)
            {
                isStarted = false;
                inSocket.Close();
                connections.CloseConnections();
            }
            return !isStarted;
        }

        /// <summary>
        /// Gets the local TransportAddress.  
        /// </summary>
        public TransportAddress GetTransportAddress()
        {
            return address.Clone();
        }

    }
}
#endif