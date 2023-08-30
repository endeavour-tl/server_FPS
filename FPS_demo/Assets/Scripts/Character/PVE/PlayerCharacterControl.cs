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
        //Debug.Log("PlayerCharacterControl 你是不是有毛病！");
        Bind(UIEvent.FIGHT_INFO_PANEL);
        //Debug.LogWarning("运行Bind没有");
    }
    public override void Execute(int eventCode, object message)
    {
        //base.Execute(eventCode, message);
        switch (eventCode)
        {
            case UIEvent.FIGHT_INFO_PANEL:
                Debug.Log("Execute PlayerCharacterControl 没有执行");
                DisplayName((UserDto)message);
                break;

            default:
                break;
        }
    }



    public static PlayerCharacterControl Instance;

    public Camera playerCamera;
    public float gravityDownForce = 10f; //自由落体速度
    public float maxSpeedOnGround = 8f;  //行走移动速度
   
    //添加
    public float runSpeedOnGround = 16;
    public float speed;//移动速度
    public bool isRun;//判断是否在奔跑
    public bool isWalk;//判断是否在行走
    private KeyCode runInputName;//奔跑键位
    public float jumpForce = 5f;//跳跃力度
    public Vector3 velocity;//设置玩家Y轴的一个冲量变化
    private string jumpInputName = "Jump";//跳跃键位
    private bool isJump;//判断是否在跳跃
//public Transform groundCheck;//地面检测物体
    public LayerMask groundMask;
    private AudioSource audioSource; //声音源，
    public AudioClip walkingSound; //声音片段
    public AudioClip runingSound;


    public float moveSharpnessOnGround = 15f;  //反应速度

    public float rotationSpeed = 200f;  //相机旋转速度

    public float cameraHeightRatio = 0.85f;  //相机在人上的身高比例
    public float maxHealth = 200f;

    private CharacterController characterController;
    private PlayerInputHandler playerInputHandler;
    private float targetCharacterHeight = 1.8f;  //目标身高
    private float cameraVerticalAngle = 0f; //相机纵坐标的值
    private float _currentHealth;
    private bool _isBossAttack;

    public float CurrentHealth => _currentHealth;

    public Vector3 characterVelocity;           //速度的公有属性


    public Text NameText;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        audioSource = GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;

        //类似于防止穿模
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
        characterController.height = targetCharacterHeight;     //调整身高
        characterController.center = Vector3.up * characterController.height * 0.5f;//调整重心
        playerCamera.transform.localPosition = Vector3.up * characterController.height * cameraHeightRatio;//调整相机位置

    }

    private void HandlerCharacterMovement()
    {
        isRun = Input.GetKey(runInputName);
        isWalk = (Mathf.Abs(playerInputHandler.GetMouseLookHorizontal()) > 0 || Mathf.Abs(playerInputHandler.GetMouseLookVertical()) > 0) ? true : false;

        speed = isRun ? runSpeedOnGround : maxSpeedOnGround;


        //camera rotate horizantal
        //水平方向旋转
        transform.Rotate(new Vector3(0, playerInputHandler.GetMouseLookHorizontal() * rotationSpeed, 0),
            Space.Self);
        //垂直方向旋转  cameraVerticalAngle一点要累加，因为GetMouseLookVertical返回的是瞬间值
        cameraVerticalAngle += playerInputHandler.GetMouseLookVertical() * rotationSpeed;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -80f, 80f); //对竖直旋转加以限制
        playerCamera.transform.localEulerAngles = new Vector3(-cameraVerticalAngle, 0, 0); //注意是反向的，所以要负号


        //move
        Vector3 wordSpaceMoveInput = transform.TransformVector(playerInputHandler.GetMoveInput());

        //判断是否在地面
        if (characterController.isGrounded)
        {
            //在地面
            Vector3 targetVelocity = wordSpaceMoveInput * speed;

            //让现在的值逐步过度到移动后的值
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, moveSharpnessOnGround * Time.deltaTime);
            
            Jump();
        }
        else
        {
            //在空中  就落入地面
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }
        if (_isBossAttack)
        {
            characterVelocity += transform.forward * -5;

        }

        //提供带有附加CharacterController组件的游戏对象的移动
        characterController.Move(characterVelocity * Time.deltaTime);

        //向下增加力量
        /*characterController.Move(Vector3.down * characterController.height / 2 * Time.deltaTime);*/
        /*characterController.Move(velocity * Time.deltaTime);*/
        PlayFootStepSound();
    }

    public void Jump()
    {
        
        isJump = Input.GetButtonDown(jumpInputName);
        //施加跳跃的力 
        if (isJump)//&& characterController.isGrounded
        {
            Debug.Log("Input Button Down Jump.");
            characterVelocity.y = Mathf.Sqrt(jumpForce * gravityDownForce);
            //velocity.y = 20f;
        }
    }

    ///播放移动的音效
    public void PlayFootStepSound()
    {
        if (characterController.isGrounded && characterVelocity.sqrMagnitude > 0.9f)
        {
            //Debug.LogError("run or walk");
            audioSource.clip = isRun ? runingSound : walkingSound;//设置行走或者奔跑的音效
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
            print("玩家被攻击");
            StartCoroutine(OnDamge());
        }
    }

    IEnumerator OnDamge()
    {
        print("玩家被攻击,现在血量为：" + _currentHealth);
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
