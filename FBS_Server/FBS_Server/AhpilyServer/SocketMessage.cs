using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer
{
    /// <summary>
    /// 网络消息
    /// 作用：发送的时候，都要发送这个类
    /// </summary>
    public class SocketMessage
    {
        /// <summary>
        /// 操作码
        /// </summary>
        public int opCode { get; set; }
        /// <summary>
        /// 子操作
        /// </summary>
        public int SubCode { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Value { get; set; }

        public SocketMessage()
        {

        }

        public SocketMessage(int opCode, int subCode, object value)
        {
            this.opCode = opCode;
            this.SubCode = subCode;
            this.Value = value;
        }
    }
}
