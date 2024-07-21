using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Obelisk.Networking
{
    public abstract class Peer
    {
        public abstract void Send(Packet packet);

        public abstract void Update();
        public abstract void Close();
    }
}
