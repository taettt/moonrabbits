using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public GameObject seedPrefab;

    private float m_attackSpeed;
    private int m_attackStat;

    void Awake()
    {
        Init();
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag=="PLAYERBULLET")
        {
            DropSeed();
        }
    }

    private void Init()
    {
        m_attackSpeed = 1.0f;
        m_attackStat = 1;
    }

    public void DropSeed()
    {
        Instantiate(seedPrefab, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
