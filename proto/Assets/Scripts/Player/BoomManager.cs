using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 필드 안에서 gen된(child 갯수로 검사해야할듯?) 트랩 개수로 bool 판단
public class BoomManager : MonoBehaviour
{
    public Transform trapParent;
    public Transform bulletParent;
    private int m_boomActiveCount = 5;
    private bool m_boomActive;

    public Image boomImage;
    public Sprite boomOffSprite;
    public Sprite[] boomOnSprite;
    private bool m_isSpriteChange;
    private int index;

    private int m_boomDamage = 16;
    public int boomDamage { get { return m_boomDamage; } }

    public BossController bc;

    void Awake()
    {
        m_boomActive = false;

        m_isSpriteChange = false;
        index = 0;
    }

    void Start()
    {
        StartCoroutine(ChangeUI());
    }

    void Update()
    {
        TrapCheck();

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (!m_boomActive)
                return;

            // 넉백
            DeleteObjects();
            bc.DecreaseHP(m_boomDamage);
        }
    }

    private IEnumerator ChangeUI()
    {
        while (m_isSpriteChange && index < 2)
        {
            boomImage.sprite = boomOnSprite[index];

            yield return new WaitForSeconds(0.3f);
            index++;
        }

        yield return 0;
    }

    // 나중에 trap에서 check함
    private void TrapCheck()
    {
        if (trapParent.childCount >= m_boomActiveCount)
        {
            m_boomActive = true;
            m_isSpriteChange = true;
        }
        else
        {
            m_boomActive = false;
            boomImage.sprite = boomOffSprite;
            m_isSpriteChange = false;
            index = 0;
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

        while(trapParent.childCount > 0)
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
