#if ASYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using log4net;

namespace Marea
{

    /// <summary>
    /// UDPAsyncTransport.
    /// </summary>
    public class UDPAsyncTransport
    {
        /// <summary>
        /// Logger for the UDPAsyncTransport.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Marea.Transport.UDPAsyncTransport");

        /// <summary>
        /// StateNetworkObject for the incoming data. Contains a Socket and a byte[].
        /// </summary>
        protected StateNetworkObject inStateNetworkObject;

        /// <summary>
        /// Socket for the outgoing data.
        /// </summary>
        protected Socket outSocket;

        /// <summary>
        /// Local Transport Address
        /// </summary>
        protected IpTransportAddress address;

        /// <summary>
        /// Local port.
        /// </summary>
        protected int localPort;

        /// <summary>
        /// Boolean to check if the UDPAsyncTransport is started.
        /// </summary>
        protected Boolean isStarted = false;

        /// <summary>
        /// AsyncCallback to send data.
        /// </summary>
        protected AsyncCallback sendCallback;

        /// <summary>
        /// AsyncCallback to send data.
        /// </summary>
        protected AsyncCallback receiveCallback;

        /// <summary>
        /// An empty IPEndPoint (0.0.0.0:0).
        /// </summary>
        protected static IPEndPoint emptyIPEndPoint;

        /// <summary>
        /// Network instance.
        /// </summary>
        private Network network;

        /// <summary>
        /// LockFreeQueue instance.
        /// </summary>
        private LockFreeQueue queue;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UDPAsyncTransport(Network network, IpTransportAddress t)
        {
            this.address = t;
            this.sendCallback = new AsyncCallback(SendCallback);
            this.receiveCallback = new AsyncCallback(ReceiveCallback);
            this.network = network;
            this.queue = LockFreeQueue.GetInstance();
            emptyIPEndPoint = new IPEndPoint(0, 0);
        }

        /// <summary>
        /// Starts the UDPAsyncTransport. Binds InStateNetworkObject Socket in the IPEndpoint given by an IpTransportAddress.  
        /// </summary>
        public void Start()
        {

            inStateNetworkObject = new StateNetworkObject(new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp));
            //pool
            inStateNetworkObject.setNetworkMessage(queue.Dequeue());

            outSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            inStateNetworkObject.WorkSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            outSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

            inStateNetworkObject.WorkSocket.MulticastLoopback = false;
            outSocket.MulticastLoopback = false;

            inStateNetworkObject.WorkSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
            outSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);

            IPAddress local = network.GetLocalIP(address.ipEndPoint.Address);
            outSocket.Bind(new IPEndPoint(local, 0));

