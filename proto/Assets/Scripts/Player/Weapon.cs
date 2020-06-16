using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttackState
{
    NONE,
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
    public Transform playerUpTr;
    [Range(0, 30)]
    public float rotSpeed;
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
    public ParticleSystem[] ps = new ParticleSystem[4];

    void Update()
    {
        if (psc.curState == PlayerState.RETIRE || psc.curState == PlayerState.NOCK || psc.curState == PlayerState.ATTACKED
            || pmc.teleported)
        {
            ResetState();
            return;
        }

        if (curState != AttackState.NONE || Input.GetMouseButton(0))
        {
            animator.SetBool("IsAttackRunPossible", true);
        }

        if (!animator.GetBool("IsRun"))
        {
            if (curState != AttackState.NONE)
            {
                playerModelTr.rotation = Quaternion.Slerp(playerModelTr.rotation, Quaternion.LookRotation(m_dirVec), Time.deltaTime * rotSpeed);
            }

        }
        else if(animator.GetBool("IsAttackRunPossible"))
        {
            float angle = Quaternion.Angle((playerUpTr.rotation * Quaternion.LookRotation(m_dirVec)), playerUpTr.rotation);
            angle = Mathf.Abs(angle);
            if (angle < 75f)
            {
                playerUpTr.rotation = Quaternion.Slerp(playerUpTr.rotation, Quaternion.LookRotation(m_dirVec), Time.deltaTime * rotSpeed * 2);
            }
        }

        UpdateFX();
        UpdateUI();


        if (curState == AttackState.CHARGE_0 || curState == AttackState.CHARGE_1)
        {
            animator.SetBool("IsAttack", true);
            animator.SetFloat("AttackDelay", curChargeTime - 1.25f);
        }

        if(Input.GetMouseButtonDown(1))
        {
            if (curState != AttackState.NONE || sm.isFaSkillPlaying)
                return;

            curState = AttackState.CHARGE_0;
            m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(true);
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
                animator.SetFloat("AttackDelay", 0.75f);
                curState = AttackState.CHARGE_2;
                m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);
                m_chargeFX[(int)ChargeFXState.FULL].SetActive(true);
            }
            else if(curChargeTime < chargeStep[2] && curChargeTime >= chargeStep[1])
            {
                curState = AttackState.CHARGE_1;
                if (Mathf.Abs(curChargeTime - chargeStep[1]) < 0.02f)
                {
                    for (int i = 0; i < ps.Length; i++)
                    {
                        ps[i].Play();
                    }
                }
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            m_chargeFX[(int)ChargeFXState.FULL].SetActive(false);
            m_chargeFX[(int)ChargeFXState.CHARGING].SetActive(false);

            if (curState == AttackState.CHARGE_1 || curState == AttackState.CHARGE_2)
            {
                animator.SetFloat("AttackDelay", 1f);
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

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(sm.isFASkillOn)
            {
                if(sm.isFaSkillPlaying)
                {
                    sm.isFaSkillPlaying = false;
                }
                else
                {
                    sm.isFaSkillPlaying = true;
                }
                StartCoroutine(Attack_FA_Check(bossTr.position));
            }
        }
    }

    private void UpdateFX()
    {
        if( Mathf.Abs(curChargeTime - chargeStep[2]) < 0.005f)
        {
            Instantiate(m_chargeFX[(int)ChargeFXState.READY], weaponTr);
        }
        else if(Mathf.Abs(curChargeTime - chargeStep[1]) < 0.005f)
        {
            m_chargeFX[(int)ChargeFXState.CHARGING].gameObject.SetActive(true);
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
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsAttackRunPossible", false);
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
        animator.SetFloat("AttackDelay", 0.75f);

        Attack_Long(pos, false);

        yield return new WaitForSeconds(0.25f);

        curState = AttackState.NONE;
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsAttackRunPossible", false);
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
        animator.SetFloat("AttackDelay", 1f);
        yield return new WaitForSeconds(1.0f);
        curState = AttackState.NONE;
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsAttackRunPossible", false);
    }

    private IEnumerator Attack_Urgent_Charge(Vector3 mousePos)
    {
        curState = AttackState.CHARGE_2;
        animator.SetBool("IsAttack", true);
        animator.SetFloat("AttackDelay", 0.75f);

        Vector3 pos = ConversionPos(mousePos);

        Attack_Charge(pos, 4, 2);
        um.BonusOff();

        yield return new WaitForSeconds(1.0f);
        curState = AttackState.NONE;
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsAttackRunPossible", false);
    }

    private IEnumerator Attack_FA_Check(Vector3 bossPos)
    {
        while (sm.isFaSkillPlaying && sm.isFASkillOn)
        {
            animator.SetBool("IsAttack", true);
            animator.SetFloat("AttackDelay", 0.75f);
            Attack_Long(bossPos, true);

            yield return new WaitForSeconds(0.1f);
            sm.DecreaseFAGague();
        }

        animator.SetBool("IsAttack", false);
        animator.SetBool("IsAttackRunPossible", false);
    }

    private void Attack_Charge(Vector3 targetPos, int attackValue, int kind)
    {
        GameObject go = Instantiate(m_chargeFX[(int)ChargeFXState.SHOOT], this.transform.position, Quaternion.LookRotation(m_dirVec));
        go.transform.SetParent(weaponTr.transform);

        SetVectors(targetPos);

        var bullet = ObjectManager.PushObject("PlayerBullet").GetComponent<PlayerBullet>();
        bullet.GetComponent<PlayerBullet>().SetVisual((PlayerBulletKind)kind);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, attackSpeed, attackValue);
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
    }

    private void SetVectors(Vector3 targetPos)
    {
        m_shootPos = playerTr.position;
        m_dirVec = targetPos - m_shootPos;
        m_dirVec.y = 0.0f;
        m_dirVec = m_dirVec.normalized;
    }
}
