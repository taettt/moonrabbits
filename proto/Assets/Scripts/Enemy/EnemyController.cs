using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EnemyPhase
{
    DEF,
    P1, // 전방 60도 발사
    P2, // 360도 발사
    P3, // 동서남북 60도 발사(첫번째는 가만히 서서, 두번째는 틀어서)
    P4,
    P5,
    P6,
    NUM
}

public class EnemyController : MonoBehaviour
{
    private float m_hp;
    public float hp { get { return m_hp; } }
    public Slider hpSlider;

    [SerializeField]
    private EnemyPhase m_curPhase;
    private Coroutine curPhaseCoroutine;

    public Transform playerTr;
    public Transform bulletHoleTr;
    [SerializeField]
    private bool m_shootInDefault;
    [SerializeField]
    private bool[] m_shootInPhase;
    [SerializeField]
    private bool isMoving;

    public GameObject boomPrefab;

    public GameManager gm;

    void Awake()
    {
        Initailize();
    }

    void Update()
    {
        hpSlider.value = m_hp / 100.0f;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "PLAYERBULLET")
        {
            m_hp -= 5f;
            Destroy(coll.gameObject);

            CheckHP();
        }
        else if(coll.tag=="PLAYER")
        {
            coll.GetComponent<PlayerController>().DecreaseHP(5.0f);
        }
    }

    private void Initailize()
    {
        m_hp = 100.0f;
        m_curPhase = EnemyPhase.DEF;
        curPhaseCoroutine = StartCoroutine(DefaultShooting());
        m_shootInDefault = true;
        m_shootInPhase = new bool[(int)EnemyPhase.NUM - 1] { false, false, false, false, false, false };
        isMoving = false;
    }

    private void CheckHP()
    {
        if (m_hp == 70.0f)
        {
            m_curPhase = EnemyPhase.P3;
            ProcessPhase(m_curPhase);
        }

        else if (m_hp == 50.0f)
        {
            m_curPhase = EnemyPhase.P5;
            ProcessPhase(m_curPhase);
        }

        else if(m_hp == 20.0f)
        {
            m_curPhase = EnemyPhase.P6;
            ProcessPhase(m_curPhase);
        }

        else if(m_hp==0.0f)
        {
            gm.GameClear();
        }
    }

    private void ProcessPhase(EnemyPhase curPhase)
    {
        m_shootInDefault = false;
        if(curPhaseCoroutine!=null)
        {
            StopCoroutine(curPhaseCoroutine);
        }

        switch(curPhase)
        {
            case EnemyPhase.P1:
                m_shootInPhase[(int)EnemyPhase.P1 - 1] = true;
                PatternPhase_1();
                break;
            case EnemyPhase.P2:
                m_shootInPhase[(int)EnemyPhase.P1 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P2 - 1] = true;
                PatternPhase_2();
                break;
            case EnemyPhase.P3:
                m_shootInPhase[(int)EnemyPhase.P1 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P2 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P3 - 1] = true;
                PatternPhase_3();
                break;
            case EnemyPhase.P4:
                isMoving = true;

                m_shootInPhase[(int)EnemyPhase.P1 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P2 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P3 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P4 - 1] = true;
                PatternPhase_4();
                break;
            case EnemyPhase.P5:
                m_shootInPhase[(int)EnemyPhase.P1 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P2 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P3 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P4 - 1] = false;
                m_shootInPhase[(int)EnemyPhase.P5 - 1] = false;
                PatternPhase_5();
                break;
        }
    }

    private void PatternPhase_1()
    {
        float damp = 1.0f;
        float totalRange = 60.0f;
        int divCount = 6;

        curPhaseCoroutine = StartCoroutine(Phase_1Shooting(damp, totalRange, divCount));
    }

    private void PatternPhase_2()
    {
        float damp = 10.0f;
        float totalRange = 360.0f;
        float curAngle = 0.0f;

        curPhaseCoroutine = StartCoroutine(Phase_2Shooting(damp, totalRange, curAngle));
    }

    private void PatternPhase_3()
    {
        float damp = 10.0f;
        float totalRange = 60.0f;
        int divCount = 6;
        float[] lookDir = { 90.0f, 180.0f, 270.0f, 360.0f };
        bool forwardShooting = true;

        curPhaseCoroutine = StartCoroutine(Phase_3Shooting(damp, totalRange, divCount, lookDir, forwardShooting));
    }

    private void PatternPhase_4()
    {
        float[] dirs = new float[2] { 90.0f, 270.0f };
        Vector3 target = new Vector3(0.0f, 16.0f, -12.0f);

        //mc.curState = EnemyState.MOVE;
        //mc.curMoveState = EnemyMoveState.STR;
        //curPhaseCoroutine = StartCoroutine(Phase_4Shooting(dirs));
    }

    private void PatternPhase_5()
    {
        float damp = 10.0f;
        float totalRange = 60.0f;
        int divCount = 6;
        float[] lookDir = { 90.0f, 180.0f, 270.0f, 360.0f };
        bool forwardShooting = true;

        curPhaseCoroutine = StartCoroutine(Phase_5Shooting(damp, totalRange, divCount, lookDir, forwardShooting));
    }

    private void PatternPhase_6()
    {
        float y = 30.0f;
        Vector2 spawnPos = Random.insideUnitCircle * 30.0f;
        Vector3 pos = new Vector3(spawnPos.x, y, spawnPos.y);

        for (int i = 0; i < 3; i++)
        {
            Instantiate(boomPrefab, pos, Quaternion.identity);
        }
    }

    private IEnumerator DefaultShooting()
    {
        yield return new WaitForSeconds(3.0f);

        while (m_shootInDefault)
        {
            var bullet = ObjectPool_Enemy.PushObject_e();
            bullet.transform.position = this.transform.position;
            bullet.transform.rotation = this.transform.rotation;
            Vector3 dir = this.transform.position - playerTr.position;
            dir.y = 0.0f;
            dir = dir.normalized;
            //bullet.Shoot(dir, 0.5f);

            yield return new WaitForSeconds(3.0f);
        }
    }

    private void ReturnDefault()
    {
        m_curPhase = EnemyPhase.DEF;
        m_shootInDefault = true;
        StopAllCoroutines();
        StartCoroutine(DefaultShooting());
    }

    private IEnumerator Phase_1Shooting(float damp, float range, int div)
    {
        while (m_shootInPhase[(int)EnemyPhase.P1 - 1])
        {
            for (float i = range; i >= -(range); i-=10)
            {
                var bullet = ObjectPool_Enemy.PushObject_e();
                bullet.transform.position = this.transform.position;
                bullet.transform.rotation = this.transform.rotation;
                //bullet.Shoot(GetDirection(i), 0.5f);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(damp);
        }
    }

    private IEnumerator Phase_2Shooting(float damp, float range, float curAngle)
    {
        yield return new WaitForSeconds(3.0f);

        while (m_shootInPhase[(int)EnemyPhase.P2 - 1])
        {
            while (curAngle <= range)
            {
                var bullet = ObjectPool_Enemy.PushObject_e();
                bullet.transform.position = this.transform.position;
                bullet.transform.rotation = this.transform.rotation;
                //bullet.Shoot(GetDirection(curAngle * 5.0f), 0.8f);

                yield return new WaitForSeconds(0.1f);
                curAngle++;
            }

            yield return new WaitForSeconds(0.5f);
            curAngle = 0.0f;
        }
    }

    private IEnumerator Phase_3Shooting(float damp, float range, int div, float[] dirs, bool forwardShooting)
    {
        while (m_shootInPhase[(int)EnemyPhase.P3 - 1])
        {
            for (float i = 30.0f; i >= -30.0f; i-=10.0f)
            {
                for (int j = 0; j < dirs.Length; j++)
                {
                    if (forwardShooting)
                    {
                        var bullet1 = ObjectPool_Enemy.PushObject_e();
                        bullet1.transform.position = this.transform.position;
                        bullet1.transform.rotation = this.transform.rotation;
                        //bullet1.Shoot(GetDirection(i + dirs[j]), 0.5f);
                    }
                    else
                    {
                        var bullet1 = ObjectPool_Enemy.PushObject_e();
                        bullet1.transform.position = this.transform.position;
                        bullet1.transform.rotation = this.transform.rotation;
                        //bullet1.Shoot(GetDirection(i + (dirs[j] + 60.0f)), 0.5f);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(10.0f);
            forwardShooting = true ? false : true;
        }
    }

    // coroutine안되면 update문에서 처리하기...
    private IEnumerator Phase_4Shooting(float[] dirs)
    {
        yield return new WaitForSeconds(3.0f);

        while (m_shootInPhase[(int)EnemyPhase.P4-1])
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(0.0f, 16.0f, -12.0f), Time.fixedDeltaTime * 3.0f);

            for (int i = 0; i < dirs.Length; i++)
            {
                var bullet1 = ObjectPool_Enemy.PushObject_e();
                bullet1.transform.position = this.transform.position;
                bullet1.transform.rotation = this.transform.rotation;
                //bullet1.Shoot(GetDirection(dirs[i]), 0.5f);
            }

            yield return null;
        }
    }

    // phase3+player shoot
    private IEnumerator Phase_5Shooting(float damp, float range, int div, float[] dirs, bool forwardShooting)
    {
        yield return new WaitForSeconds(3.0f);

        while (m_shootInPhase[(int)EnemyPhase.P3 - 1])
        {
            for (float i = 30.0f; i >= -30.0f; i -= 10.0f)
            {
                for (int j = 0; j < dirs.Length; j++)
                {
                    if (forwardShooting)
                    {
                        var bullet1 = ObjectPool_Enemy.PushObject_e();
                        bullet1.transform.position = this.transform.position;
                        bullet1.transform.rotation = this.transform.rotation;
                        //bullet1.Shoot(GetDirection(i + dirs[j]), 0.5f);
                    }
                    else
                    {
                        var bullet1 = ObjectPool_Enemy.PushObject_e();
                        bullet1.transform.position = this.transform.position;
                        bullet1.transform.rotation = this.transform.rotation;
                        //bullet1.Shoot(GetDirection(i + (dirs[j] + 60.0f)), 0.5f);
                    }
                }

                var bullet = ObjectPool_Enemy.PushObject_e();
                bullet.transform.position = this.transform.position;
                bullet.transform.rotation = this.transform.rotation;
                Vector3 dir = this.transform.position - playerTr.position;
                dir.y = 0.0f;
                dir = dir.normalized;
                //bullet.Shoot(dir, 0.5f);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(3.0f);
            forwardShooting = true ? false : true;
        }
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
        m_hp -= value;
    }
}