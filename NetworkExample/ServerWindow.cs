using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetworkExample
{
    public partial class ServerWindow : Form
    {
        /// <summary>
        /// The server connection.
        /// </summary>
        private TcpIpServerConnection _server;

        /// <summary>
        /// Dictionary of clients connected to the server.
        /// </summary>
        private Dictionary<TcpIpServerConnection.ClientConnection, ConnectedClient> _clients = new Dictionary<TcpIpServerConnection.ClientConnection, ConnectedClient>();
        
        /// <summary>
        /// Creates a new instance of <see cref="ServerWindow"/>.
        /// </summary>
        public ServerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles a click on the start button.
        /// </summary>
        private void OnStartButtonClick(object sender, EventArgs e)
        {
            if (ReferenceEquals(_server, null))
            {
                // We have to start the server.
                int port;
                if (int.TryParse(portTextbox.Text, out port))
                {
                    _server = new TcpIpServerConnection(port);
                    _server.Timeout = NetworkConstants.DefaultTimeout;
                    _server.ClientConnected += OnNewConnection;
                    _server.ClientDisconnected += OnClosedConnection;
                    _server.MessageReceived += OnDataReceived;
                    _server.Open();
                    statusWindow.Text += "\r\nOpened Server on Port " + port;
                    portTextbox.Enabled = false;
                    startButton.Text = "Stop";
                }
                else
                {
                    statusWindow.Text += "\r\nCannot parse port.";
                }
            }
            else
            {
                // We close the Server.
                _server.Dispose();
                _server = null;
                statusWindow.Text += "\r\nStopped Server!";
                portTextbox.Enabled = true;
                startButton.Text = "Start";
            }
        }

        public void OnNewConnection(object sender, ClientEventArgs e)
        {
            // Create a connectedClient instance and store it inside the _clients
            var connectedClient = new ConnectedClient();
            connectedClient.State = ConnectedClient.TransferState.Connected;
            connectedClient.Client = e.Client;
            _clients.Add(e.Client, connectedClient);
        }

        public void OnClosedConnection(object sender, ClientEventArgs e)
        {
            // Get the connected client, set the state and remove it from the _clients.
            var connectedClient = _clients[e.Client];
            connectedClient.State = ConnectedClient.TransferState.Disconncted;
            _clients.Remove(e.Client);
        }

        /// <summary>
        /// Handle the data received.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Bytes received event argument.</param>
        public void OnDataReceived(object sender, BytesReceivedEventArgs e)
        {
            var clientConnection = sender as TcpIpServerConnection.ClientConnection;
            if (clientConnection == null) throw new ArgumentException(nameof(sender));

            var client = _clients[clientConnection];
            // Handle received data
        }
    }
}
