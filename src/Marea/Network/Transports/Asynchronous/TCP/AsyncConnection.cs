#if ASYNCHRONOUS_TRANSPORTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

using log4net;

namespace Marea
{
    /// <summary>
    /// AsyncConnection used in TCPAsyncTransport.
    /// </summary>
    public class AsyncConnection : IConnection
    {
        /// <summary>
        /// Logger for the AsyncConnection.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Marea.Transport.AsyncConnection");

        /// <summary>
        /// Socket for the incoming data.
        /// </summary>
        public Socket socket;

        /// <summary>
        /// Local port.
        /// </summary>
        protected int localPort;

        /// <summary>
        /// ConnectionState enum to control the reception of MAREA header.
        /// </summary>
        protected ConnectionState state = ConnectionState.waitingMareaHeader;

        /// <summary>
        /// byte[3] which contains MAREA magic number: "MR1". This array is contained in MAREA header.
        /// </summary>
        protected static byte[] magicNumber = new byte[] { (byte)'M', (byte)'R', (byte)'1' };

        /// <summary>
        /// byte[4] which contains the size of the incoming data. This array is contained in MAREA header.
        /// </summary>
        protected byte[] mareaHeader = new byte[magicNumber.Length + sizeof(Int32)];

        /// <summary>
        /// Thread used to send data if an exception occurs.
        /// </summary>
        protected Thread thTry;

        /// <summary>
        /// ConcurrentQueue<NetworkMessage> used to store NetworkMessage entities in case an exception occurs.
        /// </summary>
        protected ConcurrentQueue<NetworkMessage> outBuffer;

        /// <summary>
        /// AsyncCallback delegated used to receive data.
        /// </summary>
        protected AsyncCallback receiveCallBack;

        /// <summary>
        /// AsyncCallback delegated used to send data.
        /// </summary>
        protected AsyncCallback sendCallback;

        /// <summary>
        /// ManualResetEvent used to control the reception of data.
        /// </summary>
        public ManualResetEvent receiveDone = new ManualResetEvent(false);

        /// <summary>
        /// CloseConnection delegate use to close the connection.
        /// </summary>
        protected CloseConnection closeConnectionHandler;

        /// <summary>
        /// NetworkMessage instance.
        /// </summary>
        private NetworkMessage networkMessage;

        /// <summary>
        /// Network instance.
        /// </summary>
        private Network network;

        /// <summary>
        /// LockFreeQueue instance.
        /// </summary>
        private LockFreeQueue queue;

        /// <summary>
        /// output Lane used to store the network bindings between the network sublayers (coder and transports).
        /// </summary>
        protected Lane hint = null;

        /// <summary>
        /// Constructor from incoming AsyncConnections
        /// </summary>
        public AsyncConnection(Network network, Socket socket, CloseConnection closeConnectionHandler)
        {
            this.queue = LockFreeQueue.GetInstance();
            this.socket = socket;
            this.networkMessage = queue.Dequeue();

            this.closeConnectionHandler = closeConnectionHandler;
            this.localPort = ((IPEndPoint)socket.LocalEndPoint).Port;

            this.receiveCallBack = new AsyncCallback(ReceiveCallback);
            this.network = network;

            try
            {
                this.socket.BeginReceive(mareaHeader, 0, mareaHeader.Length, SocketFlags.None, receiveCallBack, socket);
                receiveDone.WaitOne();
            }
            catch (ObjectDisposedException)
            {
                //It's safe to ignore this :)
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10004)
                {
                    //It's safe to ignore this :)
                }
                else if (ex.ErrorCode == 10054 || ex.ErrorCode == 10053)
                {
                    //The remote host closes this connection
                    closeConnectionHandler(this);
                }
                else
                {
                    log.Error("Socket exception while receiving in AsyncConnection", ex);
                }

            }
            catch (Exception ex)
            {
                log.Error("Exception while receiving in AsyncConnection", ex);
                throw new TransportException("Exception while receiving in AsyncConnection", ex);
            }

        }

        /// <summary>
        /// Constructor from outgoing AsyncConnection
        /// </summary>
        public AsyncConnection(Socket inSocket)
        {
            Buffer.BlockCopy(magicNumber, 0, mareaHeader, 0, magicNumber.Length);

            this.socket = inSocket;
            outBuffer = new ConcurrentQueue<NetworkMessage>();
            this.sendCallback = new AsyncCallback(SendCallback);
        }


