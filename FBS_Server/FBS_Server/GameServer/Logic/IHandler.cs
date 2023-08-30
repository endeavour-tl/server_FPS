using FBS_Server.AhpilyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.Logic
{
    public interface IHandler
    {
        void OnReceive(ClientPeer client, int subCode, object value);

        void OnDisconnect(ClientPeer client);

    }
}
