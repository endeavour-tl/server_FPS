using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public bool isLook;
    //public GameObject bullet;
    public Transform missilePortA;
    public Transform missilePortB;

    private Vector3 _lookVec;
    private BoxCollider _boxCollider;
    private Vector3 _jumpHitTarget;


    private void Awake()
    {
        Debug.LogWarning("boss Awake");
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        _health = GetComponent<Health>();
        _boxCollider = GetComponent<BoxCollider>();
        navMeshAgent.isStopped = true;

        StartCoroutine(MissileShot());
        //StartCoroutine(Think());
    }

    private void Update()
    {
        /*if (_health.isDead)
        {
            StopAllCoroutines();
            GameManager.Instance.RestartGame();
        }*/

        if (isLook)
        {
            float horizon = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _lookVec = new Vector3(horizon, 0, vertical) * 5f;
            transform.LookAt(target.position + _lookVec);
        }
        else
        {
            navMeshAgent.SetDestination(_jumpHitTarget);
        }
    }

    private IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = UnityEngine.Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                StartCoroutine(MissileShot());
                break;
            case 4:
                StartCoroutine(JumpHit());
                break;
        }
    }

    private IEnumerator MissileShot()
    {
        Debug.LogWarning("boss Awake");
        _animator.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        Debug.LogWarning("boss Awake77");
        GameObject instantMissileA = Instantiate(bullet, missilePortA.position, missilePortA.rotation);
        Bullet missileBossA = instantMissileA.GetComponent<Bullet>();
        //missileBossA.target = target;
        Debug.LogWarning("boss Awake77");
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(bullet, missilePortB.position, missilePortB.rotation);
        Bullet missileBossB = instantMissileB.GetComponent<Bullet>();
        //missileBossB.target = target;
        yield return new WaitForSeconds(2.5f);
        //StartCoroutine(Think());
        Debug.LogWarning("boss×Óµ¯·¢Éä");
        StartCoroutine(MissileShot());
    }

    private IEnumerator JumpHit()
    {
        _jumpHitTarget = target.position + _lookVec;
        isLook = false;
        _boxCollider.enabled = false;
        _animator.SetTrigger("doJumpHit");
        yield return new WaitForSeconds(2.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1f);
        isLook = true;
        navMeshAgent.isStopped = true;
        _boxCollider.enabled = true;

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());
    }
}
