using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }
    public override void LogicUpdate()
    {
        //TODO:发现player切换到chase
        if(currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        bool touchLeftWall = currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0;
        bool touchRightWall = currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0;
        bool touchWall = touchLeftWall || touchRightWall;
        if (!currentEnemy.physicsCheck.isGround || touchWall || currentEnemy.physicsCheck.checkSpike)//当碰到左右墙时
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else
            currentEnemy.anim.SetBool("walk", true);
    }

    public override void PhysicsUpdate()
    {
        
    }
    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
        //Debug.Log("Exit");
    }
}
