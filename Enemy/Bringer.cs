using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bringer : Enemy
{
    private void Awake()
    {
        base.Awake();
        patrolState = new BringerPatrolState();
    }
}
