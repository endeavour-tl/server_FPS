using Protocol.Code;
using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : UIBase
{
    private Button btnLogin;
    private Button btnClose;
    private InputField inputAccount;
    private InputField inputPassword;

    private PromptMsg promptMessage;

    private void Awake()
    {
        //Debug.Log("你是不是有毛病！");
        Bind(UIEvent.START_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.START_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;

            default:

                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        btnLogin = transform.Find("LoginButton").GetComponent<Button>();
        btnClose = transform.Find("ButtonClose").GetComponent<Button>();
        inputAccount = transform.Find("AccountInputField").GetComponent<InputField>();
        inputPassword = transform.Find("PasswordInputField").GetComponent<InputField>();
        promptMessage = new PromptMsg();

        btnLogin.onClick.AddListener(loginClick);  //点击事件处理
        btnClose.onClick.AddListener(closeClick);

        //面板需要默认隐藏
        setPanelActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        btnClose.onClick.RemoveAllListeners();
        btnLogin.onClick.RemoveAllListeners();
    }

    private void loginClick()
    {
        if (string.IsNullOrEmpty(inputAccount.text))
        {
            promptMessage.Change("登入的用户名不能为空!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        if (string.IsNullOrEmpty(inputPassword.text))
        {
            promptMessage.Change("登入的密码不能为空!", Color.red);
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
        //TODO 需要和服务器交互

        AccountDto accountDto = new AccountDto(inputAccount.text, inputPassword.text);
        SocketMessage socketMessage = new SocketMessage(OpCode.ACCOUNT, AccountCode.LOGIN, accountDto);
        Dispatch(AreaCode.NET, 0, socketMessage);


    }

    private void closeClick()
    {
        //Debug.Log("masdas");
        setPanelActive(false);
    }
}
