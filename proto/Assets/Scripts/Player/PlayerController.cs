using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private int m_hp;
    public int hp { get { return m_hp; } }
    private int m_life;
    public int life { get { return m_life; } }

    private int m_attackedMaxDamage = 4;
    public int attackedMaxDamage { get { return m_attackedMaxDamage; } }

    // attacked
    private bool m_isAttacked;
    public bool isAttacked { get { return m_isAttacked; } set { m_isAttacked = value; } }
    private bool m_isLifeDown;
    public bool isLifeDown { get { return m_isLifeDown; } set { m_isLifeDown = value; } }

    public Transform playerModelTr;
    public Animator animator;
    public PlayerStateController sc;
    public PlayerMoveController mc;

    public GameObject attackedFX;

    void Awake()
    {
        Initialize();
    }

    private IEnumerator KnockbackCoroutine(float knockVal, Vector3 dir)
    {
        float timer = 0.0f;
        RaycastHit hit;

        while (timer < 0.4f)
        {
            timer += Time.deltaTime;
            if (Physics.Raycast(this.transform.position, playerModelTr.forward, out hit, knockVal))
            {
                this.transform.Translate(new Vector3(hit.point.x, 0.0f, hit.point.z) * Time.deltaTime);
            }
            else
            {
                this.transform.Translate(dir * knockVal * -1f * Time.deltaTime);
            }
        }

        yield return 0;
    }

    public void Initialize()
    {
        m_hp = 40;
        m_life = 3;
        sc.curState = PlayerState.IDLE;

        m_isAttacked = false;
        m_isLifeDown = false;
    }

    public void DecreaseHP(int value)
    {
        if (sc.curState == PlayerState.ATTACKED || sc.curState == PlayerState.INVI
            || sc.curState == PlayerState.RETIRE || mc.teleported)
            return;

        Instantiate(attackedFX, this.transform);
        if (m_hp - value <= 0)
        {
            m_life -= 1;
            m_hp = 0;
            if (m_life < 0)
            {
                sc.SetState(PlayerState.DEATH);
            }
            else
            {
                sc.SetState(PlayerState.RETIRE);
                m_hp = 40;
            }
        }

        else
        {
            if(value < m_attackedMaxDamage)
            {
                sc.SetState(PlayerState.ATTACKED);
                StartCoroutine(KnockbackCoroutine(5.0f, mc.playerModelTr.forward));
            }
            else
            {
                SetKoncked(4.0f, mc.playerModelTr.forward * -1f);
            }

            m_isAttacked = true;
            m_hp -= value;
            sc.SetHpState(m_hp);
        }
    }

    public void SetKoncked(float knockVal, Vector3 dir)
    {
        sc.SetState(PlayerState.NOCK);
        animator.SetTrigger("IsAttacked");
        StartCoroutine(KnockbackCoroutine(knockVal, dir));
    }
}
