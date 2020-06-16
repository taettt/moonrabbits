using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillManager : MonoBehaviour
{
    //boom
    public Transform trapParent;
    public Transform bulletParent;
    private int m_boomActiveCount = 5;
    private bool m_boomActive;

    private int m_boomCount;
    public int boomCount { get { return m_boomCount; } }
    public Image boomImage;
    public Sprite boomOffSprite;
    public Sprite[] boomOnSprite;
    private bool m_isBoomSpriteChange;
    private int boomIndex;

    private int m_boomDamage = 16;
    public int boomDamage { get { return m_boomDamage; } }

    // fa
    private float m_faSkillGague;
    [SerializeField]
    private bool m_isFASkillOn;
    public bool isFASkillOn { get { return m_isFASkillOn; } }
    [SerializeField]
    private bool m_isFaSkillPlaying;
    public bool isFaSkillPlaying { get { return m_isFaSkillPlaying; } set { m_isFaSkillPlaying = value; } }

    public Image faImage;
    public Sprite faOffSprite;
    public Sprite[] faOnSprite;
    private bool m_isFASpriteChange;
    private int faIndex;
    public Text faText;

    // gague
    public Slider fxSlider;

    public BossController bc;

    void Awake()
    {
        m_boomActive = false;
        m_boomCount = 2;

        m_isBoomSpriteChange = false;
        boomIndex = 0;
        m_isFASpriteChange = false;
        faIndex = 0;
    }

    void Start()
    {
        StartCoroutine(ChangeBoomUI());
        fxSlider.minValue = 0.0f;
        fxSlider.maxValue = 100.0f;
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

        fxSlider.value = m_faSkillGague;
    }

    public void IncreaseFAGague()
    {
        if (m_faSkillGague >= 100.0f)
            return;
        else if(m_faSkillGague + 2.0f >= 100.0f)
        {
            m_faSkillGague = 100.0f;
        }
        else
        {
            m_faSkillGague += 2.0f;
            if(m_faSkillGague >= 10.0f)
            {
                m_isFASkillOn = true;
                m_isFASpriteChange = true;
                StartCoroutine(ChangeFAUI());
            }
        }

        faText.text = ((int)m_faSkillGague).ToString();
    }

    public void DecreaseFAGague()
    {
        if (m_faSkillGague <= 0.0f)
        {
            m_isFASkillOn = false;
            m_isFaSkillPlaying = false;
            faImage.sprite = faOffSprite;
            faIndex = 0;
            m_isFASpriteChange = false;
            return;
        }
        else if (m_faSkillGague - 1.0f <= 0.0f)
        {
            m_faSkillGague = 0.0f;
            m_isFASkillOn = false;
            m_isFaSkillPlaying = false;
            faImage.sprite = faOffSprite;
            faIndex = 0;
            m_isFASpriteChange = false;
        }
        else
        {
            m_faSkillGague -= 1.0f;
        }

        faText.text = ((int)m_faSkillGague).ToString();
    }

    private IEnumerator ChangeBoomUI()
    {
        while (true)
        {
            if (m_isBoomSpriteChange && boomIndex < 2)
            {
                boomImage.sprite = boomOnSprite[boomIndex];

                yield return new WaitForSeconds(0.3f);
                boomIndex++;
            }

            yield return 0;
        }
    }

    private IEnumerator ChangeFAUI()
    {
        while (m_isFASpriteChange && faIndex < 2)
        {
            boomImage.sprite = boomOnSprite[faIndex];

            yield return new WaitForSeconds(0.3f);
            faIndex++;
        }

        yield return 0;
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
            faIndex = 0;
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
