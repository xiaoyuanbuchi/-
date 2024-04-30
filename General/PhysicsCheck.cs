using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    private Vector3 Dir;
    private PlayerController playerController;
    private Rigidbody2D rb;
    [Header("������")]
    public bool manual;//�ж���ײ�������������Ƿ�ʹ���Զ��ж�
    public bool isPlayer;//������ǽ����Ƿ���Ի��playercotroller���
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;//ƫ��
    public float checkRaduis;//��ⷶΧ
    public LayerMask groundLayer;
    public LayerMask spikeLayer;
    [Header("״̬")]
    public bool isGround = Global.isground;
    public bool checkSpike;
    public bool touchLeftWall;//����Ƿ��������ǽ��
    public bool touchRightWall;
    public bool onWall;//�Ƿ���ǽ��
    private void Update()
    {
        Check();
    }
    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if(!manual)//���ֶ�
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
        if (isPlayer)
            playerController = GetComponent<PlayerController>();
    }
    public void Check()
    {
        // ������
        CheckGround();

        // ���ǽ��
        CheckWall();
    }

    private void CheckGround()
    {
        if (onWall)
        {
            Vector2 origin = transform.position;
            Vector2 offset = new(bottomOffset.x * transform.localScale.x, bottomOffset.y);
            Vector2 position = origin + offset;
            isGround = Physics2D.OverlapCircle(position, checkRaduis, groundLayer);
        }
        else
        {
            Vector2 origin = transform.position;
            Vector2 offset = new (bottomOffset.x * transform.localScale.x, 0);
            Vector2 position = origin + offset;
            isGround = Physics2D.OverlapCircle(position, checkRaduis, groundLayer);
            checkSpike = Physics2D.OverlapCircle(position, checkRaduis, spikeLayer);
        }
    }
 

    private void CheckWall()
    {
        // ǽ���ж�
        touchLeftWall  = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset,  checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);

        if (!isPlayer)
            return;

        // ��ǽ����
        if (!onWall)
        {
            bool isTouchingLeftWall  = touchLeftWall  && playerController.faceDir < 0f;
            bool isTouchingRightWall = touchRightWall && playerController.faceDir > 0f;
            bool isTouchingWall = isTouchingLeftWall || isTouchingRightWall;

            bool isDroping = rb.velocity.y < 0f;

            onWall = isTouchingWall && isDroping;
        }
        else
        {
            bool isTouchingWall = touchLeftWall || touchRightWall;
            bool isDroping = rb.velocity.y < 0f;

            onWall = isTouchingWall && isDroping;
        }
    }

    private void OnDrawGizmos()
    {
        Dir = new Vector3(transform.localScale.x, 1, 1);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset * (Vector2)Dir, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
}