            if (network.IsMulticastIP(address.ipEndPoint.Address))
            {
                IpTransportAddress toBind = (IpTransportAddress)address.Clone();
                toBind.ipEndPoint.Address = local;
                inStateNetworkObject.WorkSocket.Bind(toBind.ipEndPoint);
                outSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, toBind.ipEndPoint.Address.GetAddressBytes());
                inStateNetworkObject.WorkSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                outSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);

                MulticastOption multicastOption = new MulticastOption(address.ipEndPoint.Address, toBind.ipEndPoint.Address);
                inStateNetworkObject.WorkSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);
                outSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOption);

                log.Debug("Multicast membership to " + address.ipEndPoint.ToString() +
                    " for local incoming address " + toBind.ipEndPoint.ToString() + " created");
            }
            else if (address.ipEndPoint.Port == network.broadcastPort)
            {
                outSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                inStateNetworkObject.WorkSocket.Bind(new IPEndPoint(local, network.broadcastPort));

                log.Info("Broadcast Address = " + address.ToString());
            }
            else
            {
                inStateNetworkObject.WorkSocket.Bind(new IPEndPoint(address.ipEndPoint.Address, address.ipEndPoint.Port));
            }


            localPort = ((IPEndPoint)outSocket.LocalEndPoint).Port;
            network.localUdpPorts.Add(localPort);
            log.Info("Local Port = " + localPort);

            IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)ipeSender;
            inStateNetworkObject.WorkSocket.BeginReceiveFrom(inStateNetworkObject.NetworkMessage.Buffer, 0, inStateNetworkObject.NetworkMessage.Buffer.Length, SocketFlags.None, ref epSender, receiveCallback, inStateNetworkObject);

            isStarted = true;
        }

        /// <summary>
        /// Stops the UDPAsyncTransport. 
        /// </summary>
        public void Stop()
        {
            try
            {
                if (isStarted)
                {

                    inStateNetworkObject.WorkSocket.Close();
                    outSocket.Close();
                    network.localUdpPorts.Remove(localPort);
                    isStarted = false;
                    //pool
                    queue.Enqueue(inStateNetworkObject.NetworkMessage);
                }

            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while stoping UDPAsyncTransport", ex);
                throw new TransportException("Socket exception while stoping in UDPAsyncTransport", ex);

            }
            catch (Exception ex)
            {
                log.Error("Exception while stoping in UDPAsyncTransport", ex);
                throw new TransportException("Socket exception while stoping in UDPAsyncTransport", ex);
            }
        }

        /// <summary>
        /// Sends a NetworkMessage to a given TransportAddress.
        /// </summary>
        public void Send(NetworkMessage networkMessage)
        {
            lock (this)
            {
                if (isStarted)
                {
                    IpTransportAddress ta = (IpTransportAddress)networkMessage.TransportAddress;
                    try
                    {
                        outSocket.BeginSendTo(networkMessage.Buffer, 0, networkMessage.Offset, SocketFlags.None, ta.ipEndPoint, sendCallback, outSocket);
                        log.Debug("UDP Packet sent to " + ta.ipEndPoint + ", " + networkMessage.Offset + " bytes");

                    }
                    catch (SocketException ex)
                    {
                        log.Error("Socket exception while sending in UDPAsyncTransport", ex);
                        throw new TransportException("Socket exception while sending in UDPAsyncTransport", ex);

                    }
                    catch (Exception ex)
                    {
                        log.Error("Exception while sending in UDPAsyncTransport", ex);
                        throw new TransportException("Socket exception while sending in UDPAsyncTransport", ex);
                    }
                }

            }
        }

        /// <summary>
        /// Callback method to send UDP data  
        /// </summary>
        private void SendCallback(IAsyncResult iar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                Socket state = (Socket)iar.AsyncState;

                state.EndSendTo(iar);
            }

            catch (ObjectDisposedException)
            {
                //ObjectDisposedException should be ignored: happens when the socket is closed and disposed.
            }

            catch (ArgumentException)
            {
                //Ignore
            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while sending in UDPAsyncTransport", ex);
                throw new TransportException("Socket exception while sending in UDPAsyncTransport", ex);
            }
            catch (Exception ex)
            {
                log.Error("Exception while sending in UDPAsyncTransport", ex);
                throw new TransportException("Socket exception while sending in UDPAsyncTransport", ex);
            }
        }

        /// <summary>
        /// Callback method to receive UDP data  
        /// </summary>
        private void ReceiveCallback(IAsyncResult iar)
        {
            StateNetworkObject state = null;
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                state = (StateNetworkObject)iar.AsyncState;

                // Creates a temporary EndPoint to pass to EndReceiveFrom.

                EndPoint tempRemoteEP = (EndPoint)emptyIPEndPoint;
                int bytesRead = state.WorkSocket.EndReceiveFrom(iar, ref tempRemoteEP);

                if (network.localUdpPorts.Contains(((IPEndPoint)tempRemoteEP).Port) && network.localIPs.Contains(((IPEndPoint)tempRemoteEP).Address))
                {

                }
                else
                {
                    state.NetworkMessage.Offset = bytesRead;
                    state.NetworkMessage.SetTransportAddress(network.GetBroadcastAddress(emptyIPEndPoint.Address), address.ipEndPoint.Port, TransportMode.UDP);
                    network.Receive(state.NetworkMessage);
                }
                // Get the rest of the data.
                EndPoint sender = (EndPoint)emptyIPEndPoint;
                state.WorkSocket.BeginReceiveFrom(state.NetworkMessage.Buffer, 0, state.NetworkMessage.Buffer.Length, SocketFlags.None, ref sender, receiveCallback, state);
            }
            catch (ObjectDisposedException)
            {
                //ObjectDisposedException should be ignored: happens when the socket is closed and disposed.
            }
            catch (ArgumentException)
            {
                //Ignore
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10004)
                {
                    //It's safe to ignore this :)
                }
                else
                {
                    //Update NetworkStatusCode instead of throwing an exception
                    //log.Error("Got socket exception while receiving in UdpASyncTransport", e);
                    //throw new TransportException("Got socket exception while receiving in UdpAsyncTransport", e);
                    //TODO The state is updated but not passed to the upper layers
                    state.NetworkMessage.StatusCode = NetWorkStatusCode.TransportReceivingError;
                }

            }
            catch (Exception ex)
            {
                log.Error("Exception while receiving in UDPAsyncTransport", ex);
                throw new TransportException("Socket exception while receiving in UDPAsyncTransport", ex);
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