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

    public GameObject m_objectDestroyPrefab;
    public GameObject m_playerDestroyPrefab;

    void Update()
    {
        RaycastObject();
        this.transform.Translate(this.transform.forward * Time.smoothDeltaTime * speed);
    }

    private void OnTriggerEnter(Collider coll)
    {
        switch(coll.tag)
        {
            case "PLAYER":
                if(coll.GetComponent<PlayerMoveController>().teleported)
                {
                    return;
                }

                if (coll.GetComponent<PlayerStateController>().curState == PlayerState.ATTACKED ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.NOCK ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.INVI ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.RETIRE)
                {
                    Destroy();
                    return;
                }

                Instantiate(m_playerDestroyPrefab, this.transform.position, Quaternion.LookRotation(coll.transform.GetChild(0).forward * -1f));
                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                pc.DecreaseHP(attack);
                Destroy();
                break;
        }
    }

    private void RaycastObject()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, 1f, wallCollisionMask))
        {
            Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
            Destroy();
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

    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
    }

    public override void Destroy()
    {
        ObjectManager.PullObject("EnemyBullet", this.gameObject);
    }
}
