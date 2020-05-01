using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum _EnemyPhase
{
    DEF,
    P1,
    P2,
    P3,
    NUM
}

public class _EnemyController : MonoBehaviour
{
    private float m_hp;
    public float hp { get { return m_hp; } }
    private int m_life;
    public int life { get { return m_life; } }
    // 나중에 밑에 3개 수치 EnemyBullet에서 kind로 조정, 보정 값은 해당 스크립트에서 값 하나로 보정함
    private float m_moveSpeed;
    private float m_attackSpeed;
    private float m_attackStat;

    public GameObject wavePrefab;
    public Transform bulletParent;
    public Transform playerTr;

    private bool m_isLifeDown;
    public bool isLifeDown { get { return m_isLifeDown; } set { m_isLifeDown = value; } }
    private bool m_isMoving;
    [SerializeField]
    private bool m_init;

    [SerializeField]
    private bool m_randomRound = false;
    private float m_waiter = 0.0f;
    [SerializeField]
    private int m_curPatternIndex = -1;
    private int m_curPatternCount = 0;
    private int[] m_patternMaxCount = new int[3] { 16, 10, 3 };

    [SerializeField]
    private Queue<int> m_phaseRandQueue;
    private Coroutine curCoroutine_F = null;
    private Coroutine curCoroutine_S = null;

    [SerializeField]
    private int m_curMoveIndex = 0;
    public Vector3[] m_pattern_1Moves;
    public Vector3 m_pattern_2Move;
    private float m_moveTimer = 0.0f;

    public EnemyStateController sc;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        if (!m_isMoving)
            return;

        if (m_init)
            return;

