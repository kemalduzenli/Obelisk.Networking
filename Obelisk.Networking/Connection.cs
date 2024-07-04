using System;
using System.Net;
using System.Net.Sockets;

namespace Obelisk.Networking
{


    public abstract class Connection
    {
        internal static class Utils
        {
            public static IPEndPoint GetEndPoint(string ip, int port)
            {
                IPHostEntry host = Dns.GetHostEntry(ip);
                return new IPEndPoint(host.AddressList[0], port);
            }
        }

        internal Socket _socket { get; private set; }

        public string ip { get; internal set; }
        public int port { get; internal set; }

        public bool isActive;

        public Connection() =>
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
        
        public void Send(Message message)
        {
            _socket.Send(message.GetBuffer());
        }

        public void Close()
        {
            _socket.Close();

            isActive = false;
        }

        public Connection(Socket socket)
        {
            IPEndPoint socketEndPoint = socket.RemoteEndPoint as IPEndPoint; 

            ip = socketEndPoint.Address.ToString(); 
            port = socketEndPoint.Port;

            _socket = socket;
        }
    }
}
