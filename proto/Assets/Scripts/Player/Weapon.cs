using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackState
{
    NONE,
    SHORT,
    CHARGE,
    LONG,
    NUM
}

public class Weapon : MonoBehaviour
{
    // default
    public GameObject bulletPrefab;
    private float speed;
    private float attack;
    public Transform shootHoleTr;
    public Transform bulletParent;
    private Vector3 m_shootPos;
    private Vector3 m_dirVec;

    // charge
    private float[] chargeStep = new float[3] { 0.0f, 0.5f, 1.0f };
    [SerializeField]
    private float curChargeTime = 0.0f;

    [SerializeField]
    private AttackState curState;

    public Transform playerTr;
    public PlayerMoveController pmc;
    public PlayerController pc;
    public PlayerStateController psc;

    void Update()
    {
        if(Input.GetMouseButton(2))
        {
            if (psc.curState == PlayerState.ATTACKED)
                return;

            curState = AttackState.CHARGE;
            pmc.moveSpeed = 3.2f;

            curChargeTime += Time.deltaTime;
        }

        else if (Input.GetMouseButtonUp(2))
        {
            if (psc.curState == PlayerState.ATTACKED)
                return;

            if (curState == AttackState.CHARGE)
            {
                StartCoroutine(Attack_Charge_Check(Input.mousePosition));
            }
        }

        else if(Input.GetMouseButton(1))
        {
            if(curState==AttackState.NONE)
            {
                StartCoroutine(Attack_Long_Check(Input.mousePosition));
            }
        }
        else
        {
            pmc.moveSpeed = 4.0f;
        }
    }

    private IEnumerator Attack_Long_Check(Vector3 mousePos)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        curState = AttackState.LONG;
        pmc.moveSpeed = 3.2f;

        float values = pc.OutputAbsorpBuff();
        Color color = pc.OutputAbsorpColor();
        if (values != 0.0f && color.a != 0.0f)
        {
            speed = 18.0f;
            attack = values;

            Attack_Long(pos, true, color);
        }
        else
        {
            speed = 18.0f;
            attack = 2.0f;

            Attack_Long(pos, false, color);
        }

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
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));

            curChargeTime = 0.0f;
            float values = pc.OutputAbsorpBuff();
            Color color = pc.OutputAbsorpColor();

            if (curChargeTime > chargeStep[2])
            {
                if (values != 0.0f && color.a != 0.0f)
                {
                    speed = 18.0f;
                    attack = values;

                    pc.AbsorpValueDown(5);
                    Attack_Charge(pos, 4.0f, 2, true, color);
                }
                else
                {
                    speed = 18.0f;
                    attack = 2.0f;

                    Attack_Charge(pos, 4.0f, 2, false, color);
                }
            }
            else if (curChargeTime < chargeStep[2])
            {
                if (values != 0.0f && color.a != 0.0f)
                {
                    speed = 18.0f;
                    attack = values;

                    pc.AbsorpValueDown(10);
                    Attack_Charge(pos, 2.0f, 1, true, color);
                }
                else
                {
                    speed = 18.0f;
                    attack = 2.0f;

                    Attack_Charge(pos, 2.0f, 1, false, color);
                }
            }

            yield return new WaitForSeconds(0.25f);
            curState = AttackState.NONE;
        }
    }

    private void Attack_Charge(Vector3 targetPos, float attackPlus, int kind, bool absorpShot, Color color)
    {
        SetVectors(targetPos);

        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.transform.SetParent(bulletParent);
        bullet.GetComponent<PlayerBullet>().SetVisual((PlayerBulletKind)kind, absorpShot, color);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, speed, attack * attackPlus);

        playerTr.transform.forward = m_dirVec;
    }

    private void Attack_Long(Vector3 targetPos, bool absorpShot, Color color)
    {
        SetVectors(targetPos);

        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
        bullet.transform.SetParent(bulletParent);
        bullet.GetComponent<PlayerBullet>().SetVisual(PlayerBulletKind.DEF, absorpShot, color);
        bullet.GetComponent<PlayerBullet>().Spawn(m_shootPos, m_dirVec, speed, attack);

        playerTr.transform.forward = m_dirVec;
    }

    private void SetVectors(Vector3 targetPos)
    {
        m_shootPos = playerTr.position;
        m_dirVec = targetPos - m_shootPos;
        m_dirVec.y = 0.0f;
        m_dirVec = m_dirVec.normalized;
    }
}
