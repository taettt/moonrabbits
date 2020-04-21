using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPUIShower : MonoBehaviour
{
    public PlayerController pc;
    public PlayerStateController psc;
    public _EnemyController ec;
    public EnemyStateController esc;

    //ui, 0-> red 1->blue
    public Image[] m_playerHpFillImages = new Image[2];
    public Image[] m_playerLifeImage = new Image[3];

    public Image[] m_enemyHpFillImages = new Image[2];
    public Image[] m_enemyLifeImage = new Image[2];

    //time
    private float m_playerDownTime = 1.0f;
    private float m_playerHealTime = 1.0f;
    private float m_playerMaxTime = 3.0f;
    private float m_playerLifeHealTime = 1.0f;

    //index
    private int curEnemyLifeIndex;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        PlayerUpdate();
        EnemyUpdate();
    }

    private void Initialize()
    {
        m_playerHpFillImages[0].fillAmount = 1.0f;
        m_playerHpFillImages[1].fillAmount = 1.0f;
        m_enemyHpFillImages[0].fillAmount = 1.0f;
        m_enemyHpFillImages[1].fillAmount = 1.0f;

        curEnemyLifeIndex = 0;
    }

    private void PlayerUpdate()
    {
        m_playerHpFillImages[1].fillAmount = pc.hp / 40.0f;
        SetAttackedFill();

        if (psc.curState == PlayerState.RETIRE)
        {
            // call once
            SetLifeDown();
            SetHealAllFill();
        }
    }

    private void EnemyUpdate()
    {
        m_enemyHpFillImages[1].fillAmount = ec.hp / 120.0f;
        SetAttackedFill_Enemy();

        if (m_enemyHpFillImages[1].fillAmount <= 0.0f)
        {
            SetLifeDown_Enemy();
        }

        else if(ec.isLifeDown && esc.curState==EnemyState.RETIRE)
        {
            SetHealAllFill_Enemy();
        }
    }

    private void SetAttackedFill()
    {
        m_playerHpFillImages[0].color = Color.red;
        m_playerHpFillImages[0].fillAmount = Mathf.Lerp(m_playerHpFillImages[0].fillAmount,
            m_playerHpFillImages[1].fillAmount, m_playerDownTime * Time.deltaTime);
    }

    private void SetAttackedFill_Enemy()
    {
        m_enemyHpFillImages[0].fillAmount = Mathf.Lerp(m_enemyHpFillImages[0].fillAmount,
            m_enemyHpFillImages[1].fillAmount, 1.0f * Time.deltaTime);
    }

    private void SetHealFill()
    {
        m_playerHpFillImages[0].color = Color.green;
        m_playerHpFillImages[0].fillAmount = Mathf.Lerp(m_playerHpFillImages[0].fillAmount,
            (m_playerHpFillImages[1].fillAmount), m_playerHealTime * Time.deltaTime);
    }

    // state iniv & life down true
    private void SetHealAllFill()
    {
        m_playerHpFillImages[1].gameObject.SetActive(false);
        m_playerHpFillImages[0].color = Color.green;
        m_playerHpFillImages[0].fillAmount = Mathf.Lerp(m_playerHpFillImages[0].fillAmount,
            (m_playerHpFillImages[1].fillAmount), m_playerHealTime * Time.deltaTime);
        m_playerHpFillImages[1].gameObject.SetActive(true);
    }

    private void SetHealAllFill_Enemy()
    {
        m_enemyHpFillImages[1].gameObject.SetActive(false);
        m_enemyHpFillImages[0].fillAmount = Mathf.Lerp(m_enemyHpFillImages[0].fillAmount,
            m_enemyHpFillImages[1].fillAmount, 3.0f * Time.deltaTime);
        m_enemyHpFillImages[1].gameObject.SetActive(true);
    }

    private void SetLifeDown()
    {
        m_playerLifeImage[pc.life].color = Color.black;
    }

    private void SetLifeDown_Enemy()
    {
        m_enemyLifeImage[curEnemyLifeIndex].color = Color.black;
        curEnemyLifeIndex++;
    }

    private void SetLifeFill()
    {
        m_playerLifeImage[pc.life].color = Color.green;

        if (Time.time>m_playerLifeHealTime)
        {
            m_playerLifeImage[pc.life].color = Color.blue;
        }
    }

    public void RetryFill()
    {
        m_playerLifeImage[0].color = Color.blue;
        m_playerLifeImage[1].color = Color.blue;
        m_playerLifeImage[2].color = Color.blue;

        m_enemyLifeImage[0].color = new Color(244.0f / 255.0f, 103.0f / 255.0f, 1.0f / 255.0f, 1.0f);
        m_enemyLifeImage[1].color = new Color(244.0f / 255.0f, 103.0f / 255.0f, 1.0f / 255.0f, 1.0f);
    }
}
