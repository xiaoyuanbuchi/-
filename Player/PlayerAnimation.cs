using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }
    private void Update()
    {
        SetAnimation();
    }
    public void SetAnimation()//����Щ���ݺ�animation��������
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));//��ȡx�ľ���ֵ
        anim.SetFloat("velocityY", rb.velocity.y);//��ȡy
        anim.SetBool("isGround", physicsCheck.isGround); 
        anim.SetBool("isCrouch", playerController.isCrouch);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerController.isSlide);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");//����hurt�¼�
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");//����attack�¼�
    }
}
