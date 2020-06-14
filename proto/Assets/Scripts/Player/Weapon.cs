using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackState
{
    NONE,
    SHORT,
    DELAY,
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
    private float m_attackDelay;
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
    public Transform bossTr;
    public Transform weaponTr;
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
            || pmc.teleported || sm.isFASkillPlaying)
        {
            ResetState();
            return;
        }

        UpdateFX();
        UpdateUI();

        if (curState == AttackState.CHARGE)
        {
            animator.SetBool("IsAttack", true);
            animator.SetFloat("AttackDelay", curChargeTime - 0.25f);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (curState != AttackState.NONE || sm.isFASkillPlaying)
                return;

            curState = AttackState.CHARGE;
        }
        

        else if(Input.GetMouseButton(1))
        {
            if (curState == AttackState.DELAY || sm.isFASkillPlaying)
                return;

            curChargeTime += Time.deltaTime;

            if (curChargeTime >= chargeStep[2])
            {
                animator.SetFloat("AttackDelay", 0.75f);
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);
            }
            else
            {
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(true);
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            m_chargeFX[(int)ChargeFXState.FULL].SetActive(false);
            m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);

            if (curState == AttackState.CHARGE)
            {
                StartCoroutine(Attack_Charge_Check(Input.mousePosition));
            }
        }

        else if(Input.GetMouseButton(0))
        {
            if (curState != AttackState.NONE || Input.GetMouseButton(1) || sm.isFASkillPlaying)
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
            StartCoroutine(Attack_FA_Check());
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
        if (curState != AttackState.CHARGE)
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

        animator.SetBool("IsAttack", true);
        m_attackDelay = 1.0f;
        animator.SetFloat("AttackDelay", m_attackDelay);

        Attack_Long(pos);

        yield return new WaitForSeconds(0.25f);

        curState = AttackState.NONE;
        animator.SetBool("IsAttack", false);
        m_attackDelay = 0.0f;
    }

    private IEnumerator Attack_Charge_Check(Vector3 mousePos)
    {
        if (curChargeTime <= chargeStep[1])
        {
            curState = AttackState.NONE;
            curChargeTime = 0.0f;

            animator.SetBool("IsAttack", false);

            yield return null;
        }
        else
        {
            Vector3 pos = ConversionPos(mousePos);

            if (curChargeTime > chargeStep[2])
            {
                Attack_Charge(pos, attackValue_charge_2, 2);
                Debug.Log("charge2");
            }
            else if (curChargeTime <= chargeStep[2])
            {
                Attack_Charge(pos, attackValue_charge_1, 1);
                Debug.Log("charge1");
            }

            curChargeTime = 0.0f;
            curState = AttackState.DELAY;
            animator.SetFloat("AttackDelay", 1f);
            yield return new WaitForSeconds(1.0f);
            curState = AttackState.NONE;
            animator.SetBool("IsAttack", false);
        }
    }

    private IEnumerator Attack_Urgent_Charge(Vector3 mousePos)
    {
        curState = AttackState.CHARGE;

        Vector3 pos = ConversionPos(mousePos);

        animator.SetBool("IsAttack", true);
        m_attackDelay = 1.0f;
        animator.SetFloat("AttackDelay", m_attackDelay);
        Attack_Charge(pos, attackValue_charge_2, 2);
        um.BonusOff();

        yield return new WaitForSeconds(1.0f);
        curState = AttackState.NONE;
        animator.SetBool("IsAttack", false);
        m_attackDelay = 0.0f;
    }

    private IEnumerator Attack_FA_Check()
    {
        while (sm.m_isFASkillOn)
        {
            Attack_FA(bossTr.position);

            yield return new WaitForSeconds(0.1f);
            sm.DecreseFASkillGague();
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

    private void Attack_Long(Vector3 targetPos)
    {
        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual(PlayerBulletKind.DEF);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, attackSpeed, attackValue_default);

        playerModelTr.transform.forward = m_dirVec;
    }

    private void Attack_FA(Vector3 targetPos)
    {
        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual(PlayerBulletKind.DEF);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, attackSpeed * 2, attackValue_default);

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
