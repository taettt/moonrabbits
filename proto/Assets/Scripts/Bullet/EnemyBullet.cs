using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBulletKind
{
    DEF,
    SPEC,
    MINI,
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
        this.transform.Translate(dir * Time.smoothDeltaTime * speed, Space.World);
    }

    private void OnTriggerEnter(Collider coll)
    {
        
        switch(coll.tag)
        {
            case "PLAYER":
                if (
                    coll.GetComponent<PlayerMoveController>().teleported)
                {
                    return;
                }
                else if (coll.GetComponent<PlayerStateController>().curState == PlayerState.ATTACKED ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.NOCK ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.INVI ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.RETIRE)
                {
                    Destroy();
                    return;
                }

                GameObject go = Instantiate(m_playerDestroyPrefab, coll.transform);
                go.transform.rotation = Quaternion.LookRotation(coll.transform.GetChild(0).forward * -1f);
                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                pc.DecreaseHP(attack);
                Destroy();
                break;
            case "WALL":
                Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
                Destroy();
                break;
        }
    }

    private void RaycastObject()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position, this.transform.forward, out hit, 0.9f, wallCollisionMask))
        {
            //Debug.Log("raycast hit");
            Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
            Destroy();
        }
    }

    public virtual void SetVisual(EnemyBulletKind kind)
    {
        m_kind = kind;

        switch(m_kind)
        {
            case EnemyBulletKind.DEF:
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case EnemyBulletKind.SPEC:
                break;
            case EnemyBulletKind.MINI:
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }

    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
        this.transform.localPosition -= new Vector3(0.0f, 0.3f, 0.0f);
    }

    public override void Destroy()
    {
        ObjectManager.PullObject("EnemyBullet", this.gameObject);
    }
}
