using FBS_Server.AhpilyServer;
using FBS_Server.AhpilyServer.Cache;
using FBS_Server.GameServer.Cache;
using FBS_Server.GameServer.Model;
using FBS_Server.Logic;
using Protocol.Code;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.GameServer.Logic
{
    public class MatchHandler : IHandler
    {
        private UserCache userCache = Caches.User;
        private AccountCache accountCache = Caches.Account;
        public void OnDisconnect(ClientPeer client)
        {
            //throw new NotImplementedException();
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.ENTER_CREQ:
                    ResponseUserPVE(client);
                    break;

                default:
                    break;
            }
               
        }

        
        //客户端请求服务器响应，将客户端数据发送给客户端
        public void ResponseUserPVE(ClientPeer client)
        {
            string accountId = accountCache.GetId(client);
            string userId = userCache.GetId(accountId);
            //string clientID = userCache.GetId(client);


            UserModel model = userCache.GetModelByAccountId(accountId);
            UserDto dto = new UserDto(model.Id, model.Name, model.WinCount, model.LoseCount, model.Lv);
            client.Send(OpCode.MATCH, MatchCode.ENTER_SRES, dto);
        }



    }
}
