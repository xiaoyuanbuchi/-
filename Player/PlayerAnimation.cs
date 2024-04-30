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
    public void SetAnimation()//将这些数据和animation连接起来
    {
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));//获取x的绝对值
        anim.SetFloat("velocityY", rb.velocity.y);//获取y
        anim.SetBool("isGround", physicsCheck.isGround); 
        anim.SetBool("isCrouch", playerController.isCrouch);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("onWall", physicsCheck.onWall);
        anim.SetBool("isSlide", playerController.isSlide);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");//触发hurt事件
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");//触发attack事件
    }
}
