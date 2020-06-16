using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackState
{
    NONE,
    SHORT,
    DELAY,
    CHARGE_0,
    CHARGE_1,
    CHARGE_2,
    LONG,
    NUM
}

enum ChargeFXState
{
    READY,
    CHARGING,
    FULL,
    SHOOT,
    NUM
}

// fx func 만들기 정리하기
public class Weapon : MonoBehaviour
{
    // default
    public GameObject bulletPrefab;
    public float attackSpeed;
    public int attackValue_default;
    public int attackValue_charge_1;
    public int attackValue_charge_2;
    public Transform shootHoleTr;
    public Transform bulletParent;
    private Vector3 m_shootPos;
    private Vector3 m_dirVec;

    // charge
    private float[] chargeStep = new float[3] { 0.0f, 1.0f, 2.0f };
    [SerializeField]
    private float curChargeTime = 0.0f;

    public Text chargeText;

    [SerializeField]
    private AttackState curState;

    public Transform playerTr;
    public Transform playerModelTr;
    public Transform weaponTr;
    public Transform bossTr;
    public Animator animator;
    public PlayerMoveController pmc;
    public PlayerController pc;
    public PlayerStateController psc;
    public UrgentManager um;
    public PlayerSkillManager sm;

    // ready, charging, full, shoot, attack1, attack2
    public GameObject[] m_chargeFX;

    void Update()
    {
        if (psc.curState == PlayerState.RETIRE || psc.curState == PlayerState.NOCK || psc.curState == PlayerState.ATTACKED
            || pmc.teleported)
        {
            ResetState();
            return;
        }

        UpdateFX();
        UpdateUI();

        if(Input.GetMouseButtonDown(1))
        {
            if (curState != AttackState.NONE || sm.isFaSkillPlaying)
                return;

            curState = AttackState.CHARGE_0;
        }
        
        // 여기에서 check, state는 상관x
        else if(Input.GetMouseButton(1))
        {
            if (curState == AttackState.DELAY || sm.isFaSkillPlaying)
            {
                ResetState();
                return;
            }

            curChargeTime += Time.deltaTime;

            if (curChargeTime >= chargeStep[2])
            {
                curState = AttackState.CHARGE_2;
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);
            }
            else if(curChargeTime < chargeStep[2] && curChargeTime >= chargeStep[1])
            {
                curState = AttackState.CHARGE_1;
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(true);
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            m_chargeFX[(int)ChargeFXState.FULL].SetActive(false);
            m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);

            if (curState == AttackState.CHARGE_1 || curState == AttackState.CHARGE_2)
            {
                StartCoroutine(Attack_Charge_Check(Input.mousePosition));
            }
            else
            {
                ResetState();
            }
        }

        else if(Input.GetMouseButton(0))
        {
            if (curState != AttackState.NONE || Input.GetMouseButton(1) || sm.isFaSkillPlaying)
                return;

            if(um.urgentChargeBonus)
            {
                StartCoroutine(Attack_Urgent_Charge(Input.mousePosition));
            }
            else
            {
                StartCoroutine(Attack_Long_Check(Input.mousePosition));
            }
        }

        else if(Input.GetKeyDown(KeyCode.Q))
        {
            if(sm.isFASkillOn)
            {
                sm.isFaSkillPlaying = true ? false : true;
                StartCoroutine(Attack_FA_Check(bossTr.position));
            }
        }
    }

    private void UpdateFX()
    {
        if(Mathf.Approximately(curChargeTime, chargeStep[1])|| Mathf.Approximately(curChargeTime, chargeStep[2]))
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
        if (curState != AttackState.CHARGE_0)
            return;

        if (curChargeTime > chargeStep[2])
        {
            chargeText.text = "Charge : Charge_2";
        }
        else if (curChargeTime > chargeStep[1] && curChargeTime <= chargeStep[2])
        {
            chargeText.text = "Charge : Charge_1";
        }
        else
        {
            chargeText.text = "Charge : Charge_0";
        }
    }

    private void ResetState()
    {
        curState = AttackState.NONE;
        curChargeTime = 0.0f;
        StopAllCoroutines();

        m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);
        m_chargeFX[(int)ChargeFXState.FULL].SetActive(false);
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
        mousePos.y = 1.5f;
        Vector3 pos = ConversionPos(mousePos);
        curState = AttackState.LONG;

        Attack_Long(pos, false);

        yield return new WaitForSeconds(0.25f);

        curState = AttackState.NONE;
    }

    private IEnumerator Attack_Charge_Check(Vector3 mousePos)
    {
        Vector3 pos = ConversionPos(mousePos);

        if (curState == AttackState.CHARGE_2)
        {
            Attack_Charge(pos, attackValue_charge_2, 2);
        }
        else if (curState == AttackState.CHARGE_1)
        {
            Attack_Charge(pos, attackValue_charge_1, 1);
        }

        curChargeTime = 0.0f;
        curState = AttackState.DELAY;
        yield return new WaitForSeconds(1.0f);
        curState = AttackState.NONE;
    }

    private IEnumerator Attack_Urgent_Charge(Vector3 mousePos)
    {
        curState = AttackState.CHARGE_2;

        Vector3 pos = ConversionPos(mousePos);

        Attack_Charge(pos, 4, 2);
        um.BonusOff();

        yield return new WaitForSeconds(1.0f);
        curState = AttackState.NONE;

    }

    private IEnumerator Attack_FA_Check(Vector3 bossPos)
    {
        while (sm.isFaSkillPlaying && sm.isFASkillOn)
        {
            Attack_Long(bossPos, true);

            yield return new WaitForSeconds(0.1f);
            sm.DecreaseFAGague();
        }
    }

    private void Attack_Charge(Vector3 targetPos, int attackValue, int kind)
    {
        GameObject go = Instantiate(m_chargeFX[(int)ChargeFXState.SHOOT], this.transform.position, Quaternion.LookRotation(m_dirVec));
        go.transform.SetParent(weaponTr.transform);

        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual((PlayerBulletKind)kind);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, attackSpeed, attackValue);

        playerModelTr.transform.forward = m_dirVec;
    }

    private void Attack_Long(Vector3 targetPos, bool isFA)
    {
        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual(PlayerBulletKind.DEF);
        if (isFA)
        {
            bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, attackSpeed * 2, attackValue_default);
        }
        else
        {
            bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, attackSpeed, attackValue_default);
        }

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
