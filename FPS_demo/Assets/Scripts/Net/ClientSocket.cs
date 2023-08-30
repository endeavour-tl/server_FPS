using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ClientSocket
{
    private Socket socket;

    private string ip;
    private int port;
    /// <summary>
    /// 构造连接socket对象
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口号</param>
    public ClientSocket(string ip, int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ip = ip;
            this.port = port;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void Connect()
    {
        try
        {
            socket.Connect(ip, port);
            Debug.Log("连接服务器成功>>>>>>>>>>>");
            //开始异步接受数据
            startReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    #region 接受数据
    //接受的数据缓冲区
    private byte[] receiveBuffter = new byte[1024];

    /// <summary>
    /// 一旦收到数据 就存到缓存区里面
    /// </summary>
    private List<byte> dataCache = new List<byte>();


    private bool isProcessReceive = false;

    public Queue<SocketMessage> socketMessagesQueue = new Queue<SocketMessage>();

    /// <summary>
    /// 开始异步接受数据
    /// </summary>
    private void startReceive()
    {
        if (socket == null && socket.Connected == false)
        {
            Debug.LogError("没有连接成功，无法发送数据>>>>>>>>>>>");
            return;
        }
        socket.BeginReceive(receiveBuffter, 0, 1024, SocketFlags.None, receiveCallBack, socket);
    }

    /// <summary>
    /// 收到消息回调
    /// </summary>
    private void receiveCallBack(IAsyncResult ar)
    {
        try
        {
            int lenght = socket.EndReceive(ar);
            byte[] tempByteArray = new byte[lenght];
            Buffer.BlockCopy(receiveBuffter, 0, tempByteArray, 0, lenght);
            //Debug.Log("处理收到的数据1");
            //处理收到的数据
            dataCache.AddRange(tempByteArray);
            //Debug.Log("处理收到的数据2");
            if (isProcessReceive == false)
            {
                //Debug.Log("processRecevie94");
                processRecevie();
            }
            //Debug.Log("处理收到的数据3");
            startReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 处理收到的数据
    /// </summary>
    private void processRecevie()
    {
        //Debug.Log("processRecevie110");
        isProcessReceive = true;
        //解析数据包
        byte[] data = EncodeTool.DecodePacket(ref dataCache);
        //Debug.Log("processRecevie114");
        if (data == null)
        {
            isProcessReceive = false;
            return;
        }
        //Debug.Log("processRecevie121");
        //需要再次转成一个具体的类型 供我们使用
        SocketMessage message = EncodeTool.DecodeMessage(data);
        //Debug.Log("processRecevie124");
        //存储消息 等待处理
        socketMessagesQueue.Enqueue(message);
        //Debug.Log(message.value);
        //尾递归
        processRecevie();
    }
    #endregion


    #region 发送数据

    /*public void Send(int opCode,int subCode,object value)
    {
        SocketMessage socketMessage = new SocketMessage(opCode, subCode, value);
        byte[] data = EncodeTool.EncodeMessage(socketMessage);
        byte[] packet = EncodeTool.EncodePacket(data);

        try
        {
            socket.Send(packet);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }*/

    public void Send(SocketMessage socketMessage)
    {
        byte[] data = EncodeTool.EncodeMessage(socketMessage);
        byte[] packet = EncodeTool.EncodePacket(data);

        try
        {
            socket.Send(packet);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    #endregion
}
