using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public  Animator anim;//使数据可以被其他类调用并不会在unity中显示
    [HideInInspector] public PhysicsCheck physicsCheck;
    [Header("基本参数")]
    public float normalSpeed;//正常速度
    public float chaseSpeed;//追击速度
    [HideInInspector] public float currentSpeed;//当前速度
    public Vector3 faceDir;//面朝方向
    public float hurtForce;//player攻击敌人的力
    public Vector3 spwanPoint;//出生点

    
    public Transform attacker;
    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    public BaseState currentState;
    public BaseState patrolState;
    public BaseState chaseState;
    public BaseState skillState;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        spwanPoint = transform.position;
        //waitTimeCounter = waitTime;
    }
    private void OnEnable()//在该对象启动时调用类似Start
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }
    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();//进行巡逻状态时的逻辑判断
        TimeCounter();
    }
    private void FixedUpdate()//刚体移动放这里执行
    {
        if(!isHurt && !isDead && !wait)
        Move();
        currentState.PhysicsUpdate();
    }
    private void OnDisable()//在游戏对象在场景中消失时执行
    {
        currentState.OnExit();
    }
    public virtual void Move()//virtul子类可以修改该函数
    {
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);//通过刚体去修改速度
    }
    /// <summary>
    /// 计时器
    /// </summary>
    public void TimeCounter()
    {
        if(wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if(waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }
        if(!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
      
    }

    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,//巡逻状态下返回巡逻
            NPCState.Chase => chaseState,//追击状态下返回追击
            NPCState.Skill => skillState,
            _=> null//通常情况下返回空
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }
    #region 事件执行方法
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //转身
        if(attackTrans.position.x-transform.position.x>0)//如果player在敌人的右边
        {
            transform.localScale = new Vector3(-1, 1, 1);//敌人朝向改为右边
        }
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        //受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));
        
    }

    private IEnumerator OnHurt(Vector2 dir)//协程
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;//当敌人死亡的动画播放完的那一刻，把敌人的图层改为不和player产生碰撞的图层，减少不必要的伤害
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset+new Vector3(checkDistance*-transform.localScale.x,0), 0.2f);
    }
}
