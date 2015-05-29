#if ASYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using log4net;
using System.Collections.Concurrent;

namespace Marea
{
    /// <summary>
    /// TCPAsyncTransport.
    /// </summary>
    public class TCPAsyncTransport : ITransport
    {
        /// <summary>
        /// Logger for the TCPAsyncTransport.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Marea.Transport.TCPAsyncTransport");

        /// <summary>
        /// Thread used to accept and create the incoming AsyncConnections.
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// Socket used to listen the incoming AsyncConnections.
        /// </summary>
        protected Socket inSocket;

        /// <summary>
        /// ConnectionManager used to manage the incoming AsyncConnections.
        /// </summary>
        protected ConnectionManager connections;

        /// <summary>
        /// IpTransportAddress of the local Transport.
        /// </summary>
        protected IpTransportAddress address;

        /// <summary>
        /// bool to check if the transport is started.
        /// </summary>
        protected volatile bool isStarted = false;

        /// <summary>
        /// AsyncCallback delegated used to accept AsyncConnections.
        /// </summary>
        protected AsyncCallback acceptCallback;

        /// <summary>
        /// AsyncCallback delegated used to create AsyncConnections.
        /// </summary>
        protected AsyncCallback connectCallback;

        /// <summary>
        /// ConcurrentDictionary<TransportAddress, AsyncConnection> used to manage the outgoing AsyncConnections.
        /// </summary>
        protected ConcurrentDictionary<TransportAddress, AsyncConnection> outSockets;

        /// <summary>
        /// Network.
        /// </summary>
        private Network network;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TCPAsyncTransport(Network network, IpTransportAddress t)
        {
            connections = new ConnectionManager();
            address = t;
            this.network = network;
            this.acceptCallback = new AsyncCallback(AcceptCallback);
            this.outSockets = new ConcurrentDictionary<TransportAddress, AsyncConnection>();
            this.connectCallback = new AsyncCallback(ConnectCallback);
        }

        /// <summary>
        /// Starts a TCPAsyncTransport.
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
                        isStarted = true;

                        thread = new Thread(new ThreadStart(AcceptConnections));
                        thread.Start();
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
                            throw new TransportException("Error in TcpAsyncTransport while tring to set up the listening socket", e);
                        }
                        continue;
                    }
                    break;
                }

                address.ipEndPoint.Port = ((IPEndPoint)inSocket.LocalEndPoint).Port;
                log.Info("Local Port = " + address.ipEndPoint.Port);
            }
            return isStarted;
        }

        /// <summary>
        /// Accepts the incoming AsyncConnections. 
        /// </summary>
        private void AcceptConnections()
        {
            try
            {
                inSocket.BeginAccept(AcceptCallback, inSocket);
            }

            catch (ThreadAbortException)
            {
                //Ignore
            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while accepting a connection in TCPAsyncTransport", ex);
                throw new TransportException("Socket exception while accepting a connection in TCPAsyncTransport", ex);

            }
            catch (Exception ex)
            {
                log.Error("Exception while accepting a connection in TCPAsyncTransport", ex);
                throw new TransportException("Exception while accepting a connection in TCPAsyncTransport", ex);
            }
        }

        /// <summary>
        /// Callback method to accept the incoming AsyncConnections.  
        /// </summary>
        private void AcceptCallback(IAsyncResult iar)
        {
            try
            {
                Socket listener = (Socket)iar.AsyncState;

                Socket handler = listener.EndAccept(iar);
                handler.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

                AsyncConnection c = new AsyncConnection(network, handler, connections.closeConnectionHandler);
                connections.AddInConnection(c);
                AcceptConnections();

            }
            catch (ObjectDisposedException)
            {
                //ObjectDisposedException should be ignored: happens when the socket is closed and disposed.
            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while accepting (Callback) a connection in TCPAsyncTransport", ex);
                throw new TransportException("Socket exception while accepting (Callback) a connection in TCPAsyncTransport", ex);

            }
            catch (Exception ex)
            {
                log.Error("Exception while accepting (Callback) a connection in TCPAsyncTransport", ex);
                throw new TransportException("Exception while accepting (Callback) a connection in TCPAsyncTransport", ex);
            }
        }

        /// <summary>
        /// Gets an AsyncConnection by the given TransportAddress.  
        /// </summary>
        public AsyncConnection GetChannel(TransportAddress transportAddress)
        {

            AsyncConnection outConnection = null;
            if (!outSockets.TryGetValue(transportAddress, out outConnection))
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                outConnection = new AsyncConnection(socket);
                outSockets.TryAdd(transportAddress, outConnection);
                outConnection.socket.BeginConnect(((IpTransportAddress)transportAddress).ipEndPoint, connectCallback, outConnection.socket);
            }
            return outConnection;
        }

        /// <summary>
        /// Callback method to establish an AsyncConnection.  
        /// </summary>
        private void ConnectCallback(IAsyncResult iar)
        {
            try
            {
                Socket socket = (Socket)iar.AsyncState;

                //Complete the connection
                socket.EndConnect(iar);

            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while connecting (Callback) in TCPAsyncTransport", ex);
                throw new TransportException("Socket exception while connecting (Callback)  in TCPAsyncTransport", ex);

            }
            catch (Exception ex)
            {
                log.Error("Exception while connecting (Callback)  in TCPAsyncTransport", ex);
                throw new TransportException("Socket exception while connecting (Callback) in TCPAsyncTransport", ex);
            }
        }

        /// <summary>
        /// Stops the TCPAsyncTransport. 
        /// </summary>
        public bool Stop()
        {
            try
            {
                if (isStarted)
                {
                    isStarted = false;
                    thread.Abort();
                    inSocket.Close();
                    connections.CloseConnections();
                }
                return !isStarted;
            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while stoping TCPAsyncTransport", ex);
                throw new TransportException("Socket exception while stoping in TCPAsyncTransport", ex);

            }
            catch (Exception ex)
            {
                log.Error("Exception while stoping in TCPAsyncransport", ex);
                throw new TransportException("Socket exception while stoping in TCPAsyncTransport", ex);
            }
        }

        /// <summary>
        /// Gets the local TransportAddress.  
        /// </summary>
        public TransportAddress GetTransportAddress()
        {
            return address;
        }

    }
}
#endif