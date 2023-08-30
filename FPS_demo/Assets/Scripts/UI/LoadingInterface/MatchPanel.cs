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

        //默认状态
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
        //向服务器发起开始匹配的请求
        //  可以怎么做，假如客户端保存了自身的账号id的话，发消息的时候，都把自身的id发送给服务器
        /*socketMsg.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(true);

        //按钮隐藏掉
        this.PVEButton.interactable = (false);*/

        //向服务器发起进入PVE房间的请求
        socketMessage.Change(OpCode.MATCH, MatchCode.ENTER_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMessage);

        //Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(2, null));

    }

    private void PVPClick()
    {
        /*//向服务器发起离开匹配的请求
        socketMsg.Change(OpCode.MATCH, MatchCode.LEAVE_CREQ, null);
        Dispatch(AreaCode.NET, 0, socketMsg);

        setObjectsActive(false);

        //按钮隐显示
        this.PVEButton.interactable = true;*/
    }

    /// <summary>
    /// 控制点击匹配按钮之后的显示的物体
    /// </summary>
    private void setObjectsActive(bool active)
    {
        PVPButton.gameObject.SetActive(active);
    }

}
