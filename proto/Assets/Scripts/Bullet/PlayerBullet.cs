using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerBulletKind
{
    DEF,
    CHARGE_1,
    CHARGE_2,
    NUM
}

public class PlayerBullet : Bullet
{
    private PlayerBulletKind m_kind;
    public GameObject destroyPrefab_1;
    public GameObject destroyPrefab_2;

    public GameObject[] chargingPrefab;

    void Update()
    {
        this.transform.Translate(dir * Time.smoothDeltaTime * speed);
    }

    void OnTriggerEnter(Collider coll)
    {
        switch(coll.tag)
        {
            case "ENEMY":
                Instantiate(destroyPrefab_2, this.transform.position, Quaternion.LookRotation(dir));

                BossController ec = coll.GetComponent<BossController>();
                ec.DecreaseHP(attack);
                if (m_kind == PlayerBulletKind.CHARGE_2)
                {
                    ec.SetMoveSpeed(0.5f);
                    ec.SetAttackSpeed(0.5f);
                }
                Destroy();
                break;
            case "MINION":
                Instantiate(destroyPrefab_2, this.transform.position, Quaternion.LookRotation(dir));

                coll.GetComponent<MinionController>().DropSeed();
                Destroy(coll.gameObject);
                Destroy();
                break;
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        switch(coll.gameObject.tag)
        {
            case "WALL":
                Instantiate(destroyPrefab_1, this.transform.position, Quaternion.LookRotation(this.transform.forward * -1f));
                Destroy();
                break;
            case "OBSTACLE":
                Instantiate(destroyPrefab_1, this.transform.position, Quaternion.LookRotation(this.transform.forward * -1f));
                Destroy();
                break;
        }
    }

    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
    }

    public void SetVisual(PlayerBulletKind kind)
    {
        m_kind = kind;

        switch (m_kind)
        {
            case PlayerBulletKind.DEF:
                break;
            case PlayerBulletKind.CHARGE_1:
                this.transform.GetChild(0).gameObject.SetActive(false);
                chargingPrefab[0].SetActive(true);
                this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                break;
            case PlayerBulletKind.CHARGE_2:
                this.transform.GetChild(0).gameObject.SetActive(false);
                chargingPrefab[1].SetActive(true);
                this.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                break;
        }
    }

    public override void Destroy()
    {
        base.Destroy();

        ObjectManager.PullObject("PlayerBullet", this.gameObject);
    }
}
