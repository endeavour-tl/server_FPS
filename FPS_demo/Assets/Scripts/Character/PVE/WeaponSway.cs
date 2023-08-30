using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ҡ��
/// </summary>
public class WeaponSway : MonoBehaviour
{
    /*ҡ�ڵĲ���*/
    public float amout = 0.03f; //ҡ�ڷ���
    public float smoothAmout = 6f;//һ��ƽ��ֵ
    public float maxAmout = 0.06f;//���ҡ�ڷ���

    private Vector3 originPostion; //�ֱ۳�ʼλ��

    // Start is called before the first frame update
    void Start()
    {
        //����λ�ã�����ڸ�������任��λ�ã�
        originPostion = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //���������ֱ�ģ��λ�õ�ֵ��
        float movementX = -Input.GetAxis("Mouse X") * amout;
        float movementY = -Input.GetAxis("Mouse Y") * amout;
        //����
        movementX = Mathf.Clamp(movementX, -maxAmout, maxAmout);
        movementY = Mathf.Clamp(movementY, -maxAmout, maxAmout);

        Vector3 finallyPostion = new Vector3(movementX, movementY, 0);
        //�ֱ�λ�ñ任
        transform.localPosition = Vector3.Lerp(transform.localPosition, finallyPostion + originPostion, Time.deltaTime * smoothAmout);
        //Debug.LogWarning("�ֱ�ҡ��");
    }
}
