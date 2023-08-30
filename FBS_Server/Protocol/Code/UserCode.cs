﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    /// <summary>
    /// 有关用户的一些操作码
    /// </summary>
    public class UserCode
    {
        //获取信息
        public const int GET_INFO_CREQ = 0;
        public const int GET_INFO_SRES = 1;

        //创建角色
        public const int CREATE_CREQ = 2;
        public const int CREATE_SRES = 312123;

        public const int UPDATE_CREQ = 5;
        public const int UPDATE_SRES = 6;

        //角色上线
        public const int ONLINE_CREQ = 3;
        public const int ONLINE_SRES = 4;
    }
}
