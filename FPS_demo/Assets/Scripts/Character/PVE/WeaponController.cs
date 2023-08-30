using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// �������
/// </summary>
public class WeaponController : CharacterBase
{
    public PlayerCharacterControl playerCharacterControl;
    public Transform shooterPoint;//�����λ�ã�ǹ�ڣ�
    public int range = 100;//�������
    public int bulletsMag = 29;//һ����ϻ����
    public int bulletLeft = 173;//����
    public int currentBullets;//��ǰ�ӵ���

    public ParticleSystem muzzleFlash;//ǹ�ڻ�����Ч
    public GameObject hitParticles;//�ӵ�����������Ч
    public GameObject bulletHole;//����
    public Light muzzleFlashLight;//ǹ�ڻ���ƹ�

    public float fireRate = 0.1f;//���٣�ԽС����ٶ�Խ��
    private float fireTimer;//��ʱ��
    private float SpreadFactor; //�����һ��ƫ����

    //���
    public GameObject Enemy_A;
    public GameObject Enemy_B;
    public GameObject Enemy_D;
    public GameObject Target;

    [Header("��λ����")]
    [SerializeField] [Tooltip("��װ�ӵ�����")] private KeyCode reloadInputName;
    [SerializeField] [Tooltip("�鿴��������")] private KeyCode inspectInputName;
    [SerializeField] [Tooltip("����������")] private KeyCode AutoRifleKey;
    [SerializeField] [Tooltip("����������")] private KeyCode HandGunKey;
    [SerializeField] [Tooltip("�Զ����Զ��л�����")] private KeyCode GunShootModelInputName;

    private Animator anim;
    /*��Ч����*/
    private AudioSource audioSource;
    public AudioClip AK47ShoundClip;/*ǹ����ЧƬ��*/
    public AudioClip reloadAmmoLeftClip;//���ӵ�1��ЧƬ��
    public AudioClip reloadOutOFAmmoClip;//���ӵ�2��ЧƬ�Σ���ǹ˨��

    private bool isReloading;//�ж��Ƿ���װ��
    private bool isAiming;//�ж��Ƿ�����׼

    public Transform casingSpawnPoint;//�ӵ����׳���λ��
    public Transform casingPrefab; //�ӵ���Ԥ����

    private Camera mainCamera;
    //public ChooseGunController CGC; //�����л��������ʵ��

    /*ʹ��ö������ȫ�Զ��Ͱ��Զ�ģʽ*/
    public enum ShootMode { AutoRifle, SemiGun };
    public ShootMode shootingMode;
    private bool GunShootInput; //����ȫ�Զ��Ͱ��Զ� ����ļ�λ���뷢���ı�
    private int modeNum = 1; //ģʽ�л���һ���м������1��ȫ�Զ�ģʽ��2�����Զ�ģʽ��
    private string shootModelName;

