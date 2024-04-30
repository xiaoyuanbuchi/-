using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        // Debug.Log("Chase");
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;//当前速度变为追击速度
        currentEnemy.anim.SetBool("run", true);//动画状态变成追击状态

    }
    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
            currentEnemy.SwitchState(NPCState.Patrol);
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))//当碰到左右墙时
        {
            //追击状态下撞墙不等待，直接转身
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x , 1, 1);
        }
    }

     public override void PhysicsUpdate()
    {
       
    }

    public override void OnExit()
    {
          
        
         currentEnemy.lostTimeCounter =currentEnemy. lostTime;
        
        currentEnemy.anim.SetBool("run", false);
    }

  
}
