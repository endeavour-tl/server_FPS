using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public bool isInstantDie;

    public float maxHealth = 100f;

    public bool isDead { get; set; }

    private float _currentHealth;

    private MeshRenderer[] _meshes;  //获取3D模型数据

    private Animator _animator;

    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = value;
    }
    private void Awake()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _currentHealth = maxHealth;
    }

    /// <summary>
    /// 扣血
    /// </summary>
    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        Debug.Log("Current Health: " + _currentHealth);
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
        else if (!isDead)
        {
            foreach (MeshRenderer mesh in _meshes)
            {
                mesh.material.color = Color.gray;
            }
            isDead = true;
            if (isInstantDie)
            {
                Destroy(gameObject);
            }
            else
            {
                _animator.SetTrigger("doDie");
                Destroy(gameObject, 3);
            }
            /*_animator.SetTrigger("doDie");
            Destroy(gameObject, 3);*/
        }
    }
}
