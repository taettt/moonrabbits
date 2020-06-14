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
    public RectTransform playerFillMaskRectTr;
    public float playerFillMaskWidth;
    public Image m_playerLifeBg;
    public RectTransform playerGlow;
    public Sprite[] m_playerLifes;

    public Text playerHpText;
    public Text bossHpText;

    public Image m_enemyHpFillImages;
    public RectTransform bossFillMaskRectTr;
    public float bossFillMaskWidth;
    public RectTransform bossGlow;
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
        m_bossLifeBg.sprite = m_bossLifes[5];
        m_playerLifeBg.sprite = m_playerLifes[3];

        bossFillMaskWidth = bossFillMaskRectTr.sizeDelta.x;
        playerFillMaskWidth = playerFillMaskRectTr.sizeDelta.x;
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

        if (ec.curHp <= 0.0f)
        {
            SetLifeDown_Enemy();
        }
    }

    private void PlayerHPUpdate()
    {
        Vector2 fillMaskSize = playerFillMaskRectTr.sizeDelta;
        fillMaskSize.x = playerFillMaskWidth * (pc.hp / 40.0f);
        playerFillMaskRectTr.sizeDelta = Vector2.Lerp(playerFillMaskRectTr.sizeDelta, fillMaskSize,
            Time.deltaTime * m_hpDownTime);

        // glow는 따로 width 넣어줘야할거같음
        playerGlow.anchoredPosition = Vector2.Lerp(playerGlow.anchoredPosition,
            new Vector2((pc.hp / 120.0f) * 207, -12f), Time.deltaTime * m_hpDownTime);
    }

    private void BossHPUpdate()
    {
        Vector2 fillMaskSize = bossFillMaskRectTr.sizeDelta;
        fillMaskSize.x = bossFillMaskWidth * (ec.curHp / 120.0f);
        bossFillMaskRectTr.sizeDelta = Vector2.Lerp(bossFillMaskRectTr.sizeDelta, fillMaskSize,
            Time.deltaTime * m_hpDownTime);

        // glow는 따로 width 넣어줘야할거같음
        bossGlow.anchoredPosition = Vector2.Lerp(bossGlow.anchoredPosition,
            new Vector2((ec.curHp / 120.0f) * 330, -12f), Time.deltaTime * m_hpDownTime);
    }

    // state iniv & life down true
    private void SetHealAllFill()
    {
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
