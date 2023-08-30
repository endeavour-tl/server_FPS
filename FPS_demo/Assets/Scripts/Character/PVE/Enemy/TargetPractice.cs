using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPractice : MonoBehaviour
{
    //添加
    public float maxHealth = 100f;
    public float _currentHealth;
    private MeshRenderer[] _meshes;  //获取3D模型数据
    private void Awake()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>();
        _currentHealth = maxHealth;
    }
    public void InflictDamage(float Damage)
    {
        _currentHealth -= 20;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        StartCoroutine(OnDamage());
    }

    IEnumerator OnDamage()
    {
        foreach (MeshRenderer mesh in _meshes)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(1f);
        if (_currentHealth > 0)
        {
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.material.color = Color.gray;
            }
            Destroy(gameObject);
        }
    }
}
