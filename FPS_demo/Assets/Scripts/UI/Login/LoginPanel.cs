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
        //Debug.Log("���ǲ�����ë����");
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

        btnLogin.onClick.AddListener(loginClick);  //����¼�����
        btnClose.onClick.AddListener(closeClick);

        //�����ҪĬ������
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
            promptMessage.Change("������û�������Ϊ��!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        if (string.IsNullOrEmpty(inputPassword.text))
        {
            promptMessage.Change("��������벻��Ϊ��!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        if (inputPassword.text.Length < 4
            || inputPassword.text.Length > 16)
        {
            promptMessage.Change("��������볤��Ӧ����[4,16]!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }
        //TODO ��Ҫ�ͷ���������

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