        /// <summary>
        /// Callback method to receive data through the AsyncConnection.
        /// </summary>
        private void ReceiveCallback(IAsyncResult iar)
        {
            Socket receiveSocket = null;
            try
            {
                receiveSocket = (Socket)iar.AsyncState;

                int bytesRead = receiveSocket.EndReceive(iar);
                receiveDone.Set();

                switch (state)
                {
                    case ConnectionState.waitingMareaHeader:
                        if (mareaHeader[0] == magicNumber[0] && mareaHeader[1] == magicNumber[1] && mareaHeader[2] == magicNumber[2])
                        {
                            networkMessage.Offset = (int)(
                                                (mareaHeader[magicNumber.Length] & 0x000000FF) |
                                                (mareaHeader[magicNumber.Length + 1] << 8 & 0x0000FF00) |
                                                (mareaHeader[magicNumber.Length + 2] << 16 & 0x00FF0000) |
                                                (mareaHeader[magicNumber.Length + 3] << 24)
                            );

                            state = ConnectionState.waitingData;
                            receiveSocket.BeginReceive(networkMessage.Buffer, 0, networkMessage.Offset, SocketFlags.None, receiveCallBack, receiveSocket);

                        }
                        break;

                    case ConnectionState.waitingData:
                        networkMessage.StatusCode = NetWorkStatusCode.OK;
                        hint = network.Receive(networkMessage, hint);
                        state = ConnectionState.waitingMareaHeader;
                        receiveSocket.BeginReceive(mareaHeader, 0, mareaHeader.Length, SocketFlags.None, receiveCallBack, receiveSocket);
                        break;
                }
                receiveDone.WaitOne();

                if (log.IsDebugEnabled)
                    log.Debug("TCP Packet received in local port: " + localPort + ", bytes readed:  " + bytesRead + " bytes");
            }
            catch (ObjectDisposedException)
            {
                //It's safe to ignore this :)
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10004)
                {
                    //It's safe to ignore this :)
                }
                else if (ex.ErrorCode == 10054 || ex.ErrorCode == 10053)
                {
                    //The remote host closes this connection
                    closeConnectionHandler(this);
                    //TODO The state is updated but not passed to the upper layers
                    networkMessage.StatusCode = NetWorkStatusCode.TransportReceivingError;
                }
                else
                {
                    log.Error("Socket exception while receiving in AsyncConnection", ex);
                    //TODO The state is updated but not passed to the upper layers
                    networkMessage.StatusCode = NetWorkStatusCode.TransportReceivingError;
                }

            }
            catch (Exception ex)
            {
                log.Error("Exception while receiving in AsyncConnection", ex);
                throw new TransportException("Exception while receiving in AsyncConnection", ex);
            }
        }

        /// <summary>
        /// Closes the AsyncConnection.
        /// </summary>
        public void Close()
        {
            try
            {
                socket.Close();
                if (thTry != null)
                    thTry.Abort();
                queue.Enqueue(networkMessage);

            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while closing AsyncConnection", ex);
                throw new TransportException("Socket exception while closing AsyncConnection", ex);
            }
            catch (Exception ex)
            {
                log.Error("Exception while closing AsyncConnection", ex);
                throw new TransportException("Exception while closing AsyncConnection", ex);
            }
        }

        /// <summary>
        /// Sends data through th AsyncConnection.
        /// </summary>
        public void Send(NetworkMessage networkMessage)
        {

            IpTransportAddress ta = (IpTransportAddress)networkMessage.TransportAddress;

            if (log.IsDebugEnabled)
            {
                log.Debug("TCP Packet sent to " + ta.ipEndPoint);
                log.Debug("Bytes sent : " + networkMessage.Offset);
            }
            try
            {
                mareaHeader[magicNumber.Length] = (byte)(networkMessage.Offset);
                mareaHeader[magicNumber.Length + 1] = (byte)(networkMessage.Offset >> 8);
                mareaHeader[magicNumber.Length + 2] = (byte)(networkMessage.Offset >> 16);
                mareaHeader[magicNumber.Length + 3] = (byte)(networkMessage.Offset >> 24);

                socket.BeginSend(mareaHeader, 0, mareaHeader.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                socket.BeginSend(networkMessage.Buffer, 0, networkMessage.Offset, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    outBuffer.Enqueue(networkMessage);

                    if (log.IsDebugEnabled)
                        log.Debug("Packet Queued");

                    if (thTry == null)
                    {
                        thTry = new Thread(this.TrySending);
                        thTry.Start();
                    }
                }

                else
                {
                    log.Error("Socket exception while sending in AsyncConnection", ex);
                    throw new TransportException("Socket exception while sending in AsyncConnection", ex);
                }

            }
            catch (Exception ex)
            {
                log.Error("Exception while sending in AsyncConnection", ex);
                throw new TransportException("Socket exception while sending in AsyncConnection", ex);
            }
        }

        /// <summary>
        /// Callback method to send data
        /// </summary>
        private void SendCallback(IAsyncResult iar)
        {
            try
            {
                Socket socket = (Socket)iar.AsyncState;
                socket.EndSend(iar);
            }
            catch (SocketException ex)
            {
                log.Error("Socket exception while sending (Callback) in AsyncConnection", ex);
                throw new TransportException("Socket exception while sending (Callback) in AsyncConnection", ex);

            }
            catch (Exception ex)
            {
                log.Error("Exception while sending (Callback) in AsyncConnection", ex);
                throw new TransportException("Socket exception while sending(Callback) in AsyncConnection", ex);
            }
        }

        /// <summary>
        /// Sends the packets to the queue.
        /// </summary>
        private void TrySending()
        {
            NetworkMessage networkMessage;
            while (outBuffer.Count > 0)
            {
                if (outBuffer.TryDequeue(out networkMessage))
                    this.Send(networkMessage);
            }
            thTry = null;
        }
    }
}
#endif