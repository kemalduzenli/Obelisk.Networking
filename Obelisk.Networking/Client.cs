using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Obelisk.Networking
{
    public class Client : Peer
    {
        public byte Id;
        private readonly Socket socket;

        public event Action<Packet> OnPacket;
        public event Action OnDisconnect;

        public string Ip { get; private set; }
        public ushort Port { get; private set; }

        public ConnectionState State { get; internal set; }

        private bool isPendingPacket;

        public Client()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(ushort port, string ip = "127.0.0.1")
        {
            Ip = ip;
            Port = port;

            IPAddress ipAddress = IPAddress.Parse(ip);

            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

            socket.Connect(endPoint);
        }

        public override void Send(Packet packet) =>
            socket.Send(packet.ToBytes(), packet.size + sizeof(ushort), SocketFlags.None);

        public override void Update()
        {
            try
            {
                while (socket.Available >= sizeof(ushort) && !isPendingPacket)
                {
                    byte[] headerBuffer = new byte[sizeof(ushort)];
                    socket.Receive(headerBuffer, SocketFlags.None);
                    ushort size = BitConverter.ToUInt16(headerBuffer, 0);

                    PendingPacket(size);
                }
            }
            catch
            {
                State = ConnectionState.Disconnect;
                OnDisconnect?.Invoke();
            }
        }

        private async void PendingPacket(ushort size)
        {
            isPendingPacket = true;

            byte[] buffer = new byte[size];
            await socket.ReceiveAsync(buffer, SocketFlags.None);

            Packet packet = Packet.FromBuffer(buffer, size);

            if(packet.type != 0) 
                OnPacket?.Invoke(packet);
            else if(State == ConnectionState.Pending)
            {
                Id = packet.ReadByte(); 
                State = ConnectionState.OK;
            }

            isPendingPacket = false;
        }

        public override void Close()
        {
            socket.Close();
        }
    }
}
