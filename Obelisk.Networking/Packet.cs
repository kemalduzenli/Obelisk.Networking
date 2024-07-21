using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obelisk.Networking
{
    public struct Packet
    {
        public byte type { get => buffer[0]; }

        public ushort size { get; private set; }
        internal byte[] buffer { get; set; }

        private int writePosition, readPosition;

        public static Packet Create(byte type, int maxSize = 1024)
        {
            Packet packet = new Packet()
            {
                buffer = new byte[maxSize]
            };

            packet.WriteByte(type); 
                
            return packet;
        }

        internal static Packet FromBuffer(byte[] buffer, ushort size)
        {
            Packet packet = new Packet()
            {
                size = size,
                buffer = buffer,
                readPosition = 1
            };

            return packet;
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(size).Concat(buffer).ToArray();
        }

        #region Write

        public void WriteByte(byte value) 
        {
            if(writePosition <= buffer.Length)
            {
                buffer[writePosition++] = value;
            }
        }

        public void WriteBytes(byte[] values) 
        {
            if (writePosition + values.Length <= buffer.Length)
            {
                Array.Copy(values, 0, buffer, writePosition, values.Length);
                writePosition += values.Length;
            }
        }

        public void WriteInt(int value) =>
            WriteBytes(BitConverter.GetBytes(value));

        public void WriteFloat(float value) =>
            WriteBytes(BitConverter.GetBytes(value));

        public void WriteBool(bool value) =>
            WriteByte(value ? (byte)1 : (byte)0);

        public void WriteString(string value) 
        {
            byte[] data = Encoding.UTF8.GetBytes(value);
            WriteInt(data.Length);
            WriteBytes(data);
        }

        #endregion

        #region Read

        public byte ReadByte() 
        {
            if (readPosition < buffer.Length)
                return buffer[readPosition++];

            return 0;
        }

        public byte[] ReadBytes(int count) 
        {
            if (readPosition + count <= buffer.Length)
            {
                byte[] result = new byte[count];
                Array.Copy(buffer, readPosition, result, 0, count);
                readPosition += count;
                return result;
            }

            return new byte[0]; 
        }
        public int ReadInt() =>
            BitConverter.ToInt32(ReadBytes(sizeof(int)), 0);
        
        public float ReadFloat() =>
            BitConverter.ToSingle(ReadBytes(sizeof(float)), 0);
        
        public bool ReadBool() =>
            ReadByte() == 1;

        public string ReadString() 
        {
            int size = ReadInt();

            return Encoding.UTF8.GetString(ReadBytes(size));
        }

        #endregion
    }

}
