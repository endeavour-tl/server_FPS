using Protocol.Code;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class overGamePVE : CharacterBase
{
    private Text PlayerNameText;
    private Text gradeText;
    private Text WinCountText;
    private Text LoseCountText;
    private Button CompleteButton;

    SocketMessage socketMessage;

    private void Start()
    {
        // 获取 Text 组件
        PlayerNameText = transform.Find("PlayerNameText").GetComponent<Text>();
        gradeText = transform.Find("gradeText").GetComponent<Text>();
        WinCountText = transform.Find("WinCountText").GetComponent<Text>();
        LoseCountText = transform.Find("LoseCountText").GetComponent<Text>();
        CompleteButton = transform.Find("CompleteButton").GetComponent<Button>();

        //接触、释放鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CompleteButton.onClick.AddListener(OnClick);
        socketMessage = new SocketMessage();
        refreshPanel();
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    private void refreshPanel()
    {
        //发消息播放音乐
        //Dispatch()
        //Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "nikandao");

        PlayerNameText.text = Models.GameModel.UserDto.Name;
        gradeText.text = Models.GameModel.UserDto.Lv.ToString();
        WinCountText.text = Models.GameModel.UserDto.WinCount.ToString();
        LoseCountText.text = Models.GameModel.UserDto.LoseCount.ToString();

        /*//向服务器发送一个创建的请求
        socketMessage.Change(OpCode.USER, UserCode.CREATE_CREQ, NameInputField.text);
        Dispatch(AreaCode.NET, 0, socketMessage);*/

        //向服务器发送一个创建的请求
        socketMessage.Change(OpCode.USER, UserCode.UPDATE_CREQ, Models.GameModel.UserDto);
        Dispatch(AreaCode.NET, 0, socketMessage);

    }

    private void OnClick()
    {
        //TODO 对场景1做判断，隐藏panel组件

        //跳转场景
        LoadSceneMsg msg = new LoadSceneMsg(1,
            delegate ()
            {
                //向服务器获取信息
                SocketMessage socketMsg = new SocketMessage(OpCode.USER, UserCode.GET_INFO_CREQ, null);
                Dispatch(AreaCode.NET, 0, socketMsg);
                        //Debug.Log("加载完成！");
                    });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);


        /*Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(1, null));*/
    }
}
