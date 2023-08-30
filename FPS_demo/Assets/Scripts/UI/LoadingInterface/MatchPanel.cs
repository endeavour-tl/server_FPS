using Protocol.Code;
using UnityEngine;
using UnityEngine.UI;

public class MatchPanel : UIBase
{
    /*private void Awake()
    {
        Bind(UIEvent.SHOW_ENTER_ROOM_BUTTON);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SHOW_ENTER_ROOM_BUTTON:
                btnEnter.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }*/

    private Button PVEButton;
    private Button PVPButton;
    private Button SetButton;

    //private SocketMsg socketMsg;

    // Use this for initialization
    void Start()
    {
        PVEButton = transform.Find("PVEButton").GetComponent<Button>();
        PVPButton = transform.Find("PVPButton").GetComponent<Button>();
        SetButton = transform.Find("SetButton").GetComponent<Button>();

        PVEButton.onClick.AddListener(PVEClick);
        PVPButton.onClick.AddListener(PVPClick);
        SetButton.onClick.AddListener(SetClick);

        //socketMsg = new SocketMsg();

        //Ĭ��״̬
        //setObjectsActive(false);
        //SetButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    /*void Update()
    {
        if (txtDes.gameObject.activeInHierarchy == false)
            return;

        timer += Time.deltaTime;
        if (timer >= intervalTime)
        {
            doAnimation();
            timer = 0f;
        }
    }*/

    public override void OnDestroy()
    {
        base.OnDestroy();

        PVEButton.onClick.RemoveListener(PVEClick);
        PVPButton.onClick.RemoveListener(PVPClick);
        SetButton.onClick.RemoveListener(SetClick);
    }

    private void SetClick()
    {
        //fix bug
        //Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, 2);

        //Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(2, null));
        Dispatch(AreaCode.UI, UIEvent.SET_PANEL_ACTIVE, true);
    }

    SocketMessage socketMessage = new SocketMessage();

    private void PVEClick()
    {
        //�����������ʼƥ�������
        //  ������ô��������ͻ��˱�����������˺�id�Ļ�������Ϣ��ʱ�򣬶��������id���͸�������
        /*socketMsg.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(true);

        //��ť���ص�
        this.PVEButton.interactable = (false);*/

        //��������������PVE���������
        socketMessage.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMessage);

        //Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(2, null));

    }

    private void PVPClick()
    {
        /*//������������뿪ƥ�������
        socketMsg.Change(OpCode.MATCH, MatchCode.LEAVE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(false);

        //��ť����ʾ
        this.PVEButton.interactable = true;*/
    }

    /// <summary>
    /// ���Ƶ��ƥ�䰴ť֮�����ʾ������
    /// </summary>
    private void setObjectsActive(bool active)
    {
        PVPButton.gameObject.SetActive(active);
    }

}
