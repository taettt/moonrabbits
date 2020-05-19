using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgentManager : MonoBehaviour
{
    private bool m_isRanged;
    private float m_urgentChargeTimer;
    private float m_urgentChargeTime = 4.0f;
    [SerializeField]
    private int m_urgentChargeBonus;
    public int urgentChargeBonus { get { return m_urgentChargeBonus; } set { m_urgentChargeBonus = value; } }

    public Text urgentText;

    public PlayerMoveController mc;

    void Start()
    {
        m_isRanged = false;
        m_urgentChargeTimer = 0.0f;
        m_urgentChargeBonus = 0;
    }

    void Update()
    {
        urgentText.text = "urgent Time : " + m_urgentChargeTimer;

        if(m_urgentChargeBonus != 0)
        {
            m_urgentChargeTimer += Time.deltaTime;
            if (m_urgentChargeTimer >= m_urgentChargeTime)
            {
                m_urgentChargeBonus--;
                m_urgentChargeTimer = 0.0f;
                return;
            }
        }

        if(m_isRanged)
        {
            m_urgentChargeBonus++;
            m_isRanged = false;
            return;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (mc.teleported)
        {
            if (coll.tag == "ENEMYBULLET")
            {
                m_isRanged = true;
            }
        }
    }
}
