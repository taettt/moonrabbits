using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_1Control : BossControl
{
    public BossController bc;
    public BossPhase curPhase;

    public Transform playerTr;

    private Coroutine curCoroutine_F = null;
    private Coroutine curCoroutine_S = null;

    private bool m_isMoving;
    private Animator animator;

    [SerializeField]
    private int m_curPatternIndex = -1;
    private int m_curPatternCount = 0;
    private int[] m_patternMaxCount = new int[4] { 16, 10, 3, 4 };

    public Text patternText;
    public float patternTimer;

    private int m_curMoveIndex = 0;
    public Vector3[] m_pattern_1Moves;
    public Vector3 m_pattern_2Move;
    public Vector3[] m_pattern_3Moves;
    private float m_moveTimer = 0.0f;

    private float m_waiter = 0.0f;

    public GameObject minionPrefab;

    void Awake()
    {
        animator = this.transform.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (!m_isMoving)
            return;

        if (bc.init)
            return;

        patternTimer += Time.deltaTime;
        switch (m_curPatternIndex)
        {
            case 0:
                patternText.text = "Pattern 1 : " + patternTimer;
                Pattern1Move();
                break;
            case 1:
                patternText.text = "Pattern 2 : " + patternTimer;
                Pattern2Move();
                break;
            case 2:
                patternText.text = "Pattern 3 : " + patternTimer;
                //Pattern3Move();
                break;
        }
    }

    public override void Initialize()
    {
        m_isMoving = false;
        m_moveTimer = 0.0f;
        m_curPatternIndex = -1;
        m_curPatternCount = 0;
        m_curMoveIndex = 0;

        phaseRandQueue = new Queue<int>();

        Invoke("ExcutePhase", 2.0f);
    }

    public void RetryInit()
    {
        StopAllCoroutines();
        curCoroutine_F = null;
        curCoroutine_S = null;

        m_isMoving = false;
        m_moveTimer = 0.0f;
        m_curPatternIndex = -1;
        m_curPatternCount = 0;
        m_curMoveIndex = 0;

        bc.init = false;
        Invoke("ExcutePhase", 2.0f);
    }

    private void ExcutePhase()
    {
        if (!randomRound)
        {
            m_curPatternIndex = m_curPatternIndex == 2 ? 3 : m_curPatternIndex + 1;
            m_curPatternCount = 0;
            if (m_curPatternIndex == 3)
            {
                m_curPatternCount = 0;
                randomRound = true;

                SetRandQueue(3);
                m_curPatternIndex = phaseRandQueue.Dequeue();
            }
        }
        else
        {
            if (phaseRandQueue.Count <= 0 || phaseRandQueue == null)
                SetRandQueue(3);

            m_curPatternIndex = phaseRandQueue.Dequeue();
        }

        bc.init = false;

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

        patternTimer = 0.0f;
    }

    private void Pattern1Shoot()
    {
        if (bc.init)
        {
            return;
        }

        //animator.SetBool("IsAttack", true);

        for (float i = -180.0f; i <= 180.0f; i += 45.0f)
        {
            bc.ShootBullet(Utility.GetDirection(i), bc.attackSpeed, bc.damage);
        }

        if (m_curMoveIndex < m_pattern_1Moves.Length)
        {
            Invoke("Pattern1Shoot", 0.5f);
        }
        else
        {
            m_curMoveIndex = 0;
            //animator.SetBool("IsAttack", false);
            Invoke("ExcutePhase", 2);
        }
    }

    private void Pattern1Move()
    {
        if (bc.init)
        {
            return;
        }

        if (m_curMoveIndex >= m_pattern_1Moves.Length)
        {
            m_isMoving = false;
            return;
        }

        //animator.SetBool("IsAttack", false);
        animator.SetBool("IsRun", true);
        m_moveTimer += Time.deltaTime;

        if (m_curMoveIndex + 1 != m_pattern_1Moves.Length)
        {
            this.transform.rotation = Quaternion.LookRotation((m_pattern_1Moves[m_curMoveIndex + 1]).normalized);
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position,
            m_pattern_1Moves[m_curMoveIndex], Time.deltaTime * bc.moveSpeed * 0.5f);

        if (m_moveTimer >= 1.0f)
        {
            animator.SetBool("IsRun", false);
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
                    bc.ShootBullet(Utility.GetDirection(i + j), bc.attackSpeed, bc.damage * 2);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private IEnumerator Pattern2_2Shoot()
    {
        yield return new WaitForSeconds(2.0f);
        //animator.SetBool("IsAttack", true);

        while (m_curPatternCount < m_patternMaxCount[m_curPatternIndex])
        {
            Vector3 dir = playerTr.position - this.transform.position;
            dir = dir.normalized;

            for (float i = -30.0f; i <= 30.0f; i += 30.0f)
            {
                if (i == 0.0f)
                {
                    bc.ShootBullet(dir, bc.attackSpeed * 1.5f, bc.damage * 2);
                }
                else
                {
                    var quat = Quaternion.Euler(0.0f, i, 0.0f);
                    Vector3 reDir = quat * dir;
                    reDir.y = 0.0f;
                    reDir = reDir.normalized;
                    bc.ShootBullet(reDir, bc.attackSpeed * 1.5f, bc.damage * 2);
                }
            }

            yield return new WaitForSeconds(1.0f);
            m_curPatternCount++;
        }

        if (m_curPatternCount >= m_patternMaxCount[m_curPatternIndex])
        {
            //animator.SetBool("IsAttack", false);
            StopCoroutine(curCoroutine_F);
            Invoke("ExcutePhase", 2);
        }
    }

    private void Pattern2Move()
    {
        if (bc.init)
            return;

        m_moveTimer += Time.deltaTime;

        animator.SetBool("IsRun", true);
        this.transform.position = Vector3.MoveTowards(this.transform.position,
            m_pattern_2Move, Time.deltaTime * bc.moveSpeed * 2.0f);

        if (m_moveTimer >= 1.0f)
        {
            animator.SetBool("IsRun", false);
            m_moveTimer = 0.0f;
            m_isMoving = false;
        }
    }

    private void Pattern3Shoot()
    {
        if (bc.init)
            return;

        StopCoroutine(curCoroutine_F);

        //animator.SetBool("IsAttack", true);

        Vector3 dir = playerTr.position - this.transform.position;
        dir = dir.normalized;
        Debug.DrawRay(this.transform.position, dir * 10.0f, Color.red);

        bc.ShootBullet(dir, bc.attackSpeed * 2.0f, bc.damage * 4);

        m_curPatternCount++;
        if (m_curPatternCount < m_patternMaxCount[m_curPatternIndex])
        {
            //animator.SetBool("IsAttack", false);
            Invoke("Pattern3Shoot", 1.0f);
        }
        else
        {
            //animator.SetBool("IsAttack", false);
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
                if (bc.playerTr.position.z < 0.0f)
                {
                    targetPos = new Vector3((playerTr.position.x), 0.2f, playerTr.position.z + 20.0f);
                }
                else
                {
                    targetPos = new Vector3((playerTr.position.x), 0.2f, (playerTr.position.z + 20.0f) * -1f);
                }

                animator.SetBool("IsRun", true);
                this.transform.position = Vector3.MoveTowards(this.transform.position,
                    targetPos, Time.deltaTime * bc.moveSpeed);
                this.transform.forward = dir;

                m_moveTimer += Time.deltaTime;
                if (m_moveTimer > 0.3f)
                {
                    animator.SetBool("IsRun", false);
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
        float dis = Vector3.Distance(bc.playerTr.position, this.transform.position);
        Vector3 dir = (bc.playerTr.position - this.transform.position).normalized;

        if (dis <= 15.0f)
        {
            if (bc.playerTr.position.z < 0.0f)
            {
                targetPos = new Vector3((bc.playerTr.position.x), 0f, bc.playerTr.position.z + 20.0f);
            }
            else
            {
                targetPos = new Vector3((bc.playerTr.position.x), 0f, (bc.playerTr.position.z + 20.0f) * -1f);
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position,
                targetPos, Time.deltaTime * bc.moveSpeed);
            this.transform.forward = dir;

            m_moveTimer += Time.deltaTime;
            if (m_moveTimer > 1.0f)
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

    // weapon도 가져와서 shootbullet
    private IEnumerator Pattern4Shoot()
    {
        while (m_curPatternCount < m_patternMaxCount[m_curPatternIndex])
        {
            for (float i = 60.0f; i <= -60.0f; i -= 30.0f)
            {
                bc.ShootBullet(Quaternion.Euler(0.0f, i, 0.0f) * this.transform.forward, bc.attackSpeed, bc.damage);
                //weapon.ShootBullet(Quaternion.Euler(0.0f, i, 0.0f) * weapon.tranform.forward, bc.attackSpeed, bc.damage);

                yield return new WaitForSeconds(1.0f);
                m_curPatternCount++;
            }

        }
    }

    // 거리 비교
    private int Pattern4_CheckDistance()
    {
        float[] lengths = new float[m_pattern_3Moves.Length];
        int selectedIndex = 0;

        for (int i = 0; i < m_pattern_3Moves.Length; i++)
        {
            lengths[i] = Vector3.Distance(this.transform.position, m_pattern_3Moves[i]);
        }

        for (int i = 1; i < lengths.Length; i++)
        {
            if (lengths[i - 1] > lengths[i])
            {
                selectedIndex = i;
            }
        }

        return selectedIndex;
    }

    // weapon index도 설정함
    // 발사 dir shoot에서 설정
    private void Pattern4Mvoe()
    {
        if (m_curMoveIndex >= m_pattern_3Moves.Length)
        {
            m_isMoving = false;
            return;
        }

        m_curMoveIndex = Pattern4_CheckDistance();
        this.transform.position = Vector3.MoveTowards(this.transform.position, m_pattern_3Moves[m_curMoveIndex], Time.deltaTime * bc.moveSpeed);

        m_moveTimer += Time.deltaTime;
        if (m_moveTimer > 1.0f)
        {
            m_moveTimer = 0.0f;
            // weapon spawn
            curCoroutine_F = StartCoroutine(Pattern4Shoot());
        }
    }

    private void Pattern_MinionSpawn()
    {

    }

    // rand spawn(필드 내 정해진 범위 내에서 하는걸로 의견), 보스 앞,양옆 spawn)
    private void Pattern_MinionDistanceCheck()
    {
        Vector3[] spawnPos = new Vector3[]
        {
            new Vector3(this.transform.position.x - 3.0f, 1.2f, this.transform.position.z),
            new Vector3(this.transform.position.x + 3.0f, 1.2f, this.transform.position.z),
            new Vector3(this.transform.position.x, 1.2f, this.transform.position.z - 3.0f)
        };

        for (int i = 0; i < 3; i++)
        {
            Instantiate(minionPrefab, spawnPos[i], Quaternion.identity);
        }
    }
}
