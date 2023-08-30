using FBS_Server.AhpilyServer.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS_Server.GameServer.Cache
{
    public static class Caches
    {
        //确保唯一性
        public static AccountCache Account { get; set; }
        public static UserCache User { get; set; }
        /*public static MatchCache Match { get; set; }
        public static FightCache Fight { get; set; }*/

        static Caches()
        {
            Account = new AccountCache();
            User = new UserCache();
            /*Match = new MatchCache();
            Fight = new FightCache();*/
        }
    }
}
