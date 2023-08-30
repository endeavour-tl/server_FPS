using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { Melee, Range, Boss }  //��ս���˺�Զ�̵���
    public Type enemyType;

    public Transform target; //�����ҵ�λ��
    public BoxCollider meleeArea; //maleeArea boxcollider
    public GameObject bullet;//�ӵ�Ԥ����
    //���
    public float maxHealth = 100f;
    public float currentHealth;

    protected Rigidbody _rigidbody;
    protected NavMeshAgent navMeshAgent;
    protected Animator _animator;
    protected Health _health;

    private float _targetRadius;//��ײ��������뾶
    private float _targetRange;//��ײ���ķ�Χ
    
    private bool isChase;//�ж��Ƿ��ڸ������
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
            Invoke("ChaseStart", 2);  //2s֮��ChaseStart������
        }
        /*Invoke("ChaseStart", 2);  //2s֮��ChaseStart������*/
    }

    private void Update()
    {
        if (_health.isDead)
        {
            StopAllCoroutines();
        }
        //�ж��Ƿ���ã�����ѡ��NavMeshAgent���
        if (navMeshAgent.enabled && !_health.isDead && enemyType != Type.Boss)// 
        {
            //����ʹ��
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
    /// �Ѹ�����ƶ�ֵ����תֵ��Ϊ0
    /// </summary>
    private void FreeVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
    /// <summary>
    /// FixedUpdate������ʱ��ͬ�� �����Ĳ������ڴ���
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
    /// ���˵���Ҿ����ɨ��
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
