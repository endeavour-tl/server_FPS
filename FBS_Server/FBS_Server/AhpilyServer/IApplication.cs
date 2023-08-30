using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer
{
    public interface IApplication
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client"></param>
        void OnDisconnect(ClientPeer client);

        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="socketMessage"></param>
        void OnReceive(ClientPeer client, SocketMessage socketMessage);
    }
}
