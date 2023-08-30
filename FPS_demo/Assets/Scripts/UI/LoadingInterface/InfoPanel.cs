using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.REFRESH_INFO_PANEL);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.REFRESH_INFO_PANEL:
                UserDto user = message as UserDto;
                refreshPanel(user.Name, user.Lv);
                break;
            default:
                break;
        }
    }

    //private Image imgHead;
    private Text TextName;
    private Text TextLv;


    // Use this for initialization
    void Start()
    {
        TextName = transform.Find("TextName").GetComponent<Text>();
        TextLv = transform.Find("TextLv").GetComponent<Text>();
    }

    /// <summary>
    /// 刷新视图
    ///     有参数，包括 名字 等级 经验 豆子
    /// </summary>
    private void refreshPanel(string name, int lv)
    {
        TextName.text = name;
        TextLv.text = "Lv." + lv;
    }

}

