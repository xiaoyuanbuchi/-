using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Skill);
        }
        bool touchLWall = currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0;
        bool touchRWall = currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0;
        bool touchWall = touchLWall || touchRWall;
        if (!currentEnemy.physicsCheck.isGround || touchWall || currentEnemy.physicsCheck.checkSpike)//µ±Åöµ½×óÓÒÇ½Ê±
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

    }
}

