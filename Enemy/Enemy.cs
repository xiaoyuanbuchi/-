using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public  Animator anim;//ʹ���ݿ��Ա���������ò�������unity����ʾ
    [HideInInspector] public PhysicsCheck physicsCheck;
    [Header("��������")]
    public float normalSpeed;//�����ٶ�
    public float chaseSpeed;//׷���ٶ�
    [HideInInspector] public float currentSpeed;//��ǰ�ٶ�
    public Vector3 faceDir;//�泯����
    public float hurtForce;//player�������˵���
    public Vector3 spwanPoint;//������

    
    public Transform attacker;
    [Header("���")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("��ʱ��")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("״̬")]
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
    private void OnEnable()//�ڸö�������ʱ��������Start
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }
    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();//����Ѳ��״̬ʱ���߼��ж�
        TimeCounter();
    }
    private void FixedUpdate()//�����ƶ�������ִ��
    {
        if(!isHurt && !isDead && !wait)
        Move();
        currentState.PhysicsUpdate();
    }
    private void OnDisable()//����Ϸ�����ڳ�������ʧʱִ��
    {
        currentState.OnExit();
    }
    public virtual void Move()//virtul��������޸ĸú���
    {
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);//ͨ������ȥ�޸��ٶ�
    }
    /// <summary>
    /// ��ʱ��
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
            NPCState.Patrol => patrolState,//Ѳ��״̬�·���Ѳ��
            NPCState.Chase => chaseState,//׷��״̬�·���׷��
            NPCState.Skill => skillState,
            _=> null//ͨ������·��ؿ�
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }
    #region �¼�ִ�з���
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //ת��
        if(attackTrans.position.x-transform.position.x>0)//���player�ڵ��˵��ұ�
        {
            transform.localScale = new Vector3(-1, 1, 1);//���˳����Ϊ�ұ�
        }
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        //���˱�����
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));
        
    }

    private IEnumerator OnHurt(Vector2 dir)//Э��
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;//�����������Ķ������������һ�̣��ѵ��˵�ͼ���Ϊ����player������ײ��ͼ�㣬���ٲ���Ҫ���˺�
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
