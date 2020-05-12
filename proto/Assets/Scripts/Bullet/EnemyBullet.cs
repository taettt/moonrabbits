using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBulletKind
{
    DEF,
    SPEC,
    NUM
}

public class EnemyBullet : Bullet
{
    private EnemyBulletKind m_kind;
    private bool m_direct = false;

    public GameObject m_objectDestroyPrefab;
    public GameObject m_playerDestroyPrefab;

    void Update()
    {
        this.transform.Translate(dir * Time.smoothDeltaTime * speed);
    }

    private void OnTriggerEnter(Collider coll)
    {
        switch(coll.tag)
        {
            case "PLAYER":
                if (coll.GetComponent<PlayerStateController>().curState == PlayerState.ATTACKED ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.NOCK ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.INVI ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.RETIRE)
                {
                    Destroy();
                    return;
                }

                Instantiate(m_playerDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir * -1f));
                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                pc.DecreaseHP(attack);
                Destroy();
                break;
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        switch(coll.gameObject.tag)
        {
            case "WALL":
                Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
                Destroy();
                break;
            case "OBSTACLE":
                Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
                Destroy();
                break;
        }
    }

    public override void SetVisual()
    {
        base.SetVisual();

        switch(m_kind)
        {
            case EnemyBulletKind.DEF:
                break;
            case EnemyBulletKind.SPEC:
                break;
        }
    }

    // 외부(enemy)에서 설정하고 내부로 전달
    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
    }

    public void SetDirect(bool direct)
    {
        m_direct = direct;
    }

    public override void Destroy()
    {
        ObjectManager.PullObject("EnemyBullet", this.gameObject);
    }
}
