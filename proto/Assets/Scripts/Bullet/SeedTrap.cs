﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedTrap : MonoBehaviour
{
    private float m_seedTime;
    private float m_seedTimer;

    public Text seedText;

    [SerializeField]
    private bool m_flowerTurn;
    private int m_damage;

    private SpriteRenderer spriteColor;
    public Color m_turnColor;

    void Awake()
    {
        seedText = GameObject.Find("UI").transform.GetChild(1).GetChild(4).GetChild(0).GetComponent<Text>();
        spriteColor = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Init();
    }

    void Update()
    {
        seedText.text = "Seed Time : " + m_seedTimer;
        m_seedTimer += Time.deltaTime;
        if(m_seedTimer>m_seedTime)
        {
            m_flowerTurn = true;
            spriteColor.color = m_turnColor;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        switch (coll.tag)
        {
            case "ENEMY":
                if (m_flowerTurn)
                {
                    coll.gameObject.GetComponent<BossController>().DecreaseHP(m_damage);
                    Destroy(this.gameObject);
                }
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

    private void Init()
    {
        m_seedTime = 5.0f;
        m_damage = 4;
        m_flowerTurn = false;
    }
}
