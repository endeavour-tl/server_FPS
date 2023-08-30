//using FBS_Server.AhpilyServer.Concurrent;
using FBS_Server.GameServer.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.AhpilyServer.Cache
{
    /// <summary>
    /// 账号缓存
    ///     简单来说：存账号的
    /// </summary>
    public class AccountCache
    {
        //TODO 读取mongoDB
        string hostURL;
        string dbName;
        MongoClient client;
        IMongoDatabase db;
        IMongoCollection<AccountModel> collection;

        public AccountCache()
        {
            this.hostURL  = "mongodb://127.0.0.1:27017";
            this.dbName  = "ProductsDB";
            this.client = new MongoClient(hostURL);
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<AccountModel>("Accounts");

            //insert
            //collection.InsertOne(new Products { Account = "12345", IID = 2, Password = "12345" });
            //update
            //collection.UpdateOne(p => p.Account == "12345", Builders<Products>.Update.Set(p =>p.Password,"1234"));
            //delete
            //collection.DeleteOne(p => p.Password == "1234");
            //collection.DeleteOne(p => p.IID == 2);
            //select
            var products = collection.Find(_ => true);//all the records
            foreach (var product in products.ToList())
            {
                Console.WriteLine($"Account = {product.Account},Password = {product.Password},Id = {product.Id}");
                accModelDict.Add(product.Account, product);
            }
        }

        /// <summary>
        /// 账号 对应的  账号数据模型
        /// </summary>
        private Dictionary<string, AccountModel> accModelDict = new Dictionary<string, AccountModel>();

        /// <summary>
        /// 是否存在账号
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        public bool IsExist(string account)
        {
            return accModelDict.ContainsKey(account);
        }

        /// <summary>
        /// 用来存储账号的id
        ///     后期 是 数据库来处理的
        /// </summary>
        //private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 创建账号数据模型信息
        /// </summary>
        /// <param name="accout">账号</param>
        /// <param name="password">密码</param>
        public void Create(string account, string password)
        {
            AccountModel model = new AccountModel(account, password);

            //TODO 将账号数据写入mongoDB
            //insert
            //collection.InsertOne(new Products { Account = "12345", IID = 2, Password = "12345" });
            collection.InsertOne(model);

            accModelDict.Add(model.Account, model);
        }

        /// <summary>
        /// 获取账号对应的数据模型
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountModel GetModel(string account)
        {
            return accModelDict[account];
        }

        /// <summary>
        /// 账号密码是否匹配
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsMatch(string account, string password)
        {
            AccountModel model = accModelDict[account];
            return model.Password == password;
        }

        /// <summary>
        /// 账号 对应 连接对象
        /// </summary>
        private Dictionary<string, ClientPeer> accClientDict = new Dictionary<string, ClientPeer>();
        private Dictionary<ClientPeer, string> clientAccDict = new Dictionary<ClientPeer, string>();

        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool IsOnline(string account)
        {
            return accClientDict.ContainsKey(account);
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool IsOnline(ClientPeer client)
        {
            return clientAccDict.ContainsKey(client);
        }

        /// <summary>
        /// 用户上线
        /// </summary>
        /// <param name="client"></param>
        /// <param name="account"></param>
        public void Online(ClientPeer client, string account)
        {
            if (!accClientDict.ContainsKey(account))
            {
                accClientDict.Add(account, client);
                clientAccDict.Add(client, account);
            }
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="client"></param>
        public void Offline(ClientPeer client)
        {
            string account = clientAccDict[client];
            clientAccDict.Remove(client);
            accClientDict.Remove(account);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="account"></param>
        public void Offline(string account)
        {
            ClientPeer client = accClientDict[account];
            accClientDict.Remove(account);
            clientAccDict.Remove(client);
        }

        /// <summary>
        /// 获取在线玩家的id
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public string GetId(ClientPeer client)
        {
            string account = clientAccDict[client];
            AccountModel model = accModelDict[account];
            return model.Id;
        }



    }
}
