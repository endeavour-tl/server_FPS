using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer.Concurrent
{
    /// <summary>
    /// 线程安全的int类型
    /// </summary>
    class ConcurrentInt
    {
        private int value;
        public ConcurrentInt(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// 添加并获取
        /// </summary>
        /// <returns></returns>
        public int Add_Get()
        {
            lock (this)
            {
                value++;
                return value;
            }
        }

        /// <summary>
        /// 减少并获取
        /// </summary>
        /// <returns></returns>
        public int SubGet()
        {
            lock (this)
            {
                value--;
                return value;
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public int Get()
        {
            return value;
        }
    }
}
