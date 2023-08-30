using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : CharacterBase
{
    public static PlayerInputHandler Instance;

    public float lookSensitivity = 1f; //
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //隐藏鼠标 让鼠标不可见
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public Vector3 GetMoveInput()
    {
        //x y z
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //归一化
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }

    public float GetMouseLookHorizontal()
    {
        return GetMouseLookAxis("Mouse X");
    }

    public float GetMouseLookVertical()
    {
        return GetMouseLookAxis("Mouse Y");
    }
    public float GetMouseLookAxis(string mouseInputName)
    {
        float i = Input.GetAxisRaw(mouseInputName);
        i *= lookSensitivity * 0.01f;
        return i;
    }

    public bool GetFireInputHeld()
    {
        return Input.GetButton("Fire");
    }
}