    /*UI������*/
    public Image crossHairUI;
    public Text ammoTextUI;
    public Text ShootModelTextUI;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        currentBullets = bulletsMag;
        mainCamera = Camera.main;
        shootingMode = ShootMode.AutoRifle; //AK47��ǹĬ����ȫ�Զ�ģʽ
        shootModelName = "ȫ�Զ�";
        UpdateAmmoUI();

    }

    private void Update()
    {
        //�л�ģʽ(ȫ�Զ��Ͱ��Զ�)
        if (Input.GetKeyDown(GunShootModelInputName) && modeNum != 1)
        {
            modeNum = 1;
            shootModelName = "ȫ�Զ�";
            shootingMode = ShootMode.AutoRifle;
            ShootModelTextUI.text = shootModelName;
        }
        else if (Input.GetKeyDown(GunShootModelInputName) && modeNum != 0)
        {
            modeNum = 0;
            shootModelName = "���Զ�";
            shootingMode = ShootMode.SemiGun;
            ShootModelTextUI.text = shootModelName;
        }

        /*�������ģʽ��ת��  �����Ҫ�ô���ȥ��̬������*/
        switch (shootingMode)
        {
            case ShootMode.AutoRifle:
                GunShootInput = Input.GetMouseButton(0);
                fireRate = 0.1f;
                break;
            case ShootMode.SemiGun:
                GunShootInput = Input.GetMouseButtonDown(0);
                fireRate = 0.2f;
                break;
        }


        if (GunShootInput && currentBullets > 0)
        {
            GunFire();
        }
        else
        {
            muzzleFlashLight.enabled = false;
        }

        //��ʱ����ʱ��
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }

        anim.SetBool("Run", playerCharacterControl.isRun);//�����ܲ�����
        anim.SetBool("Walk", playerCharacterControl.isWalk);
        //��ȡ����״̬����һ�㶯����״̬
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        //���ֻ��ӵ��Ķ���
        if (info.IsName("Reload Ammo Left") || info.IsName("Reload Out Of Ammo"))
        {
            isReloading = true;
        }
        else
        {
            isReloading = false;
        }

        if (Input.GetKeyDown(reloadInputName) && currentBullets < bulletsMag && bulletLeft > 0)
        {
            Reload();
        }
        SpreadFactor = (isAiming) ? 0f : 0.01f;
        DoingAim();

        if (Input.GetKeyDown(inspectInputName))
        {
            anim.SetTrigger("Inspect");
        }
        //�л����������Զ���ǹ��
        /*if (Input.GetKeyDown(AutoRifleKey))
        {
            CGC.ChangeWeapon(0);
        }
        //�л�����������ǹ��
        if (Input.GetKeyDown(HandGunKey))
        {
            CGC.ChangeWeapon(1);
        }*/

    }

    //����UI
    public void UpdateAmmoUI()
    {
        ammoTextUI.text = currentBullets + " / " + bulletLeft;
        ShootModelTextUI.text = shootModelName;
    }

    /// <summary>
    /// ��׼���߼�
    /// </summary>
    public void DoingAim()
    {
        if (Input.GetMouseButton(1) && !isReloading && !playerCharacterControl.isRun)// 
        {
            //��׼
            isAiming = true;
            anim.SetBool("Aim", isAiming);
            crossHairUI.gameObject.SetActive(false);
            mainCamera.fieldOfView = 25;//��׼��ʱ���������Ұ��С
        }
        else
        {
            //����׼
            isAiming = false;
            anim.SetBool("Aim", isAiming);
            crossHairUI.gameObject.SetActive(true);
            mainCamera.fieldOfView = 60;//��׼��ʱ���������Ұ�ָ�

        }


    }

    /// <summary>
    /// ����߼�
    /// </summary>
    public void GunFire()
    {
        //�������٣� ��ǰ���д����,����װ�ӵ�,���ڱ���  �Ͳ����Է�����
        if (fireTimer < fireRate || currentBullets <= 0 || isReloading || playerCharacterControl.isRun) return;   // 


        RaycastHit hit;
        Vector3 shootDirection = shooterPoint.forward; //�������
        //�ĳ������shootDirection shooterPoint�����Ϸ�������С��ƫ��(TransformDirection ��local����ת��Ϊ��������)
        shootDirection = shootDirection + shooterPoint.TransformDirection(new Vector3(Random.Range(-SpreadFactor, SpreadFactor), Random.Range(-SpreadFactor, SpreadFactor)));

        if (Physics.Raycast(shooterPoint.position, shootDirection, out hit, range))
        {
            Debug.Log(hit.transform.name + "����");

            if(hit.transform.name == "Enemy A")
            {
                Enemy_A.GetComponent<Damageable>().InflictDamage(20f);
            }
            if(hit.transform.name == "Enemy B")
            {
                Enemy_B.GetComponent<Damageable>().InflictDamage(20f);
            }

            if(hit.transform.name == "Enemy D")
            {
                Enemy_D.GetComponent<Damageable>().InflictDamage(20f);
            }

            if(hit.transform.name == "Target")
            {
                Target.GetComponent<TargetPractice>().InflictDamage(20f);
            }
            GameObject hitParticleEffect = Instantiate(hitParticles, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//ʵ�����ӵ����еĻ����Ч
            GameObject bulletHoleEffect = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//ʵ����������Ч
            //������Ч
            Destroy(hitParticleEffect, 1f);
            Destroy(bulletHoleEffect, 3f);
        }

        if (!isAiming)
        {
            anim.CrossFadeInFixedTime("Fire", 0.1f); //������ͨ���𶯻���ʹ�ö����ĵ�������Ч����
        }
        else
        {//��׼״̬�£�������׼�Ŀ��𶯻�
            anim.Play("Aim Fire", 0, 0f);
        }
        muzzleFlash.Play(); //���Ż����Ч
        muzzleFlashLight.enabled = true;
        PlayerShootSound();//���������Ч
        //ʵ���׵���
        Instantiate(casingPrefab, casingSpawnPoint.transform.position, casingSpawnPoint.transform.rotation);
        currentBullets--;
        UpdateAmmoUI();
        fireTimer = 0f;//���ü�ʱ��
    }


    public void PlayerShootSound()
    {
        audioSource.clip = AK47ShoundClip;
        audioSource.Play();
    }
    /// <summary>
    /// ��װ��ҩ�߼�
    /// </summary>
    public void Reload()
    {
        if (bulletLeft <= 0) return;
        DoReloadAnimation();

        //������Ҫ�����ӵ�
        int bulletToLoad = bulletsMag - currentBullets;
        //���㱸���۳����ӵ�
        int bulletToReduce = (bulletLeft >= bulletToLoad) ? bulletToLoad : bulletLeft;
        bulletLeft -= bulletToReduce; //��������
        currentBullets += bulletToReduce;//��ǰ�ӵ�����
        UpdateAmmoUI();
    }

    //����װ������
    public void DoReloadAnimation()
    {
        if (currentBullets > 0)
        {

            anim.Play("Reload Ammo Left", 0, 0);
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }

        if (currentBullets == 0)
        {
            anim.Play("Reload Out Of Ammo", 0, 0);
            audioSource.clip = reloadOutOFAmmoClip;
            audioSource.Play();
        }
    }

}

