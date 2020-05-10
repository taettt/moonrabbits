using System.Collections;
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

    //ui, 0-> red fill 1-> hp fill
    public Image[] m_playerHpFillImages = new Image[2];
    public Image m_playerLife;
    public Sprite[] m_playerLifeImage = new Sprite[4];

    public Image[] m_enemyHpFillImages = new Image[2];
    public Image m_bossLife;
    public Sprite[] m_enemyLifeImage = new Sprite[6];

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
    }

    private void Initialize()
    {
        m_playerHpFillImages[0].fillAmount = 1.0f;
        m_playerHpFillImages[1].fillAmount = 1.0f;
        m_enemyHpFillImages[0].fillAmount = 1.0f;
        m_enemyHpFillImages[1].fillAmount = 1.0f;
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
        m_enemyHpFillImages[1].fillAmount = ec.curHp / 120f;
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
            m_playerHpFillImages[1].fillAmount, m_hpDownTime * Time.deltaTime);
    }

    private void SetAttackedFill_Enemy()
    {
        m_enemyHpFillImages[0].fillAmount = Mathf.Lerp(m_enemyHpFillImages[0].fillAmount,
            m_enemyHpFillImages[1].fillAmount, 1.0f * Time.deltaTime);
    }

    private void SetHealFill()
    {
        m_playerHpFillImages[0].fillAmount = Mathf.Lerp(m_playerHpFillImages[0].fillAmount,
            (m_playerHpFillImages[1].fillAmount), m_HealTime * Time.deltaTime);
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
        m_playerHpFillImages[1].gameObject.SetActive(false);
        m_playerHpFillImages[0].fillAmount = Mathf.Lerp(m_playerHpFillImages[0].fillAmount,
            (m_playerHpFillImages[1].fillAmount), m_maxHealTimer / m_maxHealTime);
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
        m_playerLife.sprite = m_playerLifeImage[(int)pc.life];
    }

    private void SetLifeDown_Enemy()
    {
        m_bossLife.sprite = m_enemyLifeImage[(int)ec.curLife];
    }

    private void SetLifeFill()
    {
        m_playerLife.sprite = m_playerLifeImage[(int)pc.life];
        m_bossLife.sprite = m_enemyLifeImage[(int)ec.curLife];
    }

    public void RetryFill()
    {
        SetLifeFill();
    }
}
