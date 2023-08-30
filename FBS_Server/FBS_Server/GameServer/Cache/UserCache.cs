using FBS_Server.AhpilyServer;
//using FBS_Server.AhpilyServer.Concurrent;
using FBS_Server.GameServer.Model;
using MongoDB.Driver;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.GameServer.Cache
{
    /// <summary>
    /// 角色数据缓存层
    /// </summary>
    public class UserCache
    {
        //TODO 读取mongoDB
        string hostURL;
        string dbName;
        MongoClient client;
        IMongoDatabase db;
        IMongoCollection<UserModel> collection;
        
        public UserCache()
        {
            this.hostURL = "mongodb://127.0.0.1:27017";
            this.dbName = "ProductsDB";
            this.client = new MongoClient(hostURL);
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<UserModel>("User");

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
                Console.WriteLine($"Id = {product.Id}, Name = {product.Name}," +
                    $"WinCount = {product.WinCount},LoseCount = {product.LoseCount}, " +
                    $"Lv = {product.Lv},AccountId = {product.AccountId}");
                idModelDict.Add(product.Id, product);
                accIdUIdDict.Add(product.AccountId, product.Id);
            }
        }


        /// <summary>
        /// 角色id  对应的  角色数据模型
        /// </summary>
        private Dictionary<string, UserModel> idModelDict = new Dictionary<string, UserModel>();

        /// <summary>
        /// 账号id  对应的 角色id 
        /// </summary>
        private Dictionary<string, string> accIdUIdDict = new Dictionary<string, string>();
        //ConcurrentDictionary

        /// <summary>
        /// 作为角色的id
        /// </summary>
        //ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="name">角色名</param>
        /// <param name="accountId">账号id</param>
        public void Create(string name, string accountId)
        {
            UserModel model = new UserModel(name, accountId);

            //TODO mongoDB
            //insert
            //collection.InsertOne(new Products { Account = "12345", IID = 2, Password = "12345" });
            collection.InsertOne(model);

            //保存到字典里
            idModelDict.Add(model.Id, model);
            accIdUIdDict.Add(model.AccountId, model.Id);
        }

        /// <summary>
        /// 判断此账号下是否有角色
        /// </summary>
        public bool IsExist(string accountId)
        {
            return accIdUIdDict.ContainsKey(accountId);
        }

        /// <summary>
        /// 根据账号id获取角色数据模型
        /// </summary>
        public UserModel GetModelByAccountId(string accountId)
        {
            string userId = accIdUIdDict[accountId];
            UserModel model = idModelDict[userId];
            return model;
        }

        /// <summary>
        /// 根据账号id获取角色id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public string GetId(string accountId)
        {
            return accIdUIdDict[accountId];
        }

        //存储 在线玩家 只有在线玩家 才有 这个（ClientPeer）对象 
        private Dictionary<string, ClientPeer> idClientDict = new Dictionary<string, ClientPeer>();
        private Dictionary<ClientPeer, string> clientIdDict = new Dictionary<ClientPeer, string>();

        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="clientPeer">客户端连接对象</param>
        /// <returns></returns>
        public bool IsOnline(ClientPeer client)
        {
            return clientIdDict.ContainsKey(client);
        }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline(string id)
        {
            return idClientDict.ContainsKey(id);
        }

        /// <summary>
        /// 角色上线
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        public void Online(ClientPeer client, string id)
        {
            if (!idClientDict.ContainsKey(id))
            {
                idClientDict.Add(id, client);
                clientIdDict.Add(client, id);
            }
        }

        /// <summary>
        /// 更新角色数据
        /// </summary>
        /// <param name="model"></param>
        public void Update(UserModel model)
        {
            idModelDict[model.Id] = model;

            //修改monogoDB数据
            //update
            //collection.UpdateOne(p => p.Account == "12345", Builders<Products>.Update.Set(p =>p.Password,"1234"));
            // 创建更新操作
            var filter = Builders<UserModel>.Filter.Eq(p => p.Id, model.Id);
            var update = Builders<UserModel>.Update
                .Set(p => p.Lv, model.Lv)
                .Set(p => p.WinCount, model.WinCount)
                .Set(p => p.LoseCount, model.LoseCount);
            // 执行更新操作
            var result = collection.UpdateOne(filter, update);
        }

        /// <summary>
        /// 角色下线
        /// </summary>
        /// <param name="client"></param>
        public void Offline(ClientPeer client)
        {
            string id = clientIdDict[client];
            clientIdDict.Remove(client);
            idClientDict.Remove(id);
        }

        /// <summary>
        /// 根据连接对象获取角色模型
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public UserModel GetModelByClientPeer(ClientPeer client)
        {
            string id = clientIdDict[client];
            UserModel model = idModelDict[id];
            return model;
        }

        /// <summary>
        /// 根据用户id获取数据模型
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public UserModel GetModelById(string userId)
        {
            UserModel user = idModelDict[userId];
            return user;
        }

        /// <summary>
        /// 根据角色id获取连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientPeer GetClientPeer(string id)
        {
            return idClientDict[id];
        }

        /// <summary>
        /// 根据在线玩家的连接对象 获取 角色id
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public string GetId(ClientPeer client)
        {
            if (!clientIdDict.ContainsKey(client))
            {
                throw new IndexOutOfRangeException("这个玩家不在在线的字典里面存储！");
            }
            return clientIdDict[client];
        }
         
    }
}
