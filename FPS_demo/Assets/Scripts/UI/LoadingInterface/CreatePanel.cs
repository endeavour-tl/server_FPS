using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreatePanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.CREATE_PANEL_ACTIVE);
    }
    
    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.CREATE_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;
            default:
                break;
        }
    }
    private InputField NameInputField;
    private Button ButtonCreate;
    private PromptMsg promptMessage;
    private SocketMessage socketMessage;
    // Start is called before the first frame update
    void Start()
    {
        NameInputField = transform.Find("NameInputField").GetComponent<InputField>();
        ButtonCreate = transform.Find("ButtonCreate").GetComponent<Button>();
        ButtonCreate.onClick.AddListener(createClick);

        promptMessage = new PromptMsg();
        socketMessage = new SocketMessage();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        ButtonCreate.onClick.RemoveListener(createClick);
    }

    private void createClick()
    {
        if (string.IsNullOrEmpty(NameInputField.text))
        {
            //�Ƿ�����
            promptMessage.Change("���Ʋ���Ϊ��", Color.red);
            Dispatch(AreaCode.UI, UIEvent.PROMPT_MSG, promptMessage);
            return;
        }



        //�����������һ������������
        socketMessage.Change(OpCode.USER, UserCode.CREATE_CREQ, NameInputField.text);
        Dispatch(AreaCode.NET, 0, socketMessage);
    }
}
