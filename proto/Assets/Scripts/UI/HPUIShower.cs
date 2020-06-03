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

    public Image m_playerHpFillImages;
    public Image m_playerLifeBg;
    public RectTransform playerGlow;
    public Vector2 playerGlowstartPos, playerGlowEndPos;
    public Sprite[] m_playerLifes;

    public Text playerHpText;
    public Text bossHpText;

    public Image m_enemyHpFillImages;
    public RectTransform bossGlow;
    public Vector2 bossGlowstartPos, bossGlowEndPos;
    public Image m_bossLifeBg;
    public Sprite[] m_bossLifes;

    //time
    private float m_playerHpDownTimer = 0.0f;
    private float m_bossHpDownTimer = 0.0f;
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

    // glow -> start pos ~ end pos lerp, t = player cur hp / hp
    private void PlayerUpdate()
    {
        PlayerHPUpdate();

        if (psc.curState == PlayerState.RETIRE)
        {
            // call once
            SetLifeDown();
            SetHealAllFill();
        }
    }

    private void EnemyUpdate()
    {
        BossHPUpdate();

        if (m_enemyHpFillImages.fillAmount <= 0.0f)
        {
            SetLifeDown_Enemy();
        }

        else if(ec.isLifeDown && esc.curState==EnemyState.RETIRE)
        {
            SetHealAllFill_Enemy();
        }
    }

    private void PlayerHPUpdate()
    {
        m_playerHpDownTimer += Time.deltaTime;
        if(m_playerHpDownTimer >= m_hpDownTime)
        {
            m_playerHpDownTimer = 0.0f;
            return;
        }

        m_playerHpFillImages.fillAmount = Mathf.Lerp(m_playerHpFillImages.fillAmount,
            pc.hp / 40.0f, m_playerHpDownTimer / m_hpDownTime);
        playerGlow.anchoredPosition = Vector2.Lerp(playerGlowstartPos, playerGlowEndPos,
            1 - (pc.hp / 40.0f));
    }

    private void BossHPUpdate()
    {
        m_bossHpDownTimer += Time.deltaTime;
        if (m_bossHpDownTimer >= m_hpDownTime)
        {
            m_bossHpDownTimer = 0.0f;
            return;
        }

        m_enemyHpFillImages.fillAmount = Mathf.Lerp(m_enemyHpFillImages.fillAmount,
            ec.curHp / 120.0f, m_bossHpDownTimer / m_hpDownTime);
        bossGlow.anchoredPosition = Vector2.Lerp(bossGlowstartPos, bossGlowEndPos,
            1 - (ec.curHp / 120.0f));
    }

    // state iniv & life down true
    private void SetHealAllFill()
    {
        m_maxHealTimer += Time.deltaTime;
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
        m_maxHealTimer += Time.deltaTime;
        if (m_maxHealTimer >= m_maxHealTime)
        {
            m_maxHealTimer = 0.0f;
            return;
        }

        m_enemyHpFillImages.fillAmount = Mathf.Lerp(m_enemyHpFillImages.fillAmount,
            1.0f, m_maxHealTimer / m_maxHealTime);
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
