using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Marea
{

    /// <summary>
    /// Manages all the basic operations of the network layer.
    /// </summary>
    public class Network
    {
#if ASYNCHRONOUS_TRANSPORTS
        /// <summary>
        /// UDPAsyncTransport.
        /// </summary>
        public UDPAsyncTransport UdpTransport;

        /// <summary>
        /// AsyncTCPTransport.
        /// </summary>
        public TCPAsyncTransport TcpTransport;
#elif SYNCHRONOUS_TRANSPORTS
        /// <summary>
        /// UDPTransport.
        /// </summary>
        public UDPTransport UdpTransport;

        /// <summary>
        /// TCPTransport.
        /// </summary>
        public TCPTransport TcpTransport;
#endif
        /// <summary>
        /// Used to get last multicast IPTrasnportAddress.
        /// </summary>
        private IpTransportAddress lastMulticast;

        /// <summary>
        /// Router instance.
        /// </summary>
        private Router router;

        /// <summary>
        /// LockFreeQueue instance.
        /// </summary>
        private LockFreeQueue queue;

        /// <summary>
        /// LockFreeQueue instance.
        /// </summary>
        public ServiceContainer container;

        
        /// <summary>
        /// Local broadcast Transport Address
        /// </summary>
        public TransportAddress Broadcast;

        /// <summary>
        /// Local control Transport Address
        /// </summary>
        public TransportAddress Control;

        /// <summary>
        /// Default Marea broadcast listen port
        /// </summary>
        public int broadcastPort = 11812;

        /// <summary>
        /// Default Marea control listen port
        /// </summary>
        public int controlPort = 11000;

        /// <summary>
        /// List of local IPs (for avoiding loopback datagrams in UdpTransport)
        /// </summary>
        public IPAddress[] localIPs;

        /// <summary>
        /// List of local ouptput UDP ports (for avoiding loopback datagrams in UdpTransport)
        /// </summary>

        public List<int> localUdpPorts;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Network(ServiceContainer container)
        {
            this.router = new Router(this);
            this.queue = LockFreeQueue.GetInstance();
            this.container = container;
            this.localIPs = availableNetAdapters();
            this.localUdpPorts = new List<int>();
        }

        /// <summary>
        /// Starts the entire network layer.
        /// </summary>
        public void Start()
        {
            LoadNetworkConfiguration();

            TransportAddress ta = TcpTransport.GetTransportAddress();
            SetControlTransport(ta);

            
			//TODO This generates a valid multicast address???
			if (ta is IpTransportAddress)
            {
                Byte[] tmp = ((IpTransportAddress)ta).ipEndPoint.Address.GetAddressBytes();
                tmp[0] = 225;
                lastMulticast = new IpTransportAddress(new IPAddress(tmp), 1024, TransportMode.UDP);
            }

        }

        /// <summary>
        /// Stops the entire network layer.
        /// </summary>
        public void Stop()
        {
            this.UdpTransport.Stop();
            this.TcpTransport.Stop();
        }

        /// <summary>
        /// This method is used to send data. Creates an output network lane and calls it execution.
        /// </summary>
        public Lane Send(TransportAddress transportAddress, Object o)
        {
            return Send(transportAddress, o, null);
        }

        /// <summary>
        /// This method is used to send data. Reuses the output lane if it's possible or demands to the router otherwise. 
        /// </summary>
        public Lane Send(TransportAddress transportAddress, Object o, Lane hint)
        {
            //Get hint
            if (hint == null)
                hint = router.GetOutputLane(transportAddress);

            //Get NetworkMessage from the pool and set the fields 
            NetworkMessage networkMessage = queue.Dequeue();
            networkMessage.SetMessage(transportAddress, o);

            while (hint != null)
            {
                //MultiDelegate call
                hint.NetworkHandler(networkMessage);
                if (networkMessage.StatusCode == NetWorkStatusCode.OK)
                    break;
                else
                    hint = router.GetOutputLane(transportAddress);
            }

            //pool
            queue.Enqueue(networkMessage);

            //TODO throw the exception in a smarter way (parameters, name...)
            if (hint == null)
                throw new TransportException();
            return hint;
        }

        /// <summary>
        /// This method is used to receive data. Creates an input network lane and calls it execution.
        /// </summary>
        public Lane Receive(NetworkMessage networkMessage)
        {
            return Receive(networkMessage, null);
        }

        /// <summary>
        /// This method is used to receive data. Reuses the input lane if it's possible or demands to the router otherwise. 
        /// </summary>
        public Lane Receive(NetworkMessage networkMessage, Lane hint)
        {
            //Get hint
            if (hint == null)
                hint = router.GetInputLane(networkMessage.TransportAddress);

            while (hint != null)
            {
                //MultiDelegate call
                hint.NetworkHandler(networkMessage);
                if (networkMessage.StatusCode == NetWorkStatusCode.OK)
                    break;
                else
                    hint = router.GetInputLane(networkMessage.TransportAddress);
            }
            //TODO throw the exception in a smarter way (parameters, name...)
            if (hint == null)
                throw new TransportException();

            return hint;
        }

        /// <summary>
        /// Gets a UDP and TCP transport.
        /// </summary>
         private void LoadNetworkConfiguration()
        {
#if ASYNCHRONOUS_TRANSPORTS
            this.UdpTransport = new UDPAsyncTransport(this, new IpTransportAddress(GetBroadcastAddress(localIPs[0]),
                broadcastPort,
                TransportMode.UDP));

            this.TcpTransport = new TCPAsyncTransport(this, new IpTransportAddress(localIPs[0], controlPort, TransportMode.TCP));
#elif SYNCHRONOUS_TRANSPORTS
            this.UdpTransport = new UDPTransport(this, new IpTransportAddress(
                GetBroadcastAddress(localIPs[0]),
                broadcastPort,
                TransportMode.UDP));

            this.TcpTransport = new TCPTransport(this,
                new IpTransportAddress(localIPs[0], controlPort, TransportMode.TCP));
#endif
            this.UdpTransport.Start();
            this.TcpTransport.Start();
        }

        /// <summary>
        /// Gets the next multicast address from the given transportAddress.
        /// </summary>
        public TransportAddress GetNextMulticast(TransportAddress ta)
        {
            if (lastMulticast == null) return null;
            lock (lastMulticast)
            {
                // Get next address
                if (lastMulticast.ipEndPoint.Port + 1 == broadcastPort)
                {
                    lastMulticast.ipEndPoint = new IPEndPoint(lastMulticast.ipEndPoint.Address, lastMulticast.ipEndPoint.Port + 2);
                }
                else
                {
                    lastMulticast.ipEndPoint = new IPEndPoint(lastMulticast.ipEndPoint.Address, lastMulticast.ipEndPoint.Port + 1);
                }

                // Check if port is available, otherwise get another one
                if (CheckPortAvailability(lastMulticast, System.Net.Sockets.ProtocolType.Udp))
                {
                    return new IpTransportAddress(lastMulticast.ipEndPoint.Address, lastMulticast.ipEndPoint.Port, TransportMode.UDP);
                }
                else
                {
                    return GetNextMulticast(ta);
                }
            }
        }

        /// <summary>
        /// Creates local broadcast and control transport addresses
        /// </summary>
        public void SetControlTransport(TransportAddress ta)
        {
            if (ta is IpTransportAddress)
            {
                IpTransportAddress ta2 = (IpTransportAddress)ta.Clone();
                Broadcast = new IpTransportAddress(GetBroadcastAddress(ta2.ipEndPoint.Address), broadcastPort, TransportMode.UDP);
                controlPort = ta2.ipEndPoint.Port;
                Control = ta2;
            }
            else
            {
                Broadcast = ta;
                controlPort = -1;
                Control = ta;
            }
        }

        /// <summary>
        /// Returns the local IP from any other IP of the same network, otherwise null if no local IP is used of the same network.
        /// </summary>
        public IPAddress GetLocalIP(IPAddress ta)
        {
            if (IsMulticastIP(ta))
            {
                byte[] tab = ta.GetAddressBytes();
                foreach (IPAddress l in localIPs)
                {
                    byte[] lb = l.GetAddressBytes();
                    if (lb[2] == tab[2] && lb[1] == tab[1])
                    {
                        IPAddress tmp = new IPAddress(lb);
                        if (IsSameNetwork(l, tmp))
                            return l;
                    }
                }
            }
            else
            {
                foreach (IPAddress l in localIPs)
                {
                    if (IsSameNetwork(ta, l))
                    {
                        return l;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates the broadcast address from a given IP address
        /// </summary>
        public IPAddress GetBroadcastAddress(IPAddress ip)
        {
            byte[] tmp = ip.GetAddressBytes();

            if (tmp[0] < 127)
            {
                return new IPAddress(new byte[] { tmp[0], 255, 255, 255 });
            }
            else if (tmp[0] < 192)
            {
                if (tmp[1] == 0 && tmp[1] == tmp[2] && tmp[3] == 1)
                    return new IPAddress(tmp);
                else
                    return new IPAddress(new byte[] { tmp[0], tmp[1], 225, 255 });
            }
            else
            {
                return new IPAddress(new byte[] { tmp[0], tmp[1], tmp[2], 255 });
            }
        }

        /// <summary>
        /// Checks if the port of the given IPtransportAddress is avaliable using a the specified ProtocolType
        /// </summary>
        public bool CheckPortAvailability(IpTransportAddress ta, ProtocolType mode)
        {
            // Calculate local address
            IpTransportAddress toTryBind = new IpTransportAddress(
                GetLocalIP(ta.ipEndPoint.Address),
                ta.ipEndPoint.Port,
                ta.transportMode);

            // Create socket
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, mode);

            try
            {
                s.Bind(toTryBind.ipEndPoint);
                s.Close();
            }
            catch (SocketException e)
            {
                // 10013 WSAEACCESS: Access denied (address used by another program)
                if (e.ErrorCode == 10013 || e.ErrorCode == 10048)
                {
                    return false;
                }
                else
                {
                    throw e;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the IP belongs to the multicast range.
        /// Range: 224.0.1.0 - 239.255.255.255
        /// </summary>
        public bool IsMulticastIP(IPAddress ip)
        {
            byte[] b = ip.GetAddressBytes();

            if (b[0] < 224 || b[0] > 239)
            {
                return false;
            }
            else if (b[0] == 224 && b[1] == 0 && b[2] == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if the two IPAddress belong to the same network
        /// </summary>
        public bool IsSameNetwork(IPAddress ipa1, IPAddress ipa2)
        {
            byte[] ip1;
            byte[] ip2;

            if (IsMulticastIP(ipa1))
            {
                ip1 = GetLocalIP(ipa1).GetAddressBytes();
            }
            else
            {
                ip1 = ipa1.GetAddressBytes();
            }

            if (IsMulticastIP(ipa2))
            {
                ip2 = GetLocalIP(ipa2).GetAddressBytes();
            }
            else
            {
                ip2 = ipa2.GetAddressBytes();
            }


            if (ip1[0] < 128)
            {
                return
                    ip1[0] == ip2[0];
            }
            else if (ip1[0] < 192)
            {
                return
                    (ip1[0] == ip2[0]) &&
                    (ip1[1] == ip2[1]);
            }
            else if (ip1[0] < 224)
            {
                return
                     (ip1[0] == ip2[0]) &&
                    (ip1[1] == ip2[1]) &&
                    (ip1[2] == ip2[2]);
            }
            else
            {
                return ipa1.Equals(ipa2);
            }
        }

        /// <summary>
        /// Obtains an array of the available adapters IP addresses, or loopback if none available
        /// </summary>
        private IPAddress[] availableNetAdapters()
        {
            List<IPAddress> ips = new List<IPAddress>();
            NetworkInterface loopback = null;
            UnicastIPAddressInformationCollection info;

            //log.Info("Available network interfaces:");
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
#if __MonoCS__
                if (true)
#else
                if (nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
#endif

                {
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        loopback = nic;
                    else
                    {
                        info = nic.GetIPProperties().UnicastAddresses;
                        foreach (UnicastIPAddressInformation ip in info)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                //log.Info(nic.Description + " (" + ip.Address + ")");
                                ips.Add(ip.Address);
                            }
                        }
                    }
                }
            }

            info = loopback.GetIPProperties().UnicastAddresses;

            foreach (UnicastIPAddressInformation ip in info)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    //log.Info(loopback.Description + " (" + ip.Address + ")");
                    ips.Add(ip.Address);
                }
            }

			if (ips.Count == 0) {
				ips.Add(IPAddress.Parse("127.0.0.1"));
			}

            return ips.ToArray();
        }
    }
}
