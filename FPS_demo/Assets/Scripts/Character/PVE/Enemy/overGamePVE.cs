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
        // ��ȡ Text ���
        PlayerNameText = transform.Find("PlayerNameText").GetComponent<Text>();
        gradeText = transform.Find("gradeText").GetComponent<Text>();
        WinCountText = transform.Find("WinCountText").GetComponent<Text>();
        LoseCountText = transform.Find("LoseCountText").GetComponent<Text>();
        CompleteButton = transform.Find("CompleteButton").GetComponent<Button>();

        //�Ӵ����ͷ����
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
    /// ˢ����ʾ
    /// </summary>
    private void refreshPanel()
    {
        //����Ϣ��������
        //Dispatch()
        //Dispatch(AreaCode.AUDIO, AudioEvent.PLAY_EFFECT_AUDIO, "nikandao");

        PlayerNameText.text = Models.GameModel.UserDto.Name;
        gradeText.text = Models.GameModel.UserDto.Lv.ToString();
        WinCountText.text = Models.GameModel.UserDto.WinCount.ToString();
        LoseCountText.text = Models.GameModel.UserDto.LoseCount.ToString();

        /*//�����������һ������������
        socketMessage.Change(OpCode.USER, UserCode.CREATE_CREQ, NameInputField.text);
        Dispatch(AreaCode.NET, 0, socketMessage);*/

        //�����������һ������������
        socketMessage.Change(OpCode.USER, UserCode.UPDATE_CREQ, Models.GameModel.UserDto);
        Dispatch(AreaCode.NET, 0, socketMessage);

    }

    private void OnClick()
    {
        //TODO �Գ���1���жϣ�����panel���

        //��ת����
        LoadSceneMsg msg = new LoadSceneMsg(1,
            delegate ()
            {
                //���������ȡ��Ϣ
                SocketMessage socketMsg = new SocketMessage(OpCode.USER, UserCode.GET_INFO_CREQ, null);
                Dispatch(AreaCode.NET, 0, socketMsg);
                        //Debug.Log("������ɣ�");
                    });
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, msg);


        /*Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(1, null));*/
    }
}
