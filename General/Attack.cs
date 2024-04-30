using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;//攻击范围
    public float attackRate;//攻击频率

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<Character>()?.TakeDamage(this);//把这个attack类的值传进去伤害计算函数里
    }
}
