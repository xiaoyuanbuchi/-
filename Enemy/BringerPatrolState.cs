using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringerPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        bool touchLeftWall = currentEnemy.physicsCheck.touchLeftWall && currentEnemy.transform.localScale.x > 0;
        bool touchRightWall = currentEnemy.physicsCheck.touchRightWall && currentEnemy.transform.localScale.x < 0;
        bool touchWall = touchRightWall || touchLeftWall;
        if (touchWall || !currentEnemy.physicsCheck.isGround || currentEnemy.physicsCheck.checkSpike)
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
