using Protocol.Dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacterControl : CharacterBase
{
    private void Awake()
    {
        Instance = this;
        _currentHealth = maxHealth;
        //Debug.Log("PlayerCharacterControl ���ǲ�����ë����");
        Bind(UIEvent.FIGHT_INFO_PANEL);
        //Debug.LogWarning("����Bindû��");
    }
    public override void Execute(int eventCode, object message)
    {
        //base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.FIGHT_INFO_PANEL:
                Debug.Log("Execute PlayerCharacterControl û��ִ��");
                DisplayName((UserDto)message);
                break;

            default:
                break;
        }
    }



    public static PlayerCharacterControl Instance;

    public Camera playerCamera;
    public float gravityDownForce = 10f; //���������ٶ�
    public float maxSpeedOnGround = 8f;  //�����ƶ��ٶ�
   
    //���
    public float runSpeedOnGround = 16;
    public float speed;//�ƶ��ٶ�
    public bool isRun;//�ж��Ƿ��ڱ���
    public bool isWalk;//�ж��Ƿ�������
    private KeyCode runInputName;//���ܼ�λ
    public float jumpForce = 5f;//��Ծ����
    public Vector3 velocity;//�������Y���һ�������仯
    private string jumpInputName = "Jump";//��Ծ��λ
    private bool isJump;//�ж��Ƿ�����Ծ
//public Transform groundCheck;//����������
    public LayerMask groundMask;
    private AudioSource audioSource; //����Դ��
    public AudioClip walkingSound; //����Ƭ��
    public AudioClip runingSound;


    public float moveSharpnessOnGround = 15f;  //��Ӧ�ٶ�

    public float rotationSpeed = 200f;  //�����ת�ٶ�

    public float cameraHeightRatio = 0.85f;  //��������ϵ���߱���
    public float maxHealth = 200f;

    private CharacterController characterController;
    private PlayerInputHandler playerInputHandler;
    private float targetCharacterHeight = 1.8f;  //Ŀ�����
    private float cameraVerticalAngle = 0f; //����������ֵ
    private float _currentHealth;
    private bool _isBossAttack;

    public float CurrentHealth => _currentHealth;

    public Vector3 characterVelocity;           //�ٶȵĹ�������


    public Text NameText;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        audioSource = GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;

        //�����ڷ�ֹ��ģ
        characterController.enableOverlapRecovery = true;

        UpdateCharacterHeight();


        DisplayName();
    }

    private void Update()
    {
        HandlerCharacterMovement();
    }

    private void UpdateCharacterHeight()
    {
        characterController.height = targetCharacterHeight;     //�������
        characterController.center = Vector3.up * characterController.height * 0.5f;//��������
        playerCamera.transform.localPosition = Vector3.up * characterController.height * cameraHeightRatio;//�������λ��

    }

    private void HandlerCharacterMovement()
    {
        isRun = Input.GetKey(runInputName);
        isWalk = (Mathf.Abs(playerInputHandler.GetMouseLookHorizontal()) > 0 || Mathf.Abs(playerInputHandler.GetMouseLookVertical()) > 0) ? true : false;

        speed = isRun ? runSpeedOnGround : maxSpeedOnGround;


        //camera rotate horizantal
        //ˮƽ������ת
        transform.Rotate(new Vector3(0, playerInputHandler.GetMouseLookHorizontal() * rotationSpeed, 0),
            Space.Self);
        //��ֱ������ת  cameraVerticalAngleһ��Ҫ�ۼӣ���ΪGetMouseLookVertical���ص���˲��ֵ
        cameraVerticalAngle += playerInputHandler.GetMouseLookVertical() * rotationSpeed;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -80f, 80f); //����ֱ��ת��������
        playerCamera.transform.localEulerAngles = new Vector3(-cameraVerticalAngle, 0, 0); //ע���Ƿ���ģ�����Ҫ����


        //move
        Vector3 wordSpaceMoveInput = transform.TransformVector(playerInputHandler.GetMoveInput());

        //�ж��Ƿ��ڵ���
        if (characterController.isGrounded)
        {
            //�ڵ���
            Vector3 targetVelocity = wordSpaceMoveInput * speed;

            //�����ڵ�ֵ�𲽹��ȵ��ƶ����ֵ
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, moveSharpnessOnGround * Time.deltaTime);
            
            Jump();
        }
        else
        {
            //�ڿ���  ���������
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }
        if (_isBossAttack)
        {
            characterVelocity += transform.forward * -5;

        }

        //�ṩ���и���CharacterController�������Ϸ������ƶ�
        characterController.Move(characterVelocity * Time.deltaTime);

        //������������
        /*characterController.Move(Vector3.down * characterController.height / 2 * Time.deltaTime);*/
        /*characterController.Move(velocity * Time.deltaTime);*/
        PlayFootStepSound();
    }

    public void Jump()
    {
        
        isJump = Input.GetButtonDown(jumpInputName);
        //ʩ����Ծ���� 
        if (isJump)//&& characterController.isGrounded
        {
            Debug.Log("Input Button Down Jump.");
            characterVelocity.y = Mathf.Sqrt(jumpForce * gravityDownForce);
            //velocity.y = 20f;
        }
    }

    ///�����ƶ�����Ч
    public void PlayFootStepSound()
    {
        if (characterController.isGrounded && characterVelocity.sqrMagnitude > 0.9f)
        {
            //Debug.LogError("run or walk");
            audioSource.clip = isRun ? runingSound : walkingSound;//�������߻��߱��ܵ���Ч
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        OnHitPlayer(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("EnemyBullet94");
        OnHitPlayer(other);
    }

    private void OnHitPlayer(Collider other)
    {
        //Debug.Log("EnemyBullet100");
        /*if (other.CompareTag("EnemyBullet"))
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            _currentHealth -= enemyBullet.damage;
            StartCoroutine(OnDamge());

            if (other.GetComponent<Rigidbody>())
            {
                Destroy(other.gameObject);
            }
        }*/

        if (other.CompareTag("MeleeArea"))
        {
            //MeleeAttacker meleeAttacker = other.GetComponent<MeleeAttacker>();
            //_currentHealth -= meleeAttacker.damage;
            _currentHealth -= 10;

            _isBossAttack = other.name == "BossMeleeArea";
            print("��ұ�����");
            StartCoroutine(OnDamge());
        }
    }

    IEnumerator OnDamge()
    {
        print("��ұ�����,����Ѫ��Ϊ��" + _currentHealth);
        if (_currentHealth <= 0)
        {
            OnDie();
        }
        yield return new WaitForSeconds(0.2f);
        _isBossAttack = false;

    }
    private void OnDie()
    {
        SceneManager.LoadScene("2.PVE");
    }

    private void DisplayName(UserDto userDto)
    {
        Debug.LogWarning("DisplayName");
        NameText.text = userDto.Name;
    }
    private void DisplayName()
    {
        Debug.LogWarning("DisplayName");
        NameText.text = "Name : "+Models.GameModel.UserDto.Name;
    }
}
