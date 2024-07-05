using System.Net;
using System.Net.Sockets;

namespace Obelisk.Networking
{
    public class Client : Connection
    {
        private Dictionary<byte, Action<Message>> messageListener = new();

        public ushort Id;
        private ushort receiveBufferLenght = 1024;

        public Client() : base() { }
        public Client(Socket socket) : base(socket) { }

        public event Action onConnect;
        public event Action onDisconnect;

        internal Message Receive()
        {
            byte[] buffer = new byte[receiveBufferLenght];

            int bytesRead = _socket.Receive(buffer);

            return Message.Load(buffer, bytesRead);
        }

        public void Connect(string ip, ushort port)
        {
            isActive = true;

            this.ip = ip;
            this.port = port;

            onConnect.Invoke();

            while (isActive)
            {
                try
                {
                    Message msg = Receive();

                    if (messageListener.ContainsKey(msg.channelId))
                        messageListener[msg.channelId](msg);
                }
                catch (Exception)
                {
                    break;
                }
            }

            onDisconnect.Invoke();

            Close();    
        }

        public void Disconnect()
        {
            _socket.Disconnect(false);
            Close();
        }
    }
}
