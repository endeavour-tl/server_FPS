using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Dto
{
    /// <summary>
    /// 用户数据的传输模型
    /// </summary>
    [Serializable]
    public class UserDto
    {
        public string Id;//由于游戏满足不了需求 所以定义了这个id
        public string Name;//角色名字
        public int WinCount;//胜场
        public int LoseCount;//负场
        public int Lv;//等级

        public UserDto()
        {

        }

        public UserDto(string id, string name, int winCount, int loseCount, int lv)
        {
            this.Id = id;
            this.Name = name;
            this.WinCount = winCount;
            this.LoseCount = loseCount;
            this.Lv = lv;
        }
    }
}
