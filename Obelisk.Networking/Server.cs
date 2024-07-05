using System.Net;
using System.Net.Sockets;

namespace Obelisk.Networking
{
    public class Server : Connection
    {
        private List<Client> clients;
        private Dictionary<byte, Action<Message, Client>> messageListener;

        public Server() : base() 
        {
            clients = new List<Client>();   
            messageListener = new Dictionary<byte, Action<Message, Client>>();
        }

        public event Action<Client> onClientConnect;
        public event Action<Client> onClientDisconnect;

        public void AddListener(byte channel, Action<Message, Client> callback) =>
            messageListener[channel] = callback;

        public void Start(string ip, ushort port)
        {
            isActive = true;

            this.ip = ip;
            this.port = port;

            _socket.Bind(Utils.GetEndPoint(ip, port));
            _socket.Listen(10);

            while (isActive)
            {
                Client newClient = new Client(_socket.Accept());

                try
                {
                    Task.Run(() => HandleClient(newClient));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            } 

            _socket.Close();
        }

        public void SendAll(Message message)
        {
            byte[] buffer = message.GetBuffer();

            foreach (Client client in clients)
                client._socket.Send(buffer);
        }

        private void HandleClient(Client client)
        {
            onClientConnect?.Invoke(client);

            clients.Add(client);  

            while (client.isActive)
            {
                if(!isActive)
                    break;

                try
                {
                    Message msg = client.Receive();

                    if (messageListener.ContainsKey(msg.channelId))
                        messageListener[msg.channelId](msg, client);
                }
                catch (Exception)
                {
                    break;
                }
            }

            onClientDisconnect?.Invoke(client);

            client.Close();

            clients.Remove(client);
        }
    }
}
