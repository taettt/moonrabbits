using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BossPhase
{
    DEF,
    P1,
    P2,
    P3,
    NUM
}

public class BossController : MonoBehaviour
{
    private Boss m_curBoss;
    public Boss curBoss { get { return m_curBoss; } }
    private BossControl m_curBossControl;
    public Boss[] m_bossList;

    private int m_curHp;
    public int curHp { get { return m_curHp; } }
    private int m_curLife;
    public int curLife { get { return m_curLife; } }

    private float m_moveSpeed;
    public float moveSpeed { get { return m_moveSpeed; } }
    private float m_attackSpeed;
    public float attackSpeed { get { return m_attackSpeed; } }
    private int m_damage;
    public int damage { get { return m_damage; } }

    public GameObject wavePrefab;
    public Transform bulletParent;
    public Transform playerTr;
    public Transform weaponTr;

    private bool m_isLifeDown;
    public bool isLifeDown { get { return m_isLifeDown; } set { m_isLifeDown = value; } }
    private bool m_isMoving;
    [SerializeField]
    private bool m_init;
    public bool init { get { return m_init; } set { m_init = value; } }

    public EnemyStateController sc;

    void Awake()
    {
        InitBossInformation();
    }

    private void InitBossInformation()
    {
        m_curBoss = m_bossList[0];

        m_curHp = m_curBoss.m_maxHp;
        m_curBossControl = this.GetComponent<Boss_1Control>();
        m_curLife = m_curBoss.m_maxLife;
        m_moveSpeed = m_curBoss.m_moveSpeed;
        m_attackSpeed = m_curBoss.m_attackSpeed;
        m_damage = m_curBoss.m_damage;

        //ConnectBoss();
    }

    public void Initialize()
    {
        m_curBoss = m_bossList[0];

        m_curHp = m_curBoss.m_maxHp;
        m_curBossControl = this.GetComponent<Boss_1Control>();
        m_curLife = m_curBoss.m_maxLife;
        m_moveSpeed = m_curBoss.m_moveSpeed;
        m_attackSpeed = m_curBoss.m_attackSpeed;
        m_damage = m_curBoss.m_damage;

        m_curBossControl.Initialize();
    }

    public void PhaseInit()
    { 
        if (m_init)
            return;

        m_init = true;
        m_curBossControl.GetComponent<Boss_1Control>().RetryInit();
    }

    public void ShootBullet(Vector3 dir, float speed, int attackVal)
    {
        var bullet = ObjectManager.PushObject("EnemyBullet").GetComponent<EnemyBullet>();
        bullet.transform.SetParent(bulletParent);
        bullet.transform.position = weaponTr.position;
        bullet.Spawn(bullet.transform.position,
            dir, speed, attackVal);
    }

    private IEnumerator KnockbackCoroutine(float knockValue, Vector3 dir)
    {
        float timer = 0.0f;

        while (timer < 0.4f)
        {
            timer += Time.deltaTime;
            this.transform.Translate(dir * 4.0f * -1f * Time.deltaTime);
        }

        yield return 0;
    }

    /*
    private void ShootWave(Vector3 dir, float speed, int attackVal, float dis, float limit)
    {
        GameObject wave = Instantiate(wavePrefab);
        wave.transform.position = new Vector3(this.transform.position.x,
            0.3f, this.transform.position.z);
        wave.transform.rotation = this.transform.rotation;
        wave.GetComponent<WaveBullet>().SetStatus(dir, speed, attackVal, dis, limit);
    }
    */

    public Vector3 GetDirection(float angle)
    {
        Vector3 dir = Vector3.forward;
        var quat = Quaternion.Euler(0.0f, angle, 0.0f);
        Vector3 newDir = quat * dir;
        newDir.y = 0.0f;
        newDir = newDir.normalized;

        return newDir;
    }

    public void DecreaseHP(int value)
    {
        if (sc.curState != EnemyState.NOCK)
        {
            sc.SetState(EnemyState.ATTACKED);
        }

        if (m_curHp - value <= 0)
        {
            m_isLifeDown = true;
            m_curHp = 120;
            m_curLife -= 1;
            if (m_curLife <= 0)
            {
                sc.SetState(EnemyState.DEATH);
            }
            else
            {
                sc.SetState(EnemyState.RETIRE);
            }
        }

        else
        {
            m_curHp -= value;
        }
    }

    public void SetNockbacked(float nockVal, Vector3 dir)
    {
        sc.SetState(EnemyState.NOCK);
        StartCoroutine(KnockbackCoroutine(nockVal, dir));
    }

    public void SetMoveSpeed(float value)
    {
        if (m_moveSpeed <= 8.0f)
            return;

        m_moveSpeed *= value;
    }

    public void SetAttackSpeed(float value)
    {
        if (m_attackSpeed <= 8.0f)
            return;

        m_attackSpeed *= value;
    }

    public void SetAttackStatus(int value)
    {
        m_damage *= value;
    }

    public void ConnectBoss(Boss boss)
    {
        m_curBoss = boss;
        m_curHp = boss.m_maxHp;
        m_curLife = boss.m_maxLife;
        m_moveSpeed = boss.m_moveSpeed;
        m_attackSpeed = boss.m_attackSpeed;
        m_damage = boss.m_damage;
    }
}
