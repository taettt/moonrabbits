using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Create Boss")]
[SerializeField]
public class Boss : ScriptableObject
{
    public int m_maxHp;
    public int m_maxLife;

    public int m_maxAttackedDamage;
    public int m_trapDamage;
    public int m_boomDamage;

    public int m_damage;
    public float m_attackSpeed;
    public float m_moveSpeed;

    // delay
    public float m_knockDelay;
    public float m_retireDelay;
    public float m_deathDelay;
}
