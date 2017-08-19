using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NetworkExample
{
    /// <summary>
    /// Tcp/ip server connection.
    /// 
    /// This opens a socket on the server and listens for new connections. All connections
    /// are handled inside.
    /// </summary>
    public class TcpIpServerConnection : IDisposable
    {
        #region ClientConnection

        /// <summary>
        /// Client connection - all data about a client that is connected to the server.
        /// </summary>
        public class ClientConnection : IDisposable
        {
            #region constants

            /// <summary>
            /// Size of the buffer.
            /// </summary>
            public const int BufferSize = 4096;

            #endregion

            #region Fields

            /// <summary>
            /// Trace source for all logging in this class.
            /// </summary>
            private static readonly TraceSource Logger = new TraceSource(NetworkConstants.TraceSourceName);

            /// <summary>
            /// Socket of this connection.
            /// </summary>
            private readonly Socket _socket;

            /// <summary>
            /// Buffer to read data.
            /// </summary>
            private readonly byte[] _buffer = new byte[BufferSize];

            /// <summary>
            /// Timestamp of last Activity of the connection.
            /// </summary>
            private DateTime _lastActivity;

            #endregion

            #region Properties

            /// <summary>
            /// Sets or gets if this instance is disposed already.
            /// </summary>
            public bool Disposed { get; private set; } = false;

            /// <summary>
            /// Timeout in seconds. 0 means no timeout.
            /// </summary>
            public int Timeout { get; set; }

            /// <summary>
            /// Id of the client connection - used to identify this client connection.
            /// </summary>
            public Guid Id { get; set; } = new Guid();

            #endregion

            #region Events

            /// <summary>
            /// Event that a Message was Received
            /// </summary>
            public event EventHandler<BytesReceivedEventArgs> MessageReceived;

            #endregion

            #region Lifetime

            /// <summary>
            /// Creates a new instance of ClientConnection.
            /// </summary>
            /// <param name="socket"></param>
            public ClientConnection(Socket socket)
            {
                _socket = socket;
                _lastActivity = DateTime.Now;
                Logger.TraceEvent(TraceEventType.Information, 0, "New client connection {0}.", Id);
            }

            /// <summary>
            /// Finalizer.
            /// </summary>
            ~ClientConnection()
            {
                Logger.TraceEvent(TraceEventType.Error, 0, "Instance of ClientConnection wasn't finalized!");
                Dispose(false);
            }

            /// <summary>
            /// Dispose this instance.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Dispose this client connection.
            /// </summary>
            /// <param name="disposing"></param>
            protected void Dispose(bool disposing)
            {
                // Only dispose once.
                if (Disposed)
                    return;

                // Log message
                Logger.TraceEvent(TraceEventType.Information, 0, "Disposing client connection {0}.", Id);

                // Do the dispose
                if (disposing)
                {
                    // Dispose socket
                    _socket?.Dispose();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Handles possible input of the client.
            /// </summary>
            /// <returns>True if client connection should be closed. Else false.</returns>
            public bool HandleInput()
            {
                try
                {
                    if (_socket.Available > 0)
                    {
                        var bytesRead = _socket.Receive(_buffer);
                        _lastActivity = DateTime.Now;
                        OnMessageReceived(this, new BytesReceivedEventArgs(_buffer.Take(bytesRead).ToArray()));
                    }
                    else
                    {
                        // Check if we have a Timeout value and if the timeout was reached.
                        if (Timeout > 0 && DateTime.Now.Subtract(_lastActivity).TotalSeconds > Timeout)
                        {
                            // Timeout occured - close connection.
                            Logger.TraceEvent(TraceEventType.Information, 0, "Closing client connection {0} because a timeout occured.", Id);
                            return true;
                        }
                    }

                    // Return false - the connection should not be closed.
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.TraceEvent(TraceEventType.Error, 0, "Exception when reading from socket of Client {2}: {0} / {1}", ex.Message, ex.StackTrace, Id);

                    // When we was unable to read from the socket then it should be closed.
                    return true;
                }
            }

            /// <summary>
            /// Send the message to the client.
            /// </summary>
            /// <param name="message">Message to send.</param>
            public void Send(byte[] message)
            {
                _socket.Send(message);
            }

            /// <summary>
            /// Close the client connection.
            /// </summary>
            public void Close()
            {
                Dispose();
            }

            /// <summary>
            /// A message was received by the client connection.
            /// </summary>
            /// <param name="sender">Sender of this event,</param>
            /// <param name="e">Event.</param>
            void OnMessageReceived(object sender, BytesReceivedEventArgs e)
            {
                var handle = MessageReceived;
                handle?.Invoke(sender, e);
            }

            #endregion
        }

        #endregion

        #region Fields

        /// <summary>
        /// A Collection of all connected clients.
        /// </summary>
        private readonly Collection<ClientConnection> _clients = new Collection<ClientConnection>();

        /// <summary>
        /// Socket that listens for new connections.
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// IO thread
        /// </summary>
        private Thread _ioThread;

        /// <summary>
        /// Signals if the loop should stop or not.
        /// </summary>
        private bool _shouldStop;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates a new instance of TcpIpServerConnection.
        /// </summary>
        public TcpIpServerConnection()
        { }

        /// <summary>
        /// Creates a new instance of TcpIpServerConnection.
        /// </summary>
        /// <param name="port">Port to listen for new connections.</param>
        public TcpIpServerConnection(int port)
           : this()
        {
            Port = port;
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~TcpIpServerConnection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this instance.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            // Check if we are already deisposed.
            if (Disposed)
                return;

            // Dispose this connection
            if (disposing)
            {
                if (!_shouldStop)
                {
                    _shouldStop = true;
                    // Small wait for thread to end.
                    Thread.Sleep(30);
                }

                _socket?.Dispose();

                foreach (var clientConnection in _clients)
                {
                    clientConnection.Dispose();
                }
                _clients.Clear();

                // Abort Thread if required.
                _ioThread?.Abort();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that a message was Received.
        /// </summary>
        /// <remarks>
        /// This event is handled by the io Thread of the TcpIpServerConnection so make sure that you do not
        /// use it for any stuff that is time consuming. Best is to queue Data only and use other threads
        /// to process it
        /// 
        /// Also be sure to handle all Exception if you do not want your connection closed! Exception handling is
        /// simply closing the connection to the client.
        /// </remarks>
        public event EventHandler<BytesReceivedEventArgs> MessageReceived;

        /// <summary>
        /// A client connected to the server.
        /// </summary>
        public event EventHandler<ClientEventArgs> ClientConnected;

        /// <summary>
        /// A client was disconected from the server.
        /// </summary>
        public event EventHandler<ClientEventArgs> ClientDisconnected;

        #endregion

        #region Properties

        /// <summary>
        /// Port to listen on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The Timeout used for connections in seconds. 0 means no timeout.
        /// </summary>
        public int Timeout { get; set; } = NetworkConstants.DefaultTimeout;

        /// <summary>
        /// Sets or gets if this instance is disposed already.
        /// </summary>
        public bool Disposed { get; private set; } = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Open the socket and listen for new connections.
        /// </summary>
        public void Open()
        {
            if (Disposed)
                throw new InvalidOperationException("Instance already disposed!");

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _socket.Listen(1000);

            _shouldStop = false;
            _ioThread = new Thread(HandleIo);
            _ioThread.Start();
        }

        /// <summary>
        /// Close the socket and all client connections, dispose this instance.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle the IO of the server. 
        /// </summary>
        private void HandleIo()
        {
            // Stopwatch to measure "load"
            var stopwatch = new Stopwatch();

            // collection of client connectios that should be closed.
            var clientsToDispose = new Collection<ClientConnection>();

            // Main Loop
            while (!_shouldStop)
            {
                stopwatch.Start();

                // check if new connections are available
                if (_socket.Poll(0, SelectMode.SelectRead))
                {
                    // Create a new ClientConnection, add it to the _clients connection and listen to MessageReceived events.
                    var connection = new ClientConnection(_socket.Accept());
                    connection.Timeout = Timeout;
                    _clients.Add(connection);
                    connection.MessageReceived += OnMessageReceived;
                    OnClientConnected(this, new ClientEventArgs(connection));
                }

                // Go through all clientConnections and handle possible input.
                foreach (var clientConnection in _clients)
                {
                    if (clientConnection.HandleInput())
                        clientsToDispose.Add(clientConnection);
                }

                // remove clients that should be disposed.
                foreach (var clientConnection in clientsToDispose)
                {
                    clientConnection.Dispose();
                    _clients.Remove(clientConnection);
                    OnClientDisconnected(this, new ClientEventArgs(clientConnection));
                }

                // clear collection of clients that should be removed.
                clientsToDispose.Clear();

                // Wait a little bit depending on load (duration of check).
                stopwatch.Stop();
                if (stopwatch.Elapsed.TotalMilliseconds < 20)
                    Thread.Sleep(20);
                else if (stopwatch.Elapsed.TotalMilliseconds < 100)
                    Thread.Sleep(10);

                if (!_socket.IsBound)
                    _shouldStop = true;
            }

            // Cleanup
            _socket?.Dispose();
            _ioThread = null;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// A message was received.
        /// </summary>
        /// <param name="sender">Sender of the event. This is a client connection.</param>
        /// <param name="e">Message with received bytes.</param>
        protected virtual void OnMessageReceived(object sender, BytesReceivedEventArgs e)
        {
            var handle = MessageReceived;
            handle?.Invoke(sender, e);
        }

        /// <summary>
        /// A client connected.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">ClientEventArgs with connected client.</param>
        protected virtual void OnClientConnected(object sender, ClientEventArgs e)
        {
            var handle = ClientConnected;
            handle?.Invoke(sender, e);
        }

        /// <summary>
        /// A client was disconnected.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">ClientEventArgs with disconnected client.</param>
        protected virtual void OnClientDisconnected(object sender, ClientEventArgs e)
        {
            var handle = ClientDisconnected;
            handle?.Invoke(sender, e);
        }

        #endregion
    }
}
