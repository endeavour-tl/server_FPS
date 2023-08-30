using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer
{
    /// <summary>
    /// 客户端连接池
    /// 作用：重用客户端连接对象
    /// </summary>
    class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue;
        public ClientPeerPool(int capacity)
        {
            //设置最大容积
            clientPeerQueue = new Queue<ClientPeer>(capacity);
        }

        public void Enqueue(ClientPeer client)
        {
            clientPeerQueue.Enqueue(client);
        }

        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }
    }
}
