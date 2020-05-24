using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackState
{
    NONE,
    SHORT,
    CHARGE,
    LONG,
    NUM
}

enum ChargeFXState
{
    READY,
    CHARGING,
    FULL,
    SHOOT,
    CHARGE_1,
    CHARGE_2,
    NUM
}

// fx func 만들기 정리하기
public class Weapon : MonoBehaviour
{
    // default
    public GameObject bulletPrefab;
    private float speed;
    private int attack;
    public Transform shootHoleTr;
    public Transform bulletParent;
    private Vector3 m_shootPos;
    private Vector3 m_dirVec;

    // charge
    private float[] chargeStep = new float[3] { 0.0f, 0.5f, 1.0f };
    [SerializeField]
    private float curChargeTime = 0.0f;

    public Text chargeText;

    [SerializeField]
    private AttackState curState;

    public Transform playerTr;
    public Transform playerModelTr;
    public Transform weaponTr;
    public Animator animator;
    public PlayerMoveController pmc;
    public PlayerController pc;
    public PlayerStateController psc;
    public UrgentManager um;

    // ready, charging, full, shoot, attack1, attack2
    public GameObject[] m_chargeFX;

    void Update()
    {
        UpdateFX();

        if(Input.GetMouseButtonDown(0))
        {
            if (psc.curState == PlayerState.ATTACKED)
                return;

            curState = AttackState.CHARGE;
            pmc.moveSpeed = 5.6f;
        }

        else if(Input.GetMouseButton(0))
        {
            if (psc.curState == PlayerState.ATTACKED)
                return;

            curState = AttackState.CHARGE;
            pmc.moveSpeed = 5.6f;

            curChargeTime += Time.deltaTime;

            if (curChargeTime >= chargeStep[2])
            {
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);
            }
            else
            {
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(true);
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            m_chargeFX[(int)ChargeFXState.FULL].SetActive(false);
            m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);

            if (psc.curState == PlayerState.ATTACKED)
                return;

            if (curState == AttackState.CHARGE)
            {
                StartCoroutine(Attack_Charge_Check(Input.mousePosition));
            }
        }

        else if(Input.GetMouseButton(1))
        {
            if (curState != AttackState.NONE)
                return;

            if(um.urgentChargeBonus!=0)
            {
                StartCoroutine(Attack_Urgent_Charge(Input.mousePosition));
            }
            else
            {
                StartCoroutine(Attack_Long_Check(Input.mousePosition));
            }
        }
        else
        {
            pmc.moveSpeed = 8.0f;
        }
    }

    private void UpdateFX()
    {
        if(curChargeTime == chargeStep[1] || curChargeTime == chargeStep[2])
        {
            Instantiate(m_chargeFX[(int)ChargeFXState.READY], weaponTr);
        }

        else if(curChargeTime > chargeStep[2])
        {
            m_chargeFX[(int)ChargeFXState.FULL].SetActive(true);
        }
    }

    private void UpdateUI()
    {
        if (curState != AttackState.CHARGE)
            return;

        if (curChargeTime > chargeStep[2])
        {
            chargeText.text = "Charge : Charge_2";
        }
        else if (curChargeTime > chargeStep[1] && curChargeTime < chargeStep[2])
        {
            chargeText.text = "Charge : Charge_1";
        }
        else
        {
            chargeText.text = "Charge : Charge_0";
        }
    }

   private Vector3 ConversionPos(Vector3 mousePos)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.NameToLayer("MouseHit")))
        {
            mousePos = hit.point;
        }

        return mousePos;
    }

    private IEnumerator Attack_Long_Check(Vector3 mousePos)
    {
        Vector3 pos = ConversionPos(mousePos);
        curState = AttackState.LONG;
        pmc.moveSpeed = 5.6f;

        speed = 24.0f;
        attack = 2;

        Attack_Long(pos);

        yield return new WaitForSeconds(0.25f);

        curState = AttackState.NONE;
    }

    private IEnumerator Attack_Charge_Check(Vector3 mousePos)
    {
        if (curChargeTime <= chargeStep[1])
        {
            curState = AttackState.NONE;
            curChargeTime = 0.0f;

            yield return null;
        }
        else
        {
            Vector3 pos = ConversionPos(mousePos);

            curChargeTime = 0.0f;

            if (curChargeTime > chargeStep[2])
            {
                speed = 24.0f;
                attack = 2;

                Attack_Charge(pos, 4, 2);
            }
            else if (curChargeTime < chargeStep[2])
            {
                speed = 24.0f;
                attack = 2;

                Attack_Charge(pos, 2, 1);
            }

            yield return new WaitForSeconds(0.25f);
            curState = AttackState.NONE;
        }
    }

    private IEnumerator Attack_Urgent_Charge(Vector3 mousePos)
    {
        curState = AttackState.CHARGE;

        Vector3 pos = ConversionPos(mousePos);

        speed = 24.0f;
        attack = 2;

        Attack_Charge(pos, 4, 2);
        um.urgentChargeBonus--;

        yield return new WaitForSeconds(0.25f);
        curState = AttackState.NONE;

    }

    private void Attack_Charge(Vector3 targetPos, int attackPlus, int kind)
    {
        GameObject go = Instantiate(m_chargeFX[(int)ChargeFXState.SHOOT], this.transform.position, Quaternion.LookRotation(m_dirVec));
        go.transform.SetParent(this.transform);

        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual((PlayerBulletKind)kind);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, speed, attack * attackPlus);

        playerModelTr.transform.forward = m_dirVec;
    }

    private void Attack_Long(Vector3 targetPos)
    {
        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual(PlayerBulletKind.DEF);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, speed, attack);

        playerModelTr.transform.forward = m_dirVec;
    }

    private void SetVectors(Vector3 targetPos)
    {
        m_shootPos = playerTr.position;
        m_dirVec = targetPos - m_shootPos;
        m_dirVec.y = 0.0f;
        m_dirVec = m_dirVec.normalized;
    }
}
