using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer
{
    ///<summary>
    ///服务器端
    /// </summary>
    class ServerPeer
    {
        /// <summary>
        /// 服务器端socket对象
        /// </summary>
        private Socket socket;

        /// <summary>
        /// 限制客户端连接数量的信号量
        /// </summary>
        private Semaphore semaphore;

        /// <summary>
        /// 客户端对象连接池
        /// </summary>
        private ClientPeerPool clientPeerPool;

        /// <summary>
        /// 应用层
        /// </summary>
        private IApplication application;

        /// <summary>
        /// 设置应用层
        /// </summary>
        /// <param name="app"></param>
        public void SetApplication(IApplication app)
        {
            this.application = app;
        }

        public ServerPeer() { }

        ///<summary>
        ///用来开启服务器   
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port, int maxCount)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                semaphore = new Semaphore(maxCount, maxCount);

                //直接new出最大数量的连接对象
                clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer tempClientPeer = null;
                for (int i = 0; i < maxCount; i++)
                {
                    tempClientPeer = new ClientPeer();
                    tempClientPeer.receiveArgs.Completed += receive_Completed;
                    tempClientPeer.receiveComplated = receiveComplated;
                    tempClientPeer.sendDisconnect = Disconnect;
                    clientPeerPool.Enqueue(tempClientPeer);

                }

                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(maxCount);//监听连接的最大数量
                Console.WriteLine("服务器启动成功>>>>>>");
                startAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        //------------------------------------接收连接（start）----------------------------------------------//
        #region
        /// <summary>
        /// 开始接收客户端连接
        /// </summary>
        public void startAccept(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs == null)
            {
                socketAsyncEventArgs = new SocketAsyncEventArgs();
                //Completed用于完成异步操作的事件。
                socketAsyncEventArgs.Completed += accept_Completed;
            }

            //返回值判断异步事件是否执行完成，如果放回true 代表正在执行， 执行完毕后会触发
            //如果返回false 代表已经执行完毕 直接处理
            //使用异步的方式进行连接
            bool result = socket.AcceptAsync(socketAsyncEventArgs);
            if (!result)
            {
                processAccept(socketAsyncEventArgs);
            }
        }

        /// <summary>
        /// 接受连接请求异步事件完成时候触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="socketAsyncEventArgs"></param>
        private void accept_Completed(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            processAccept(socketAsyncEventArgs);
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void processAccept(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            //限制线程访问
            semaphore.WaitOne();

            //得到客户端的对象
            //Socket clientSocket = socketAsyncEventArgs.AcceptSocket;
            ClientPeer client = clientPeerPool.Dequeue();
            client.ClientSocket = socketAsyncEventArgs.AcceptSocket;
            //再进行保存处理

            Console.WriteLine("客户端连接成功" + client.ClientSocket.RemoteEndPoint.ToString());

            //开始接受客户端发来的数据
            startReceive(client);

            socketAsyncEventArgs.AcceptSocket = null;
            startAccept(socketAsyncEventArgs);
        }

        #endregion
        //------------------------------------接收连接（end）------------------------------------------------//



        //------------------------------------接收数据（start）----------------------------------------------//
        #region
        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <param name="client"></param>
        private void startReceive(ClientPeer client)
        {
            try
            {
                bool result = client.ClientSocket.ReceiveAsync(client.receiveArgs);
                if (!result)
                {
                    processReceive(client.receiveArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        /// <summary>
        /// 处理接收请求
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void processReceive(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            ClientPeer client = socketAsyncEventArgs.UserToken as ClientPeer;
            //判断网络消息是否连接成功
            if (client.receiveArgs.SocketError == SocketError.Success && client.receiveArgs.BytesTransferred > 0)
            {
                //拷贝数据到数组
                byte[] byteArray = new byte[client.receiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.receiveArgs.Buffer, 0, byteArray, 0, client.receiveArgs.BytesTransferred);

                //让客户端自身处理数据包  自身解析
                client.StartReceive(byteArray);

                //尾递归
                startReceive(client);
            }
            //如果没有传输的字节数  就代表断开连接  断开连接
            else if (client.receiveArgs.BytesTransferred == 0)
            {
                if (client.receiveArgs.SocketError == SocketError.Success)
                {
                    //客户端主动断开连接
                    Disconnect(client, "客户端主动断开连接");
                }
                else
                {
                    //网络异常
                    Disconnect(client, client.receiveArgs.SocketError.ToString());
                }
            }

        }

        /// <summary>
        /// 当接收完成时，触发事件
        /// </summary>
        /// <param name="socketAsyncEventArgs"></param>
        private void receive_Completed(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            processReceive(socketAsyncEventArgs);
        }

        /// <summary>
        /// 一条数据解析完成的处理
        /// </summary>
        /// <param name="client">对应的连接对象</param>
        /// <param name="value">解析出来的一个具体能使用的类型</param>
        private void receiveComplated(ClientPeer client, SocketMessage socketMessage)
        {
            //给应用层 让其使用
            application.OnReceive(client, socketMessage);
        }
        #endregion
        //------------------------------------接收数据（end）------------------------------------------------//

        //------------------------------------断开连接（start）----------------------------------------------//
        #region
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client">表示断开的客户端连接对象</param>
        /// <param name="reason">断开原因</param>
        public void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                //清空一些数据
                if (client == null)
                {
                    throw new Exception("当前指定的客户端连接对象为空，无法断开连接");
                }
                Console.WriteLine(client.ClientSocket.RemoteEndPoint.ToString() + "客户端断开连接 原因 : " + reason);
                //通知运用层 这个客户端断开连接
                application.OnDisconnect(client);

                client.Disconnect();
                //回收对象方便下次使用
                clientPeerPool.Enqueue(client);
                semaphore.Release();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
        }
        #endregion
        //------------------------------------断开连接（end）------------------------------------------------//
    }
}