        switch(m_curPatternIndex)
        {
            case 0:
                Pattern1Move();
                break;
            case 1:
                Pattern2Move();
                break;
            case 2:
                //Pattern3Move();
                break;
        }
    }

    public void Initialize()
    {
        m_hp = 120.0f;
        m_life = 2;
        m_moveSpeed = 16.0f;
        m_attackSpeed = 16.0f;
        m_attackStat = 2.0f;

        m_isMoving = false;

        StopAllCoroutines();
        m_moveTimer = 0.0f;
        m_phaseRandQueue = new Queue<int>();
        m_curPatternIndex = -1;
        m_curPatternCount = 0;
        m_curMoveIndex = 0;

        Invoke("ExcutePhase", 2.0f);
    }

    public void PhaseInit()
    {
        if (m_init)
            return;

        m_hp = 120.0f;
        m_moveSpeed = 16.0f;
        m_attackSpeed = 16.0f;
        m_attackStat = 2.0f;

        m_randomRound = false;
        m_isMoving = false;

        StopAllCoroutines();
        m_moveTimer = 0.0f;
        m_phaseRandQueue = new Queue<int>();
        m_curPatternIndex = -1;
        m_curPatternCount = 0;
        m_curMoveIndex = 0;

        m_init = true;
        Invoke("ExcutePhase", 1.0f);
    }

    private void SetRandQueue(int patternCount)
    {
        if (m_phaseRandQueue.Count > 0)
            return;

        int[] randArr = Utility.SetRandArry(patternCount);
        m_phaseRandQueue.Clear();

        int index = 0;
        while (m_phaseRandQueue.Count < patternCount)
        {
            m_phaseRandQueue.Enqueue(randArr[index]);
            index++;
        }
    }

    private void ExcutePhase()
    {
        if (!m_randomRound)
        {
            m_curPatternIndex = m_curPatternIndex == 2 ? 3 : m_curPatternIndex + 1;
            m_curPatternCount = 0;
            if (m_curPatternIndex == 3)
            {
                m_curPatternCount = 0;
                m_randomRound = true;

                SetRandQueue(3);
                m_curPatternIndex = m_phaseRandQueue.Dequeue();
            }
        }
        else
        {
            if (m_phaseRandQueue.Count <= 0 || m_phaseRandQueue == null)
                SetRandQueue(3);

            m_curPatternIndex = m_phaseRandQueue.Dequeue();
        }

        m_init = false;

        switch (m_curPatternIndex)
        {
            case 0:
                Invoke("Pattern1Shoot", 2.0f);
                m_isMoving = true;
                break;
            case 1:
                curCoroutine_F = StartCoroutine(Pattern2_1Shoot());
                curCoroutine_S = StartCoroutine(Pattern2_2Shoot());
                m_isMoving = true;
                break;
            case 2:
                StopCoroutine(curCoroutine_S);
                m_isMoving = true;
                curCoroutine_F = StartCoroutine(Pattern3Move());
                break;
            case 3:
                Invoke("ExcutePhase", 0.1f);
                break;
        }
    }

    private void Pattern1Shoot()
    {
        if (m_init)
        {
            Debug.Log("return");
            return;
        }

        for (float i = -180.0f; i <= 180.0f; i += 45.0f)
        {
            ShootBullet(GetDirection(i), m_attackSpeed, m_attackStat);
        }

        if(m_curMoveIndex < m_pattern_1Moves.Length)
        {
            Invoke("Pattern1Shoot", 0.5f);
        }
        else
        {
            m_curMoveIndex = 0;
            Invoke("ExcutePhase", 2);
        }
    }

    private void Pattern1Move()
    {
        if (m_init)
        {
            Debug.Log("return");
            return;
        }

        if (m_curMoveIndex>=m_pattern_1Moves.Length)
        {
            m_isMoving = false;
            return;
        }

        m_moveTimer += Time.deltaTime;

        this.transform.position = Vector3.MoveTowards(this.transform.position,
            m_pattern_1Moves[m_curMoveIndex], Time.deltaTime * m_moveSpeed * 0.5f);

        if (m_moveTimer >= 1.0f)
        {
            m_curMoveIndex++;
            m_moveTimer = 0.0f;
        }
    }
    
    private IEnumerator Pattern2_1Shoot()
    {
        yield return new WaitForSeconds(2.0f);

        while (m_curPatternCount < m_patternMaxCount[m_curPatternIndex])
        {
            for (float i = 10.0f; i <= 350.0f; i += 10.0f)
            {
                for (float j = 90.0f; j < 360.0; j += 90.0f)
                {
                    ShootBullet(GetDirection(i + j), m_attackSpeed, m_attackStat * 2.0f);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator Pattern2_2Shoot()
    {
        yield return new WaitForSeconds(2.0f);

        while (m_curPatternCount < m_patternMaxCount[m_curPatternIndex])
        {
            Vector3 dir = playerTr.position - this.transform.position;
            dir = dir.normalized;

            for (float i = -30.0f; i <= 30.0f; i += 30.0f)
            {
                if (i == 0.0f)
                {
                    ShootBullet(dir, m_attackSpeed * 1.5f, m_attackStat * 2.0f);
                }
                else
                {
                    var quat = Quaternion.Euler(0.0f, i, 0.0f);
                    Vector3 reDir = quat * dir;
                    reDir.y = 0.0f;
                    reDir = reDir.normalized;
                    ShootBullet(reDir, m_attackSpeed * 1.5f, m_attackStat * 2.0f);
                }
            }

            yield return new WaitForSeconds(1.0f);
            m_curPatternCount++;
        }

        if (m_curPatternCount >= m_patternMaxCount[m_curPatternIndex])
        {
            StopCoroutine(curCoroutine_F);
            Invoke("ExcutePhase", 2);
        }
    }

    private void Pattern2Move()
    {
        if (m_init)
            return;

        m_moveTimer += Time.deltaTime;

        this.transform.position = Vector3.MoveTowards(this.transform.position,
            m_pattern_2Move, Time.deltaTime * m_moveSpeed * 2.0f);

        if (m_moveTimer >= 1.0f)
        {
            m_moveTimer = 0.0f;
            m_isMoving = false;
        }
    }

    private void Pattern3Shoot()
    {
        if (m_init)
            return;

        StopCoroutine(curCoroutine_F);

        float dis = Vector3.Distance(this.transform.position, playerTr.position);
        Vector3 dir = playerTr.position - this.transform.position;
        dir = dir.normalized;

        //ShootWave(dir, m_attackSpeed, m_attackStat * 4.0f, dis, 90.0f);
        ShootBullet(dir, m_attackSpeed * 2.0f, m_attackStat * 4.0f);

        m_curPatternCount++;
        if (m_curPatternCount < m_patternMaxCount[m_curPatternIndex])
        {
            Invoke("Pattern3Shoot", 1.0f);
        }
        else
        {
            Invoke("ExcutePhase", 2.0f);
        }
    }

    private IEnumerator Pattern3Move()
    {
        while (m_isMoving)
        {
            Vector3 targetPos = Vector3.zero;
            float dis = Vector3.Distance(playerTr.position, this.transform.position);
            Vector3 dir = (playerTr.position - this.transform.position).normalized;

            if (dis <= 15.0f)
            {
                if (playerTr.position.z < 0.0f)
                {
                    targetPos = new Vector3((playerTr.position.x), 1.2f, playerTr.position.z + 20.0f);
                }
                else
                {
                    targetPos = new Vector3((playerTr.position.x), 1.2f, (playerTr.position.z + 20.0f) * -1f);
                }

                this.transform.position = Vector3.MoveTowards(this.transform.position,
                    targetPos, Time.deltaTime * m_moveSpeed);
                this.transform.forward = dir;

                m_moveTimer += Time.deltaTime;
                if (m_moveTimer > 1.0f)
                {
                    m_moveTimer = 0.0f;
                    m_isMoving = false;
                }
            }
            else
            {
                m_isMoving = false;
            }
        }

        yield return new WaitForSeconds(0.5f);
        Invoke("Pattern3Shoot", 0.5f);
    }

    private void _Pattern3Move()
    {
        Vector3 targetPos = Vector3.zero;
        float dis = Vector3.Distance(playerTr.position, this.transform.position);
        Vector3 dir = (playerTr.position - this.transform.position).normalized;

        if (dis <= 15.0f)
        {
            if(playerTr.position.z < 0.0f)
            {
                targetPos = new Vector3((playerTr.position.x), 1.2f, playerTr.position.z + 20.0f);
            }
            else
            {
                targetPos = new Vector3((playerTr.position.x), 1.2f, (playerTr.position.z + 20.0f) * -1f);
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position,
                targetPos, Time.deltaTime * m_moveSpeed);
            this.transform.forward = dir;

            m_moveTimer += Time.deltaTime;
            if(m_moveTimer>1.0f)
            {
                m_moveTimer = 0.0f;
                Invoke("Pattern3Shoot", 0.5f);
            }
        }
        else
        {
            Invoke("Pattern3Shoot", 0.5f);
        }
    }

    /*
    private IEnumerator Pattern_1Shoot()
    {
        isShooting = true;

        while (m_curPhaseLoopCount < m_phaseLoopCount[0])
        {
            for (float i = -180.0f; i <= 180.0f; i += 45.0f)
            {
                ShootBullet(GetDirection(i), 8.0f, 2.0f);
            }

            yield return new WaitForSeconds(0.5f);
            m_curPhaseLoopCount++;
        }

        m_curPhaseLoopCount = 0;
        isShooting = false;
        curPhase = (int)_EnemyPhase.P2;
    }

    private IEnumerator Pattern_2Shoot()
    {
        isShooting = true;

        while (m_curPhaseLoopCount <= m_phaseLoopCount[1])
        {
            for (float i = -30.0f; i <= 30.0f; i += 10.0f)
            {
                for (float j = 90.0f; j < 360.0; j += 90.0f)
                {
                    ShootBullet(GetDirection(i + j), 16.0f, 4.0f);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
        isShooting = false;
    }

    private IEnumerator Pattern_2Shoot_1()
    {
        isShooting = true;

        for (float i = -30.0f; i <= 30.0f; i += 10.0f)
        {
            for(float j=90.0f; j<360.0; j+=90.0f)
            {
                ShootBullet(GetDirection(i + j), 16.0f, 4.0f);
            }

            yield return new WaitForSeconds(0.1f);
        }
        isShooting = false;
    }

    private IEnumerator Pattern_2Shoot_2()
    {
        isShooting = true;

        while (true)
        {
            GameObject bullets = Instantiate(p_2_2bullet);
            bullets.transform.position = this.transform.position;
            Vector3 dir = (playerTr.position - this.transform.position).normalized;
            bullets.transform.forward = dir;

            Transform[] childs = bullets.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].parent = null;
            }

            Destroy(bullets);

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator _Pattern_2Shoot_2()
    {
        while (m_curPhaseLoopCount <= m_phaseLoopCount[1])
        {
            for (float i = -30.0f; i <=30.0f; i+=30.0f)
            {
                var bullet = ObjectPool_Enemy.PushObject();
                bullet.transform.position = this.transform.position;
                bullet.transform.rotation = this.transform.rotation;
                Vector3 dir = this.transform.position - playerTr.position;
                dir.y = 0.0f;
                dir = dir.normalized * -1;

                bullet.SetDirect(true);
                if (i == 0)
                {
                    bullet.Spawn(bullet.transform.position,
                        dir, 16.0f, 4.0f);
                }
                else
                {
                    bullet.Spawn(bullet.transform.position,
                       dir * i, 16.0f, 4.0f);
                }
            }

            yield return new WaitForSeconds(1.5f);
            m_curPhaseLoopCount++;
        }

        m_curPhaseLoopCount++;
        if (m_curPhaseLoopCount >= m_phaseLoopCount[(int)_EnemyPhase.P2 - 1])
        {
            m_curPhaseLoopCount = 0;
            isShooting = false;
            curPhase = (int)_EnemyPhase.P3;
        }

        isShooting = false;
        curPhase = (int)_EnemyPhase.P3;
    }

    private IEnumerator Pattern_3Shoot()
    {
        for(int i=0; i<3; i++)
        {
            GameObject bullet = Instantiate(p_3bullet);
            Vector3 dir = playerTr.position - this.transform.position;
            dir = dir.normalized;
            bullet.GetComponent<WaveBullet>().SetStatus(dir, 16.0f, 4.0f);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Pattern_1Move(Vector3 targetPos)
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position,
            targetPos, Time.deltaTime);
    }
    */

    private void ShootBullet(Vector3 dir, float speed, float attackVal)
    {
        var bullet = ObjectManager.PushObject("EnemyBullet").GetComponent<EnemyBullet>();
        bullet.transform.SetParent(bulletParent);
        bullet.transform.position = this.transform.position;
        bullet.transform.rotation = this.transform.rotation;
        bullet.Spawn(bullet.transform.position,
            dir, speed, attackVal);
    }

    private void ShootWave(Vector3 dir, float speed, float attackVal, float dis, float limit)
    {
        GameObject wave = Instantiate(wavePrefab);
        wave.transform.position = new Vector3(this.transform.position.x,
            0.3f, this.transform.position.z);
        wave.transform.rotation = this.transform.rotation;
        wave.GetComponent<WaveBullet>().SetStatus(dir, speed, attackVal, dis, limit);
    }

    private Vector3 GetDirection(float angle)
    {
        Vector3 dir = Vector3.forward;
        var quat = Quaternion.Euler(0.0f, angle, 0.0f);
        Vector3 newDir = quat * dir;
        newDir.y = 0.0f;
        newDir = newDir.normalized;

        return newDir;
    }

    public void DecreaseHP(float value)
    {
        sc.curState = EnemyState.ATTACKED;
        m_hp -= value;

        if(m_hp<=0.0f)
        {
            m_isLifeDown = true;
            sc.curState = EnemyState.RETIRE;
            m_hp = 120.0f;
            m_life -= 1;

            if(m_life<=0)
            {
                sc.curState = EnemyState.DEATH;
            }
        }
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

    public void SetAttackStatus(float value)
    {
        m_attackStat *= value;
    }
}
