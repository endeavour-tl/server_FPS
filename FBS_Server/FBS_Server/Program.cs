using FBS_Server.AhpilyServer;
using FBS_Server.GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerPeer serverPeer = new ServerPeer();

            //指定所关联的应用
            serverPeer.SetApplication(new NetMsgCenter());

            serverPeer.Start(6669, 10);

            Console.ReadKey();
        }
    }
}
