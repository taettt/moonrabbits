using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCirculrSplitMagicSphere : EnemyBullet
{
    public bool isDestroyed;
    private EnemyBulletKind m_kind;
    Vector3 toPlayer;
    void Start()
    {
        b1C = FindObjectOfType<Boss_1Control>();
        isDestroyed = false;
        ShootCircular();
        toPlayer = (b1C.playerTr.position - this.transform.position).normalized;
        toPlayer.y = 0;
        
    }
    void Update()
    {
        //RaycastObject();

        this.transform.Translate(toPlayer * Time.smoothDeltaTime * 4f, Space.World);

    }

    private void OnTriggerEnter(Collider coll)
    {
        switch (coll.tag)
        {
            //case "PLAYER":
            //    if (coll.GetComponent<PlayerStateController>().curState == PlayerState.ATTACKED ||
            //        coll.GetComponent<PlayerStateController>().curState == PlayerState.NOCK ||
            //        coll.GetComponent<PlayerStateController>().curState == PlayerState.INVI ||
            //        coll.GetComponent<PlayerStateController>().curState == PlayerState.RETIRE)
            //    {
            //        Destroy();
            //        return;
            //    }

            //    Instantiate(m_playerDestroyPrefab, this.transform.position, Quaternion.LookRotation(coll.transform.GetChild(0).forward * -1f));
            //    PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
            //    pc.DecreaseHP(attack);
            //    Destroy();
            //    break;
            case "WALL":
                Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
                b1C.ForceExcutePhase();
                Destroy(this.gameObject);
                Debug.Log("magic sphere destroyed");
                break;
        }
    }

    //private void RaycastObject()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 0.9f, wallCollisionMask))
    //    {
    //        Debug.Log("hit");
    //        Instantiate(m_objectDestroyPrefab, this.transform.position, Quaternion.LookRotation(dir));
    //        Destroy();
    //    }
    //}

        /*
    public override void SetVisual(EnemyBulletKind kind)
    {
        base.SetVisual();
    }
    */

    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
        this.transform.localPosition += new Vector3(0.0f, 0.3f, 0.0f);
    }

    public override void Destroy()
    {
        ObjectManager.PullObject("EnemyBullet", this.gameObject);
    }

    public Boss_1Control b1C;

    private void ShootCircular()
    {
        for (float i = -180.0f; i <= 180.0f; i += 45.0f)
        {
            var bullet = ObjectManager.PushObject("EnemyBullet").GetComponent<EnemyBullet>();
            bullet.transform.SetParent(b1C.bc.bulletParent);
            bullet.transform.position = this.transform.position;
            bullet.Spawn(bullet.transform.position,
                Utility.GetDirection(i), 15.0f, 3);
            bullet.transform.rotation = Quaternion.identity;

        }

        Invoke("ShootCircular", 0.5f);
    }
}
