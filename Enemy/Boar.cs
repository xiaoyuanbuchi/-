using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy// �̳е����������
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();//��ֵҰ���Ѳ��״̬�߼�
        chaseState = new BoarChaseState();//��ֵҰ���׷��״̬�߼�
    }
}
