using Protocol.Code;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistPanel : UIBase
{
    /*private void Awake()
    {
        Debug.LogWarning("可能我是个假的Awake");
    }*/
    private void Awake()
    {
        //Debug.Log("你是不是有毛病！");
        Bind(UIEvent.SignUP_PANEL_ACTIVE);
    }

    //处理事件
    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SignUP_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;

            default:

                break;
        }
    }

    private Button btnRegist;
    private Button btnClose;
    private InputField inputAccount;
    private InputField inputPassword;
    private InputField InputRepeat;
    private PromptMsg promptMessage;
    SocketMessage socketMessage;
    // Start is called before the first frame update
    void Start()
    {
        btnRegist = transform.Find("RegistButton").GetComponent<Button>();
        btnClose = transform.Find("ButtonClose").GetComponent<Button>();
        inputAccount = transform.Find("AccountInputField").GetComponent<InputField>();
        inputPassword = transform.Find("PasswordInputField").GetComponent<InputField>();
        InputRepeat = transform.Find("PasswordAgainInputField").GetComponent<InputField>();
        promptMessage = new PromptMsg();
        socketMessage = new SocketMessage();

        btnRegist.onClick.AddListener(RegistClick);  //点击事件处理
        btnClose.onClick.AddListener(closeClick);

        //面板默认隐藏
        setPanelActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        btnRegist.onClick.RemoveAllListeners();  //清除事件处理
        btnClose.onClick.RemoveAllListeners();
    }

    //AccountDto accountDto = new AccountDto();


    private void RegistClick()
    {
        if (string.IsNullOrEmpty(inputAccount.text))
        {
            promptMessage.Change("输入的账号不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        if (string.IsNullOrEmpty(inputPassword.text))
        {
            promptMessage.Change("输入的密码不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        if (inputPassword.text.Length < 4
            || inputPassword.text.Length > 16)
        {
            promptMessage.Change("登入的密码长度应该在[4,16]!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        if (string.IsNullOrEmpty(InputRepeat.text)
            || inputPassword.text != InputRepeat.text)
        {
            promptMessage.Change("请确保两次输入的密码正确!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }

        //TODO 需要和服务器交互
        AccountDto dto = new AccountDto(inputAccount.text, inputPassword.text);
        socketMessage.Change(OpCode.ACCOUNT, AccountCode.REGIST_CREQ, dto);
        /*accountDto.Account = inputAccount.text;
        accountDto.password = inputPassword.text;
        socketMessage.opCode = OpCode.ACCOUNT;
        socketMessage.SubCode = AccountCode.REGIST_CREQ;
        socketMessage.value = accountDto;*/
        Dispatch(AreaCode.NET, 0, socketMessage);

        //注册成功返回
        setPanelActive(false);

    }

    private void closeClick()
    {
        setPanelActive(false);
    }
}
