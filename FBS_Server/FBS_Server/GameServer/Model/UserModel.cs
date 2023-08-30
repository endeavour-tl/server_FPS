using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.GameServer.Model
{
    /// <summary>
    /// 角色数据模型
    /// </summary>
    public class UserModel
    {
        //TODO MongoDB读取的Id

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }//唯一id由MongoDB自动生成
        public string Name { get; set; }//角色名字

        public int WinCount { get; set; }//胜场
        public int LoseCount { get; set; }//负场

        public int Lv { get; set; }//等级

        //金币 游戏币...

        public string AccountId;//外键：与这个角色所关联的账号Id


        public UserModel(string name, string accountId)
        {
            this.Name = name;
            this.WinCount = 0;
            this.LoseCount = 0;
            this.AccountId = accountId;
            this.Lv = 1;
        }

    }
}
