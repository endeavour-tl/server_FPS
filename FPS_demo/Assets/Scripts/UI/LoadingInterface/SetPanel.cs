using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : UIBase
{
    private void Awake()
    {
        Bind(UIEvent.SET_PANEL_ACTIVE);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SET_PANEL_ACTIVE:
                setPanelActive((bool)message);
                break;
            default:
                break;
        }
    }

    private Button CloseButton;
    private Slider SliderVolume;
    private Button NextMusicButton;

    // Start is called before the first frame update
    void Start()
    {
        CloseButton = transform.Find("CloseButton").GetComponent<Button>();
        SliderVolume = transform.Find("SliderVolume").GetComponent<Slider>();
        NextMusicButton = transform.Find("NextMusicButton").GetComponent<Button>();

        CloseButton.onClick.AddListener(closeClick);
        NextMusicButton.onClick.AddListener(NextMuiscClick);
        SliderVolume.onValueChanged.AddListener(volumeValueChanged);

        //默认状态
        setPanelActive(false);

    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        CloseButton.onClick.RemoveListener(closeClick);
        NextMusicButton.onClick.RemoveListener(NextMuiscClick);
        SliderVolume.onValueChanged.RemoveListener(volumeValueChanged);
    }

    private void setClick()
    {
        setPanelActive(true);
    }

    private void closeClick()
    {
        setPanelActive(false);
    }

    private void NextMuiscClick()
    {
        //TODO 切换音乐
    }


    /// <summary>
    /// 当滑动条滑动的时候会调用
    /// </summary>
    /// <param name="value">滑动条的值</param>
    private void volumeValueChanged(float value)
    {
        //TODO 操作声音
        //Dispatch(AreaCode.AUDIO, AudioEvent.SET_BG_VOLUME, value);
    }
}
