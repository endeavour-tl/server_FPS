using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { Melee, Range, Boss }  //近战敌人和远程敌人
    public Type enemyType;

    public Transform target; //获得玩家的位置
    public BoxCollider meleeArea; //maleeArea boxcollider
    public GameObject bullet;//子弹预制体
    //添加
    public float maxHealth = 100f;
    public float currentHealth;

    protected Rigidbody _rigidbody;
    protected NavMeshAgent navMeshAgent;
    protected Animator _animator;
    protected Health _health;

    private float _targetRadius;//碰撞检测的球体半径
    private float _targetRange;//碰撞检测的范围
    
    private bool isChase;//判断是否在跟踪玩家
    private bool isAttack;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _health = GetComponent<Health>();
        //_boxCollider = GetComponentInChildren<BoxCollider>();
        //_animator = GameObject.Find("Hips").GetComponent<Animator>();
        if (enemyType != Type.Boss)
        {
            Invoke("ChaseStart", 2);  //2s之后ChaseStart被调用
        }
        /*Invoke("ChaseStart", 2);  //2s之后ChaseStart被调用*/
    }

    private void Update()
    {
        if (_health.isDead)
        {
            StopAllCoroutines();
        }
        //判断是否可用，即勾选上NavMeshAgent组件
        if (navMeshAgent.enabled && !_health.isDead && enemyType != Type.Boss)// 
        {
            //可以使用
            navMeshAgent.SetDestination(target.position);
            navMeshAgent.isStopped = !isChase;
        }
    }
    private void ChaseStart()
    {
        isChase = true;
        if (_animator)
        {
            //Debug.Log("2");
            _animator.SetBool("isWalk", true);//isWalk
        }
    }

    /// <summary>
    /// 把刚体的移动值和旋转值设为0
    /// </summary>
    private void FreeVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    /// <summary>
    /// FixedUpdate与物理时间同步 与刚体的操作放在此中
    /// </summary>
    private void FixedUpdate()
    {
        if (isChase)
        {
            FreeVelocity();
        }
        if (enemyType != Type.Boss)
        {
            Targeting();
        }
        /*Targeting();*/
    }
    /// <summary>
    /// 敌人到玩家距离的扫描
    /// </summary>
    private void Targeting()
    {
        if (enemyType == Type.Melee)
        {
            _targetRadius = 0.5f;
            _targetRange = 2f;
        }
        else if (enemyType == Type.Range)
        {
            _targetRadius = 0.5f;
            _targetRange = 10f;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _targetRadius,
            transform.forward, _targetRange, LayerMask.GetMask("Player"));
        if (hits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;

        _animator.SetBool("isAttack", true);

        if (enemyType == Type.Melee)
        {
            yield return new WaitForSeconds(0.2f);
            meleeArea.enabled = true;//MaleeArea

            yield return new WaitForSeconds(1f);
            meleeArea.enabled = false;

            yield return new WaitForSeconds(1f);
        }
        else if (enemyType == Type.Range)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
            Rigidbody rigidbodyBullet = instantBullet.GetComponent<Rigidbody>();
            rigidbodyBullet.velocity = transform.forward * 20f;
            yield return new WaitForSeconds(2f);
        }

        /*yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;//MaleeArea
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1f);*/

        isChase = true;
        isAttack = false;

        _animator.SetBool("isAttack", false);
    }
}
