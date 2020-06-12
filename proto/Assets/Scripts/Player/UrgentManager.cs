using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgentManager : MonoBehaviour
{
    private float m_urgentChargeTimer;
    private float m_urgentChargeTime = 4.0f;
    [SerializeField]
    private bool m_urgentChargeBonus;
    public bool urgentChargeBonus { get { return m_urgentChargeBonus; } set { m_urgentChargeBonus = value; } }
    [SerializeField]
    private bool m_urgentRangeIn;
    public bool urgentRangeIn { get { return m_urgentRangeIn; } }

    public Text urgentText;
    public Image urgentImage;
    public Sprite urgentOffSprite;
    public Sprite urgentOnSprite;
    public Animation urgentUIAnim;

    public GameObject urgentOnFX;
    public GameObject urgentMoveFX;

    void Start()
    {
        m_urgentChargeTimer = 0.0f;
        m_urgentChargeBonus = false;
    }

    void Update()
    {
        urgentText.text = "urgent Time : " + m_urgentChargeTimer;

        if(m_urgentChargeBonus)
        {
            m_urgentChargeTimer += Time.deltaTime;
            if (m_urgentChargeTimer >= m_urgentChargeTime)
            {
                BonusOff();
                m_urgentChargeTimer = 0.0f;
                return;
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (m_urgentChargeBonus)
            return;

        if (coll.tag == "ENEMYBULLET")
        {
            m_urgentRangeIn = true;
        }
    }

    public void BonusOn()
    {
        Instantiate(urgentOnFX, this.transform);

        m_urgentChargeBonus = true;
        urgentImage.sprite = urgentOnSprite;
        m_urgentRangeIn = false;

        urgentMoveFX.SetActive(true);
    }

    public void BonusOff()
    {
        m_urgentChargeBonus = false;
        urgentImage.sprite = urgentOffSprite;

        urgentMoveFX.SetActive(false);
    }
}
