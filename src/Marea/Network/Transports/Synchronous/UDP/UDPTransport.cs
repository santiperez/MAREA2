#if SYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

using log4net;


namespace Marea
{
    /// <summary>
    /// UDPTransport.
    /// </summary>
    public class UDPTransport : ITransport
    {
        /// <summary>
        /// Logger for the UDPTransport.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Marea.Transports.UDPTransport");

        /// <summary>
        /// Thread to accept the incoming connections.
        /// </summary>
        protected Thread th;

        /// <summary>
        /// Socket for the incoming data.
        /// </summary>
        protected Socket inSocket;
        
        /// <summary>
        /// Socket for the outgoing data.
        /// </summary>
        protected Socket outSocket;

        /// <summary>
        /// Local Transport Address
        /// </summary>
        protected IpTransportAddress address;

        /// <summary>
        /// Listen port for inStateObject Socket.
        /// </summary>
        protected int localPort;

        /// <summary>
        /// bool to check if the UDPTransport is started.
        /// </summary>
        private volatile bool end=true;

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
        public UDPTransport(Network network, IpTransportAddress t)
        {
            this.address = t;
            this.network = network;
            this.queue = LockFreeQueue.GetInstance();
        }

        /// <summary>
        /// Starts the UDPTransport.
        /// </summary>
        public bool Start()
        {
            if (end)
            {

                // Can multiple apps listen the same UDP port? YES
                // Read it in http://www.dotnet247.com/247reference/msgs/32/164271.aspx
                inSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                outSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                inSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                outSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

                IPAddress local = network.GetLocalIP(address.ipEndPoint.Address);
                outSocket.Bind(new IPEndPoint(local, 0));

                if (network.IsMulticastIP(address.ipEndPoint.Address))
                {
                    IpTransportAddress toBind = (IpTransportAddress)address.Clone();
                    toBind.ipEndPoint.Address = local;
                    inSocket.Bind(toBind.ipEndPoint);
                    outSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, toBind.ipEndPoint.Address.GetAddressBytes());
                    inSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                    outSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                    inSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(address.ipEndPoint.Address, toBind.ipEndPoint.Address));
                    outSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(address.ipEndPoint.Address, toBind.ipEndPoint.Address));
                    log.Debug("Multicast membership to " + address.ipEndPoint.ToString() +
                        " for local incoming address " + toBind.ipEndPoint.ToString() + " created");
                }
                else if (address.ipEndPoint.Port == network.broadcastPort)
                {
                    outSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                    //MONO Seems that MacOS X requires to bind to IPAddress.Any
					//inSocket.Bind(new IPEndPoint(local, Transport.broadcastPort));
					inSocket.Bind(new IPEndPoint(IPAddress.Any, network.broadcastPort));

                    log.Info("Broadcast Address = " + address.ToString());
                }
                else
                {
                    inSocket.Bind(new IPEndPoint(address.ipEndPoint.Address, address.ipEndPoint.Port));
                }

                localPort = ((IPEndPoint)outSocket.LocalEndPoint).Port;
                network.localUdpPorts.Add(localPort);
                log.Info("Local Port = " + localPort);

                ThreadStart ts = new ThreadStart(this.Run);
                th = new Thread(ts);
                end = false;
                th.Start();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Stops the UDPTransport. 
        /// </summary>
        public bool Stop()
        {
            if (!end)
            {
                end = true;
                inSocket.Close();
                outSocket.Close();
                network.localUdpPorts.Remove(localPort);
            }
            return end;
        }

        /// <summary>
        /// Sends UDP data.  
        /// </summary>
        public void Send(NetworkMessage networkMessage)
        {

            IpTransportAddress ta = (IpTransportAddress)networkMessage.TransportAddress;
            int write = 0;

            try
            {
                while (write != networkMessage.Offset)
                {
                    write += outSocket.SendTo(networkMessage.Buffer, write, networkMessage.Offset - write, SocketFlags.None, ta.ipEndPoint);
                }
                if (log.IsDebugEnabled)
                    log.Debug("UDP Packet sent to " + ta.ipEndPoint + ", " + networkMessage.Offset + " bytes");
                
            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while sending in UDPTransport", ex);
                throw new TransportException("Socket exception while sending in UDPTransport", ex);

            }
            catch (Exception ex)
            {
                if (!end)
                {
                    log.Error("Exception while sending in UDPAsyncTransport", ex);
                    throw new TransportException("Socket exception while sending in UDPAsyncTransport", ex);
                }
            }
        }

        /// <summary>
        /// Receives UDP data. 
        /// </summary>
        private void Run()
        {
            EndPoint ep = new IPEndPoint(IPAddress.Any, address.ipEndPoint.Port);
            //SocketFlags flags ;
            IPEndPoint ip;
            int received = 0;

            //pool
            NetworkMessage networkMessage = queue.Dequeue();
            while (!end)
            {
               
                try
                {
                    if (log.IsDebugEnabled)
                        log.Debug("Waiting for UDP Packets...");
                    //flags = SocketFlags.None;

                    //IPPacketInformation ipinfo;
                   
					//Not implemented on MONO
					//received = inSocket.ReceiveMessageFrom(networkMessage.Buffer, 0, 
					//                                       networkMessage.Buffer.Length, ref flags, ref ep, out ipinfo);

					received = inSocket.ReceiveFrom(networkMessage.Buffer, ref ep);

                    ip = (IPEndPoint)ep;

					//TODO Keep an Array NOT a copy for performance... 
                    if ((Array.IndexOf(network.localUdpPorts.ToArray(), ip.Port) >= 0) && (Array.IndexOf(network.localIPs, ip.Address) >= 0))
                    {
                        // Ignores the packets from local port and local IPs
                        continue;
                    }
                    networkMessage.Offset = received;
                    networkMessage.SetTransportAddress(ip.Address, ip.Port, TransportMode.UDP);

                    if (log.IsDebugEnabled)
                        log.Debug("UDP Packet received from " + ip.Address + ", " + received + " bytes");

                    network.Receive(networkMessage);
                }
                catch (ThreadAbortException)
                {
                    //It's safe to ignore this :)
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
                        //log.Error("Got socket exception while receiving in UdpTransport", ex);
                        //throw new TransportException("Got socket exception while receiving in UdpTransport", ex);
                        //TODO The state is updated but not passed to the upper layers
                        networkMessage.StatusCode = NetWorkStatusCode.TransportReceivingError;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Got exception while receiving in UdpTransport", ex);
                    throw new TransportException("Got exception while receiving in UdpTransport", ex);
                   
                }
            }
            //Add this object back into the pool.
            queue.Enqueue(networkMessage);
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