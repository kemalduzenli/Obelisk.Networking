using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Obelisk.Networking
{
    public class Server : Peer
    {
        private readonly Socket socket;
        public ushort Port { get; private set; }

        public readonly Dictionary<byte, Connection> Connections;

        public event Action<Connection> OnClientConnected;
        public event Action<Connection> OnClientDisconnected;

        public event Action<Connection, Packet> OnPacket;

        private bool pendingConnection;
        private byte idCounter;

        public Server() 
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Connections = new();
        }

        public void Start(ushort port, ushort maxConnectedClientCount = 5, IPAddress ipAddress = null)
        {
            Port = port;

            if (ipAddress == null)
                ipAddress = IPAddress.Parse("127.0.0.1");

            IPEndPoint endPoint = new IPEndPoint(ipAddress, Port);

            socket.Bind(endPoint);
            socket.Listen(maxConnectedClientCount);
        }

        public override void Send(Packet packet)
        {
            foreach (Connection connection in Connections.Values)
                connection.Send(packet);
        }

        public void Send(Connection connection, Packet packet) =>
            connection.Send(packet);

        public void Send(byte connectionId, Packet packet)
        {
            if(Connections.TryGetValue(connectionId, out Connection connection)) 
                connection.Send(packet);
        }
        
        public override void Update()
        {
            if (!pendingConnection)
                PendingConnection();

            foreach(Connection connection in Connections.Values)
                switch(connection.State)
                {
                    case ConnectionState.OK:
                        connection.Update();
                        break;

                    case ConnectionState.Disconnect:
                        Connections.Remove(connection.Id);
                        OnClientDisconnected?.Invoke(connection);
                        break;
                }
        }

        private async void PendingConnection()
        {
            pendingConnection = true;

            Socket newSocket = await socket.AcceptAsync();

            Connection connection = new Connection(idCounter++, newSocket);
            connection.State = ConnectionState.Pending;

            Packet packet = Packet.Create(0);
            packet.WriteByte(connection.Id);
            connection.Send(packet);

            connection._onPacket += (Packet _packet) => OnPacket(connection, _packet);

            connection.State = ConnectionState.OK;
            Connections.Add(connection.Id, connection);    

            OnClientConnected?.Invoke(connection);

            pendingConnection = false;
        }

        public override void Close() =>
            socket.Close();
    }
}
