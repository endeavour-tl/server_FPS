using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : UIBase
{
    private Button buttonStart;
    private Button buttonRegist;

    void Start()
    {
        buttonStart = transform.Find("LoginButton").GetComponent<Button>();
        buttonRegist = transform.Find("RegistButton").GetComponent<Button>();
        //¶¯Ì¬×¢²á
        buttonStart.onClick.AddListener(startClick);
        buttonRegist.onClick.AddListener(SignUpClick);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        buttonStart.onClick.RemoveAllListeners();
        buttonRegist.onClick.RemoveAllListeners();
    }

    private void startClick()
    {
        //Debug.Log("12323");
        Dispatch(AreaCode.UI, UIEvent.START_PANEL_ACTIVE, true);
    }

    private void SignUpClick()
    {
        //Debug.Log("12323");
        Dispatch(AreaCode.UI, UIEvent.SignUP_PANEL_ACTIVE, true);
    } 
}
