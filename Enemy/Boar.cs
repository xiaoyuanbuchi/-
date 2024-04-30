using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy// ¼Ì³ĞµĞÈËÕâ¸ö¸¸Àà
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();//¸³ÖµÒ°ÖíµÄÑ²Âß×´Ì¬Âß¼­
        chaseState = new BoarChaseState();//¸³ÖµÒ°ÖíµÄ×·»÷×´Ì¬Âß¼­
    }
}
