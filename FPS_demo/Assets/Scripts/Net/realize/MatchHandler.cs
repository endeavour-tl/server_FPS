using Protocol.Code;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MatchHandler : HandlerBase
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case MatchCode.ENTER_SRES:
                PVEFightResponse(value as UserDto);
                break;
        }
    }

    public void PVEFightResponse(UserDto userDto)
    {
        //保存服务器发来的角色数据
        //GameModel model = new GameModel();
        Models.GameModel.UserDto = userDto;
        //跳转场景
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(2, null));
        //Debug.LogError("PVEFightResponse is Error");
        Models.GameModel.UserDto = userDto;
        //Dispatch(AreaCode.CHARACTER, UIEvent.FIGHT_INFO_PANEL, userDto);
    }

}

