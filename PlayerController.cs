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
    [Header("�����¼�")]
    public SceneLoadEventSO sceneLoadEvent;//���س���
    public VoidEventSO afterSceneLoadedEvent;//�������غ��¼�
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
    // ��ǰʵ����ȫ��player list�е�id
    public int id;

    [Header("����������")]
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
   
    public int hurtTime; // �������˳�����֡��

    [Header("�������")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("״̬")]
    public bool isHurt;
    public bool isCrouch;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;
    public bool isDrinking;

    // ��start�������еĺ���
    private void Awake()
    {
        // inputControl = new PlayerInputControl();//ʵ����

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
        #region ǿ����·
        runSpeed = speed;
        #endregion
        inputControl.Enable();
    }

    private void Update()
    {
        if (isDrinking)//�ں�ҩʱ������Ҳ���
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

  


    // �������йصĻ��������
    private void FixedUpdate()
    {
        // �ж�����ʱ�����ƶ���ʱ��
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
    //�������ع���ֹͣ����
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        
        inputControl.Gameplay.Disable();
    }

    private void onLoadDataEvent()
    {
        isDead = false;
    }
    //�������ؽ���֮����������
    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }

    public int faceDir;
    public void Move(Vector2 inputDirection)
    {
        // ���ˣ�����,����,��ǽ����ʱ�����ƶ�
        if (isHurt || isAttack || wallJump || isSlide)
            return;

        // �ж��Ƿ����ڸ���ǽ��
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

        // ���﷭ת
        transform.localScale = new Vector3(faceDir, 1, 1);
        // �¶�
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;

        if(isCrouch)
        {
            // �޸���ײ���С��λ�Ʋ�ͨ��return��ֹ�ƶ�
            coll.offset = new Vector2(-0.05f, 0.79f);
            coll.size = new Vector2(0.7f, 1.6f);
            return;
        }
        else
        {
            // ��ԭ֮ǰ��ײ�����
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

    // ��Ծ
    public void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("jump");
        if(physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            playerAudioController.PlayJumpClip();//������Ծ��Ч
            isSlide = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
            StopAllCoroutines();//��ϻ���Э�̲�����ͼ���isSlide
        }
        else if(physicsCheck.onWall)
        {
            playerAudioController.PlayJumpClip();//������Ծ��Ч
            rb.AddForce(new Vector2(-faceDir, 3f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    // ����
    public void PlayerAttack(CallbackContext obj)
    {
        if(physicsCheck.isGround && !isSlide)// ��Ծ��������ʱ���ܹ���
        {
            playerAnimation.PlayAttack();
            isAttack = true;
        }
    }

    // ����
    public bool PlayerInteract()
    {
        Vector2 position = transform.position;
        Vector2 direction = new Vector2(faceDir, 0);
        position += direction; // ��ֹ�ж�������
        // Debug.Log("Interact from: position: " + position + " direction: " + direction);
        return interactCheck.Check(position, direction);
    }
   
    public void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            playerAudioController.PlaySildeClip();//������Ծ��Ч
            gameObject.layer = LayerMask.NameToLayer("Enemy");//����ͼ��ʵ�ֻ���ʱ���޵�Ч��
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
            if (!physicsCheck.isGround)//�뿪����ʱ���ѭ��
                break;
            bool isTouchingLeftWall = physicsCheck.touchLeftWall && faceDir < 0f;
            bool isTouchingRightWall = physicsCheck.touchRightWall && faceDir > 0f;
            bool isTouchingWall = isTouchingLeftWall || isTouchingRightWall;
            if (isTouchingWall)//���й�����ײǽֹͣ����
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
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;// ��ȡ�����ķ���

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);// ˲ʱ���������ﷴ��
    }

    public void PlayerDead()
    {
        isDead = true;
        // inputControl.Gameplay.Disable();//�������ֹ����ƶ���ֹͣgameplay�����룩
     
      //  inputControl.Gameplay.Jump.Disable();

    }
    #endregion

    public void CheckState(Vector2 inputDirection)
    {
        // �������Ļ���ײ��ѡ��⻬����
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
        if (physicsCheck.onWall && inputDirection.y < 0f)
        {
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);//�����»�
            rb.AddForce(new Vector2(-faceDir, 0) * wallJumpForce, ForceMode2D.Impulse);//����ǽ��
        }
        else if(physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        if (wallJump && rb.velocity.y < 0f)
            wallJump = false;
    }

    // ���û�Ծ�����տ����¼�
    public void Active()
    {
        Global.SetCurrentPlayer(id);

        // ���Ҫ�ӵ���ʼ��Ϸ����֮�󣬷������������˵���
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
            isOpen = !isOpen;//��һ�´򿪱������ٰ�һ��ȡ������
            myBag.SetActive(isOpen);
        }
    }
}
