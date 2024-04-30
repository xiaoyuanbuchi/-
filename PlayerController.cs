using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEventSO sceneLoadEvent;//加载场景
    public VoidEventSO afterSceneLoadedEvent;//场景加载后事件
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    public UnityEvent<PlayerController> drinkPotion;
    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private PhysicsCheck physicsCheck;
    private Character character;
    private PlayerAudioController playerAudioController;
    private PlayerAnimation playerAnimation;
    public Vector2 inputDirection;
    public InteractCheck interactCheck;

    bool isOpen;
    public GameObject myBag;
    // 当前实例在全局player list中的id
    public int id;

    [Header("基本参数有")]
    public float speed;
    private float runSpeed;
    private float walkSpeed => speed / 3.0f;

    public VoidEventSO OnLoadDataEvent { get; private set; }

    private Vector2 originalOffset;
    private Vector2 originalSize;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;
   
    public int hurtTime; // 计算受伤持续的帧数

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")]
    public bool isHurt;
    public bool isCrouch;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;
    public bool isDrinking;

    // 比start更早运行的函数
    private void Awake()
    {
        // inputControl = new PlayerInputControl();//实例化

        id = Global.AddPlayer(this);
        Active();
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();
        playerAudioController = GetComponent<PlayerAudioController>();
        //potion = GetComponent<Potion>();
        originalOffset = coll.offset;
        originalSize = coll.size;
        interactCheck = new();
        #region 强制走路
        runSpeed = speed;
        #endregion
        inputControl.Enable();
    }

    private void Update()
    {
        if (isDrinking)//在喝药时限制玩家操作
            isDrinking = physicsCheck.isGround && !isAttack && !isHurt && !isSlide && !isCrouch;
        OpenMyBag();
    }

    private void OnEnable()
    {
        inputControl.Disable();
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += onLoadDataEvent;
        backToMenuEvent.OnEventRaised += onLoadDataEvent;
    }

  

    private void OnDisable()
    {
       // inputControl.Disable();
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= onLoadDataEvent;
        backToMenuEvent.OnEventRaised -= onLoadDataEvent;
    }

  


    // 与物理有关的基本用这个
    private void FixedUpdate()
    {
        // 判断受伤时禁用移动的时间
        if (++hurtTime >= Global.hurtDuration)
        {
            isHurt = false;
            hurtTime = 0;
        }
    }

    public void toRun()
    {
        speed = runSpeed;
    }

    public void toWalk()
    {
        speed = walkSpeed;
    }
    //场景加载过程停止控制
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        
        inputControl.Gameplay.Disable();
    }

    private void onLoadDataEvent()
    {
        isDead = false;
    }
    //场景加载结束之后启动控制
    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }

    public int faceDir;
    public void Move(Vector2 inputDirection)
    {
        // 受伤，攻击,滑铲,蹬墙跳的时候不能移动
        if (isHurt || isAttack || wallJump || isSlide)
            return;

        // 判断是否正在附着墙面
        bool isClingingWall = physicsCheck.onWall;
        if (isClingingWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        hurtTime = 0;

        faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

        // 人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);
        // 下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;

        if(isCrouch)
        {
            // 修改碰撞体大小和位移并通过return禁止移动
            coll.offset = new Vector2(-0.05f, 0.79f);
            coll.size = new Vector2(0.7f, 1.6f);
            return;
        }
        else
        {
            // 还原之前碰撞体参数
            coll.size = originalSize;
            coll.offset = originalOffset;
        }

        float deltaTime = Time.deltaTime;

        if (inputDirection.x != 0 && physicsCheck.isGround && !isAttack && !isSlide)
        {
            playerAudioController.PlayMoveClip(true);
        }
        else
            playerAudioController.PlayMoveClip(false);
        rb.velocity = new Vector2(inputDirection.x * speed * deltaTime, rb.velocity.y);
    }

    // 跳跃
    public void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("jump");
        if(physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            playerAudioController.PlayJumpClip();//播放跳跃音效
            isSlide = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
            StopAllCoroutines();//打断滑铲协程并调整图层和isSlide
        }
        else if(physicsCheck.onWall)
        {
            playerAudioController.PlayJumpClip();//播放跳跃音效
            rb.AddForce(new Vector2(-faceDir, 3f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    // 攻击
    public void PlayerAttack(CallbackContext obj)
    {
        if(physicsCheck.isGround && !isSlide)// 跳跃，滑铲的时候不能攻击
        {
            playerAnimation.PlayAttack();
            isAttack = true;
        }
    }

    // 交互
    public bool PlayerInteract()
    {
        Vector2 position = transform.position;
        Vector2 direction = new Vector2(faceDir, 0);
        position += direction; // 防止判定到自身
        // Debug.Log("Interact from: position: " + position + " direction: " + direction);
        return interactCheck.Check(position, direction);
    }
   
    public void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            playerAudioController.PlaySildeClip();//播放跳跃音效
            gameObject.layer = LayerMask.NameToLayer("Enemy");//调整图层实现滑铲时的无敌效果
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
            StartCoroutine(TriggerSlide(targetPos));
            character.OnSlide(slidePowerCost);
        }
    }
    private IEnumerator TriggerSlide(Vector3 targetPos)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)//离开地面时打断循环
                break;
            bool isTouchingLeftWall = physicsCheck.touchLeftWall && faceDir < 0f;
            bool isTouchingRightWall = physicsCheck.touchRightWall && faceDir > 0f;
            bool isTouchingWall = isTouchingLeftWall || isTouchingRightWall;
            if (isTouchingWall)//滑行过程中撞墙停止滑行
            {
                isSlide = false;
                break;
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (MathF.Abs(targetPos.x - transform.position.x) > 0.3f);
        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;// 获取反弹的方向

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);// 瞬时的力把人物反弹
    }

    public void PlayerDead()
    {
        isDead = true;
        // inputControl.Gameplay.Disable();//死亡后禁止玩家移动（停止gameplay的输入）
     
      //  inputControl.Gameplay.Jump.Disable();

    }
    #endregion

    public void CheckState(Vector2 inputDirection)
    {
        // 跳起来的话碰撞体选择光滑材质
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
        if (physicsCheck.onWall && inputDirection.y < 0f)
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);//加速下滑
            rb.AddForce(new Vector2(-faceDir, 0) * wallJumpForce, ForceMode2D.Impulse);//脱离墙面
        }
        else if(physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        if (wallJump && rb.velocity.y < 0f)
            wallJump = false;
    }

    // 设置活跃，接收控制事件
    public void Active()
    {
        Global.SetCurrentPlayer(id);

        // 这句要加到开始游戏操作之后，否则人能在主菜单跑
        CentralControl.SetState(CentralControl.ControlState.Player); 
    }
    public void DrinkPotion()
    {
        drinkPotion?.Invoke(this);
    }


    void OpenMyBag()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            isOpen = !isOpen;//按一下打开背包，再按一下取消背包
            myBag.SetActive(isOpen);
        }
    }
}
