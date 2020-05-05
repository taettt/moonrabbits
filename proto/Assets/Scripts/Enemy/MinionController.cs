using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public GameObject seedPrefab;
    public Transform seedParent;

    private float m_attackSpeed;
    private int m_attackStat;

    void Awake()
    {
        seedParent = GameObject.Find("Traps").transform;
        Init();
    }

    void OnTriggernEnter(Collider coll)
    {
        if(coll.tag=="PLAYERBULLET")
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
        GameObject go = Instantiate(seedPrefab, this.transform.position, Quaternion.identity);
        go.transform.SetParent(seedParent);
    }
}
