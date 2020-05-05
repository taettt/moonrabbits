using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedTrap : MonoBehaviour
{
    private float m_rad;
    private float m_seedTime;

    [SerializeField]
    private bool m_flowerTurn;
    private int m_damage;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if(Time.time>m_seedTime)
        {
            m_flowerTurn = true;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.gameObject.tag)
        {
            case "ENEMY":
                if (!m_flowerTurn)
                {
                    return;
                }

                coll.gameObject.GetComponent<_EnemyController>().DecreaseHP(m_damage);
                Destroy(this.gameObject);
                break;
            case "PLAYER":
                if (m_flowerTurn)
                {
                    return;
                }

                coll.gameObject.GetComponent<PlayerController>().DecreaseHP(m_damage);
                Destroy(this.gameObject);
                break;
        }
    }

    /*
    // overlap으로 충돌객체 다 구해오는것도 ㄱㅊ을듯??
    void OnCollisionStay(Collision coll)
    {
        switch (coll.gameObject.tag)
        {
            case "ENEMY":
                if (!m_flowerTurn)
                {
                    coll.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    return;
                }

                //coll.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                coll.gameObject.GetComponent<_EnemyController>().DecreaseHP(m_damage);
                Destroy(this.gameObject);
                break;
            case "PLAYER":
                if (m_flowerTurn)
                {
                    coll.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    return;
                }

                //coll.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                coll.gameObject.GetComponent<PlayerController>().DecreaseHP(m_damage);
                Destroy(this.gameObject);
                break;
        }
    }
    */

    private void KinematicActive(Collision coll, bool active)
    {
        coll.gameObject.GetComponent<Rigidbody>().isKinematic = active;
    }

    private void Init()
    {
        m_seedTime = 5.0f;
        m_damage = 4;
        m_flowerTurn = false;
    }
}
