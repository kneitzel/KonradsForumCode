    namespace NetworkExample
{
    /// <summary>
    /// Data of a connected client.
    /// </summary>
    public class ConnectedClient
    {
        /// <summary>
        /// Enumeration of states.
        /// </summary>
        public enum TransferState
        {
            /// <summary>
            /// The state is undefined.
            /// </summary>
            Undefined = 0,

            /// <summary>
            /// The client is connected to the server.
            /// </summary>
            Connected = 1,

            /// <summary>
            /// Size of Filename was transfered.
            /// </summary>
            NameSizeTransfered = 2,

            /// <summary>
            /// Name was transfered successfuly.
            /// </summary>
            NameTransfered = 3,

            /// <summary>
            /// Size of the file was transfered.
            /// </summary>
            SizeTransfered = 4,

            /// <summary>
            /// File was transfered successfuly.
            /// </summary>
            FileTransfered = 5,

            /// <summary>
            /// Client was disconnected.
            /// </summary>
            Disconncted = 6
        }

        /// <summary>
        /// Filename that is transfered.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Size of the file that should be transmitted.
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Current state.
        /// </summary>
        public TransferState State { get; set; }

        /// <summary>
        /// Client connection.
        /// </summary>
        public TcpIpServerConnection.ClientConnection Client { get; set; }
    }
}