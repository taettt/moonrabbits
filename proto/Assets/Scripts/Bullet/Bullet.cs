using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float m_speed;
    public float speed { get { return m_speed; } }
    private int m_attack;
    public int attack { get { return m_attack; } }

    private Vector3 m_dir;
    public Vector3 dir { get { return m_dir; } }
    private Vector3 m_spawnPos;
    public Vector3 spawnPos { get { return m_spawnPos; } }

    public LayerMask wallCollisionMask;

    public virtual void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        m_spawnPos = spawnPos;
        m_dir = dir;

        m_speed = speed;
        m_attack = attack;

        this.transform.position = m_spawnPos;
        this.transform.rotation = Quaternion.LookRotation(m_dir);
    }

    // mat, prefab setting
    public virtual void SetVisual()
    {
    }

    public virtual void Destroy() { }
}
