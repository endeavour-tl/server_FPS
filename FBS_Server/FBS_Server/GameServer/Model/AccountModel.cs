using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.GameServer.Model
{
    /// <summary>
    /// 账号的数据模型
    /// </summary>
    public class AccountModel
    {
        //TODO MongoDB读取的Id

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string Account { get; set; }
        public string Password { get; set; }

        //...创建日期 电话号码

        public AccountModel(string account, string password)
        {
            this.Account = account;
            this.Password = password;
        }
    }
}
