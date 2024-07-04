
namespace Obelisk.Networking
{
    public class Message
    {
        private static Message Singleton = new Message();   

        public byte channelId;
        public int lenght;

        private byte[] buffer = new byte[0];
        private List<byte[]> data = new List<byte[]>();

        public static Message Create(byte channelId)
        {
            Singleton.data.Clear();

            Singleton.channelId = channelId;    
            Singleton.lenght = 0;
            Singleton.buffer = new byte[0];

            return Singleton;
        }

        public static Message Load(byte[] buffer, int msgLenght)
        {
            Singleton.data.Clear();

            Singleton.channelId = 0;
            Singleton.lenght = msgLenght;
            Singleton.buffer = buffer;

            return Singleton;
        }

        public byte[] GetBuffer() =>
            buffer;
    }
}
