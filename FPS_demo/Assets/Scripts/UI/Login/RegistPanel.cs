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
        Debug.LogWarning("�������Ǹ��ٵ�Awake");
    }*/
    private void Awake()
    {
        //Debug.Log("���ǲ�����ë����");
        Bind(UIEvent.SignUP_PANEL_ACTIVE);
    }

    //�����¼�
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

        btnRegist.onClick.AddListener(RegistClick);  //����¼�����
        btnClose.onClick.AddListener(closeClick);

        //���Ĭ������
        setPanelActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        btnRegist.onClick.RemoveAllListeners();  //����¼�����
        btnClose.onClick.RemoveAllListeners();
    }

    //AccountDto accountDto = new AccountDto();


    private void RegistClick()
    {
        if (string.IsNullOrEmpty(inputAccount.text))
        {
            promptMessage.Change("������˺Ų���Ϊ��!", Color.red);
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
        if (string.IsNullOrEmpty(InputRepeat.text)
            || inputPassword.text != InputRepeat.text)
        {
            promptMessage.Change("��ȷ�����������������ȷ!", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }

        //TODO ��Ҫ�ͷ���������
        AccountDto dto = new AccountDto(inputAccount.text, inputPassword.text);
        socketMessage.Change(OpCode.ACCOUNT, AccountCode.REGIST_CREQ, dto);
        /*accountDto.Account = inputAccount.text;
        accountDto.password = inputPassword.text;
        socketMessage.opCode = OpCode.ACCOUNT;
        socketMessage.SubCode = AccountCode.REGIST_CREQ;
        socketMessage.value = accountDto;*/
        Dispatch(AreaCode.NET, 0, socketMessage);

        //ע��ɹ�����
        setPanelActive(false);

    }

    private void closeClick()
    {
        setPanelActive(false);
    }
}
