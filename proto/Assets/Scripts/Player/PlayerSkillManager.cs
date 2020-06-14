using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillManager : MonoBehaviour
{
    // booms
    public Transform trapParent;
    public Transform bulletParent;
    private int m_boomActiveCount;
    private bool m_boomActive;

    private int m_boomCount;
    public int boomCount { get { return m_boomCount; } }
    public Image boomImage;
    public Sprite boomOffSprite;
    public Sprite[] boomOnSprite;
    private bool m_isBoomSpriteChange;
    private int boomIndex;

    private int m_boomDamage;
    public int boomDamage { get { return m_boomDamage; } }

    public BossController bc;

    // fas
    private float m_FASkillGague;
    [SerializeField]
    public bool m_isFASkillOn;
    private bool m_isFASkillPlaying;
    public bool isFASkillPlaying;

    public Text faSkillText;
    public Image faImage;
    public Sprite faOffSprite;
    public Sprite[] faOnSprite;
    private bool m_isFASpriteChange;
    private int faIndex;

    void Awake()
    {
        m_boomActiveCount = 5;
        m_boomDamage = 16;
        m_boomActive = false;
        m_boomCount = 2;

        m_isFASkillOn = false;
        m_isFASkillPlaying = false;
        m_isBoomSpriteChange = false;
        boomIndex = 0;
    }

    void Start()
    {
        StartCoroutine(ChangeBoomUI());
    }

    void Update()
    {
        TrapCheck();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!m_boomActive)
                return;

            // 넉백
            DeleteObjects();
            bc.DecreaseHP(m_boomDamage);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (!m_isFASkillOn)
                return;
            else
            {
                m_isFASkillPlaying = m_isFASkillPlaying ? false : true;
            }
        }

        faSkillText.text = ((int)m_FASkillGague).ToString();
    }

    private IEnumerator ChangeBoomUI()
    {
        while (m_isBoomSpriteChange && boomIndex < 2)
        {
            boomImage.sprite = boomOnSprite[boomIndex];

            yield return new WaitForSeconds(0.3f);
            boomIndex++;
        }

        yield return 0;
    }

    private IEnumerator ChangeFAUI()
    {
        while(m_isFASpriteChange && faIndex < 2)
        {
            faImage.sprite = faOnSprite[faIndex];

            yield return new WaitForSeconds(0.3f);
            faIndex++;
        }
    }

    // 나중에 trap에서 check함
    private void TrapCheck()
    {
        if (trapParent.childCount >= m_boomActiveCount)
        {
            m_boomActive = true;
            m_isBoomSpriteChange = true;
        }
        else
        {
            m_boomActive = false;
            boomImage.sprite = boomOffSprite;
            m_isBoomSpriteChange = false;
            boomIndex = 0;
        }
    }

    public void IncreseFASkillGague()
    {
        if(m_FASkillGague >= 100.0f)
        {
            return;
        }

        if (m_FASkillGague + 2.0f >= 100.0f)
        {
            m_FASkillGague = 100.0f;
        }
        else
        {
            m_FASkillGague += 2.0f;
        }

        if(m_FASkillGague >= 10.0f)
        {
            m_isFASkillOn = true;
            m_isFASpriteChange = true;
            StartCoroutine(ChangeFAUI());
        }
    }

    public void DecreseFASkillGague()
    {
        if(m_FASkillGague <= 0.0f)
        {
            m_isFASkillPlaying = false;
            m_isFASpriteChange = false;
            faImage.sprite = faOffSprite;
            faIndex = 0;
            return;
        }

        if(m_FASkillGague - 1.0f <= 0.0f)
        {
            m_FASkillGague = 0.0f;
        }
        else
        {
            m_FASkillGague -= 1.0f;
        }
    }

    private void DeleteObjects()
    {
        DeleteTraps();
        DeleteBullets();
    }

    private void DeleteTraps()
    {
        if (trapParent.childCount <= 0)
            return;

        while (trapParent.childCount > 0)
        {
            DestroyImmediate(trapParent.GetChild(0).gameObject);
        }
    }

    private void DeleteBullets()
    {
        if (bulletParent.childCount <= 0)
            return;

        while (bulletParent.childCount > 0)
        {
            DestroyImmediate(bulletParent.GetChild(0).gameObject);
        }
    }
}
