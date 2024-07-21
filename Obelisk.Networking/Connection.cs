using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Obelisk.Networking
{
    public enum ConnectionState : byte { Pending, OK, Disconnect }

    public class Connection : Peer
    {
        public byte Id;
        private readonly Socket socket;

        internal event Action<Packet>? _onPacket;

        public ConnectionState State { get; internal set; }

        private bool isPendingPacket;

        public Connection(byte Id, Socket socket)
        {
            this.Id = Id;
            this.socket = socket;
        }
        
        public override void Send(Packet packet) =>
            socket.Send(packet.ToBytes(), packet.size + sizeof(ushort), SocketFlags.None);

        public override void Update()
        {
            try
            {
                while(socket.Available >= sizeof(ushort) && !isPendingPacket)
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
            }
        }

        private async void PendingPacket(ushort size)
        {
            isPendingPacket = true;

            byte[] buffer = new byte[size];
            await socket.ReceiveAsync(buffer, SocketFlags.None);

            Packet packet = Packet.FromBuffer(buffer, size);
            _onPacket?.Invoke(packet);

            isPendingPacket = false;    
        }

        public override void Close()
        {
            socket.Close();
        }
    }
}
