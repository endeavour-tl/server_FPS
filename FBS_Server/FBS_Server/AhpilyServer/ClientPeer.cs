using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer
{
    public class ClientPeer
    {
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            this.receiveArgs = new SocketAsyncEventArgs();
            this.receiveArgs.UserToken = this;
            this.socketAsyncEventArgs = new SocketAsyncEventArgs();
            this.receiveArgs.SetBuffer(new byte[1024], 0, 1024);
            this.socketAsyncEventArgs.Completed += SendArgs_Completed;
        }

        #region   接收数据

        public delegate void ReceiveComplated(ClientPeer client, SocketMessage socketMessage);

        /// <summary>
        /// 一个消息解析完成的回调
        /// </summary>
        public ReceiveComplated receiveComplated;

        /// <summary>
        /// 一旦接收到数据 就存到缓存区
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        /// <summary>
        /// 接收的异步套接字请求
        /// </summary>
        public SocketAsyncEventArgs receiveArgs { get; set; }

        /// <summary>
        /// 是否正在处理接收的数据
        /// </summary>
        private bool isReceiveProcess = false;


        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if (!isReceiveProcess)
            {
                processReceive();
            }
        }

        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void processReceive()
        {
            isReceiveProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodeMessage(ref dataCache);

            if (data == null)
            {
                isReceiveProcess = false;
                return;
            }

            //需要再次转成一个具体的类型 供我们使用
            SocketMessage message = EncodeTool.DecodeMessage(data);

            //回调给上层
            if (receiveComplated != null)
            {
                receiveComplated(this, message);
            }
            //尾递归
            processReceive();
        }
        //粘包、拆包问题：解决策略：消息头和消息尾
        //比如发送的数据 ：12345
        /*void test()
        {
            *//*byte[] bt = Encoding.Default.GetBytes("12345");
            
            //怎么构造
            //消息头 :消息的长度 bt
            //消息尾 :具体的长度
            int length = bt.Length;
            byte[] bt1 = BitConverter.GetBytes(length);

            //得到的消息 : bt1 + bt


            //怎么读取
            //int length = 前四个字节转成int类型
            //然后读取这个长度的数据*//*
        }*/
        #endregion

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //清空数据
            dataCache.Clear();
            isReceiveProcess = false;
            //给发送数据那里预留的
            sendQueue.Clear();
            isSendProcess = false;
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();

            ClientSocket = null;
        }

        #endregion

        #region 发送数据

        /// <summary>
        /// 发送消息的队列
        /// </summary>
        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        private bool isSendProcess = false;
        /// <summary>
        /// 发送的异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs socketAsyncEventArgs;

        /// <summary>
        /// 发送的时候 发现断开连接的回调
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reason"></param>
        public delegate void SendDisconnect(ClientPeer client, string reason);

        public SendDisconnect sendDisconnect;




        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode, int subCode, object value)
        {
            SocketMessage socketMessage = new SocketMessage(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMessage(socketMessage);
            byte[] packet = EncodeTool.EncodePacket(data);

            Send(packet);
            /*//存入消息队列中
            sendQueue.Enqueue(packet);
            if (!isSendProcess)
            {
                send();
            }*/
        }

        public void Send(byte[] packet)
        {
            //存入消息队列中
            sendQueue.Enqueue(packet);
            if (!isSendProcess)
            {
                send();
            }
        }

        /// <summary>
        /// 处理发送的消息
        /// </summary>
        private void send()
        {
            isSendProcess = true;

            //如果数据的条数等于0的话，就停止发送
            if (sendQueue.Count == 0)
            {
                isSendProcess = false;
                return;
            }
            //去除一条数据
            byte[] packet = sendQueue.Dequeue();
            //设置消息  发送的异步套接字操作  的发送数据缓存区
            socketAsyncEventArgs.SetBuffer(packet, 0, packet.Length);
            bool result = ClientSocket.SendAsync(socketAsyncEventArgs);
            if (result == false)
            {
                sendComplated();
            }
        }

        private void SendArgs_Completed(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            sendComplated();
        }

        /// <summary>
        /// 当异步发送请求完成的时候调用
        /// </summary>
        private void sendComplated()
        {
            //发送的有没有错误
            if (socketAsyncEventArgs.SocketError != SocketError.Success)
            {
                //发送出错了   客户端断开连接
                sendDisconnect(this, socketAsyncEventArgs.SocketError.ToString());
            }
            else
            {
                send();
            }
        }

        #endregion
    }
}
