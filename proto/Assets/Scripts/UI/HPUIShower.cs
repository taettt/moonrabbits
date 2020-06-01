﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 나중에 코루틴으로 처리
public class HPUIShower : MonoBehaviour
{
    public PlayerController pc;
    public PlayerStateController psc;
    public BossController ec;
    public EnemyStateController esc;

    public Image m_playerHpFillImages;
    public Image m_playerLifeBg;
    public Sprite[] m_playerLifes;

    public Text playerHpText;
    public Text bossHpText;

    public Image m_enemyHpFillImages;
    public Image m_bossLifeBg;
    public Sprite[] m_bossLifes;

    //time
    private float m_hpDownTime = 1.0f;
    private float m_HealTime = 1.0f;
    private float m_maxHealTimer = 0.0f;
    private float m_maxHealTime = 3.0f;
    private float m_lifeHealTime = 1.0f;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        PlayerUpdate();
        EnemyUpdate();

        playerHpText.text = pc.hp.ToString();
        bossHpText.text = ec.curHp.ToString();
    }

    private void Initialize()
    {
        m_playerHpFillImages.fillAmount = 1.0f;
        m_enemyHpFillImages.fillAmount = 1.0f;
        m_bossLifeBg.sprite = m_bossLifes[5];
        m_playerLifeBg.sprite = m_playerLifes[3];
    }

    private void PlayerUpdate()
    {
        m_playerHpFillImages.fillAmount = pc.hp / 40.0f;

        if (psc.curState == PlayerState.RETIRE)
        {
            // call once
            SetLifeDown();
            SetHealAllFill();
        }
    }

    private void EnemyUpdate()
    {
        m_enemyHpFillImages.fillAmount = ec.curHp / 120f;

        if (m_enemyHpFillImages.fillAmount <= 0.0f)
        {
            SetLifeDown_Enemy();
        }

        else if(ec.isLifeDown && esc.curState==EnemyState.RETIRE)
        {
            SetHealAllFill_Enemy();
        }
    }

    // state iniv & life down true
    private void SetHealAllFill()
    {
        if (m_maxHealTimer >= m_maxHealTime)
        {
            m_maxHealTimer = 0.0f;
            return;
        }

        m_maxHealTimer += Time.deltaTime;
        m_playerHpFillImages.fillAmount = Mathf.Lerp(m_playerHpFillImages.fillAmount,
            1.0f, m_maxHealTimer / m_maxHealTime);
    }

    private void SetHealAllFill_Enemy()
    {
        m_enemyHpFillImages.fillAmount = Mathf.Lerp(m_enemyHpFillImages.fillAmount,
            1.0f, 3.0f * Time.deltaTime);
    }

    private void SetLifeDown()
    {
        m_playerLifeBg.sprite = m_playerLifes[(int)pc.life];
    }

    private void SetLifeDown_Enemy()
    {
        m_bossLifeBg.sprite = m_bossLifes[(int)ec.curLife];
    }

    public void RetryFill()
    {
        Initialize();
    }
}
