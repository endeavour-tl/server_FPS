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
    /// ��������socket����
    /// </summary>
    /// <param name="ip">IP��ַ</param>
    /// <param name="port">�˿ں�</param>
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
            Debug.Log("���ӷ������ɹ�>>>>>>>>>>>");
            //��ʼ�첽��������
            startReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    #region ��������
    //���ܵ����ݻ�����
    private byte[] receiveBuffter = new byte[1024];

    /// <summary>
    /// һ���յ����� �ʹ浽����������
    /// </summary>
    private List<byte> dataCache = new List<byte>();


    private bool isProcessReceive = false;

    public Queue<SocketMessage> socketMessagesQueue = new Queue<SocketMessage>();

    /// <summary>
    /// ��ʼ�첽��������
    /// </summary>
    private void startReceive()
    {
        if (socket == null && socket.Connected == false)
        {
            Debug.LogError("û�����ӳɹ����޷���������>>>>>>>>>>>");
            return;
        }
        socket.BeginReceive(receiveBuffter, 0, 1024, SocketFlags.None, receiveCallBack, socket);
    }

    /// <summary>
    /// �յ���Ϣ�ص�
    /// </summary>
    private void receiveCallBack(IAsyncResult ar)
    {
        try
        {
            int lenght = socket.EndReceive(ar);
            byte[] tempByteArray = new byte[lenght];
            Buffer.BlockCopy(receiveBuffter, 0, tempByteArray, 0, lenght);
            //Debug.Log("�����յ�������1");
            //�����յ�������
            dataCache.AddRange(tempByteArray);
            //Debug.Log("�����յ�������2");
            if (isProcessReceive == false)
            {
                //Debug.Log("processRecevie94");
                processRecevie();
            }
            //Debug.Log("�����յ�������3");
            startReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// �����յ�������
    /// </summary>
    private void processRecevie()
    {
        //Debug.Log("processRecevie110");
        isProcessReceive = true;
        //�������ݰ�
        byte[] data = EncodeTool.DecodePacket(ref dataCache);
        //Debug.Log("processRecevie114");
        if (data == null)
        {
            isProcessReceive = false;
            return;
        }
        //Debug.Log("processRecevie121");
        //��Ҫ�ٴ�ת��һ����������� ������ʹ��
        SocketMessage message = EncodeTool.DecodeMessage(data);
        //Debug.Log("processRecevie124");
        //�洢��Ϣ �ȴ�����
        socketMessagesQueue.Enqueue(message);
        //Debug.Log(message.value);
        //β�ݹ�
        processRecevie();
    }
    #endregion


    #region ��������

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
