using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;

namespace Marea
{
    /// <summary>
    /// Delegate to close an IConnection.
    /// </summary>
    public delegate bool CloseConnection(IConnection connection);

    /// <summary>
    /// ConnectionState to control IConnection.
    /// </summary>
    public enum ConnectionState { waitingMareaHeader, waitingData };

    public class ConnectionManager
    {
        /// <summary>
        /// List<IConnection> to manage IConnections.
        /// </summary>
        protected List<IConnection> connections;

        /// <summary>
        /// CloseConnection delegate.
        /// </summary>
        public CloseConnection closeConnectionHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectionManager()
        {
            this.connections = new List<IConnection>();
            this.closeConnectionHandler = this.Close;
        }

        /// <summary>
        /// Adds an IConnection to the ConnectionManager.
        /// </summary>
        public void AddInConnection(IConnection connection)
        {
            lock (connections)
            {
                connections.Add(connection);
            }
        }

        /// <summary>
        /// Closes all IConnections from the ConnectionManager.
        /// </summary>
        public void CloseConnections()
        {
            lock (connections)
            {
                foreach (IConnection c in connections)
                {
                    c.Close();
                }
                connections.Clear();
            }
        }

        /// <summary>
        /// Removes a IConnections from the ConnectionManager.
        /// </summary>
        protected bool Remove(IConnection connection)
        {
            lock (connections)
            {
                return connections.Remove(connection);
            }
        }

        /// <summary>
        /// Closes the given IConnections from the ConnectionManager.
        /// </summary>
        public bool Close(IConnection connection)
        {
            lock (connections)
            {
                connection.Close();
                return this.Remove(connection);
            }
        }
    }
}
