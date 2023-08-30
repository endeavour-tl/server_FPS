using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObjectRecord: CharacterBase
{
    //单例模式
    public static ObjectRecord Instance = null;

    public Health Enemy_A;
    public Health Enemy_B;
    //public PlayerCharacterControl player;
    /// <summary>
    /// 判断结果情况
    /// </summary>
    public enum GameOver { PlayerDie, EnemyDie }

    public GameOver GAMEOVERTYPE;

    public bool isAdd = false; //false 表示没有显示

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Enemy_A.isDead && Enemy_B.isDead && !isAdd)
        {
            isAdd = true;
            GAMEOVERTYPE = GameOver.EnemyDie;
            StartCoroutine(over());
        }
        
    }

    IEnumerator over()
    {
        Models.GameModel.UserDto.Lv = Models.GameModel.UserDto.Lv + 1;
        Models.GameModel.UserDto.WinCount = Models.GameModel.UserDto.WinCount + 1;
        //Models.GameModel.UserDto.LoseCount = Models.GameModel.UserDto.LoseCount + 1;
        yield return new WaitForSeconds(10f);
        Dispatch(AreaCode.SCENE, SceneEvent.LOAD_SCENE, new LoadSceneMsg(4, null));
        //StopAllCoroutines();
    }
}
