
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer
{
    /// <summary>
    /// 关于编码的工具类
    /// </summary>
    public static class EncodeTool
    {
        //------------------------------------粘包、拆包问题 封装一个有规定的数据包（start）----------------------------------------------//
        #region
        /// <summary>
        /// 构造消息体：消息头 + 消息尾
        /// </summary>
        /// <param name="data">传入的消息</param>
        /// <returns></returns>
        public static byte[] EncodePacket(Byte[] data)
        {
            //内存流对象
            MemoryStream ms = new MemoryStream();
            //二进制写入对象
            BinaryWriter bw = new BinaryWriter(ms);
            //先写入长度
            bw.Write(data.Length);
            //再写入消息
            bw.Write(data);

            Byte[] byteArray = new byte[(int)ms.Length];
            //将指定数目的字节从起始于特定偏移量的源数组复制到起始于特定偏移量的目标数组。ms---->byteArray
            Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);
            ms.Close();
            bw.Close();
            return byteArray;
        }

        /// <summary>
        /// 解析消息体  从缓存取出一个个完整的数据包
        /// </summary>
        /// <param name="dataCache">消息块</param>
        /// <returns></returns>
        public static byte[] DecodeMessage(ref List<byte> dataCache)        //ref相当于c++的引用
        {
            //四个字节 构成一个int长度  不能构成一个完整的消息
            if (dataCache.Count < 4)
            {
                return null;
                //throw new Exception("数据缓存长度不足4，不能构成一个完整的消息");
            }

            //此中的using关键字可以关闭创建的对象
            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    //length:1111 position:11111111
                    int lenght = br.ReadInt32();
                    int dataRemainLength = (int)(ms.Length - ms.Position);
                    if (lenght > dataRemainLength)
                    {
                        return null;
                        //throw new Exception("数据长度不够包头约定的长度 不能构成一个完整的消息");
                    }
                    Byte[] data = br.ReadBytes(lenght);
                    //更新一下数据缓存
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dataRemainLength));
                    return data;
                }
            }
        }

        #endregion
        //------------------------------------粘包、拆包问题 封装一个有规定的数据包（end）------------------------------------------------//

        //------------------------------------构造发送的SocketMessage类（start）----------------------------------------------//
        #region
        /// <summary>
        /// 把socketMessage类转换成数组 发送出去
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static byte[] EncodeMessage(SocketMessage message)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(message.opCode);
            bw.Write(message.SubCode);
            //如果不等于空 才需要将object转换为字节数组存起来
            if (message.Value != null)
            {
                byte[] valuesBytes = encodeObject(message.Value);

                bw.Write(valuesBytes);


            }
            byte[] data = new byte[ms.Length];

            Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return data;
        }

        /// <summary>
        /// 将收到的字节数据转换成socketMessage对象  供我们使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SocketMessage DecodeMessage(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            SocketMessage socketMessage = new SocketMessage();
            socketMessage.opCode = br.ReadInt32();
            socketMessage.SubCode = br.ReadInt32();
            //还有剩余的字节没有读取 代表value 有值
            if (ms.Length > ms.Position)
            {
                byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                object value = DecodeObject(valueBytes);
                socketMessage.Value = value;
            }
            br.Close();
            ms.Close();
            return socketMessage;
        }
        #endregion
        //------------------------------------构造发送的SocketMessage类（end）------------------------------------------------//

        //------------------------------------把一个object类转换为byte[]（start）----------------------------------------------//
        #region

        /// <summary>
        /// 序列化对象 
        /// </summary>
        /// <param name="value">转化数据</param>
        /// <returns></returns>
        public static byte[] encodeObject(object value)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            //将对象或具有指定顶级 （根）、 对象图序列化到给定的流。
            bf.Serialize(ms, value);
            byte[] valueBytes = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, valueBytes, 0, (int)valueBytes.Length);
            ms.Close();
            return valueBytes;
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="valueBytes"></param>
        /// <returns></returns>
        public static object DecodeObject(byte[] valueBytes)
        {
            using (MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }
        #endregion
        //------------------------------------把一个object类转换尾byte[]（end）------------------------------------------------//
    }
}
