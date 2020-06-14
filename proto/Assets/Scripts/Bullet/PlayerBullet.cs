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
    public GameObject destroyPrefab_3;

    public GameObject[] chargingPrefab;

    void Update()
    {
        RaycastObject();
        this.transform.Translate(dir * Time.smoothDeltaTime * speed, Space.World);
    }

    void OnTriggerEnter(Collider coll)
    {
        switch(coll.tag)
        {
            case "ENEMY":
                if (m_kind == PlayerBulletKind.DEF)
                {
                    Instantiate(destroyPrefab_2, this.transform.position, Quaternion.LookRotation(dir));
                }
                else
                {
                    Instantiate(destroyPrefab_3, this.transform.position, Quaternion.LookRotation(dir));
                }

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
                Debug.Log("Player Bullet Collided With Minion");
                if (m_kind == PlayerBulletKind.DEF)
                {
                    Instantiate(destroyPrefab_2, this.transform.position, Quaternion.LookRotation(dir));
                }
                else
                {
                    Instantiate(destroyPrefab_3, this.transform.position, Quaternion.LookRotation(dir));
                }

                //coll.GetComponent<MinionController>().DropSeed();
                //Destroy(coll.gameObject);
                Destroy();
                break;
            case "WALL":
                Instantiate(destroyPrefab_1, this.transform.position, Quaternion.LookRotation(dir));
                Destroy();
                break;
        }
    }

    private void RaycastObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 0.9f, wallCollisionMask))
        {
            Instantiate(destroyPrefab_1, this.transform.position, Quaternion.LookRotation(dir));
            Destroy();
        }
    }

    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, int attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
        this.transform.localPosition += new Vector3(0.0f, 1.0f, 0.0f);
    }

    public void SetVisual(PlayerBulletKind kind)
    {
        m_kind = kind;

        switch (m_kind)
        {
            case PlayerBulletKind.DEF:
                this.transform.GetChild(0).gameObject.SetActive(true);
                chargingPrefab[0].SetActive(false);
                chargingPrefab[1].SetActive(false);

                this.GetComponent<BoxCollider>().size = new Vector3(0.8f, 0.1f, 1f);
                break;
            case PlayerBulletKind.CHARGE_1:
                this.transform.GetChild(0).gameObject.SetActive(false);
                chargingPrefab[0].SetActive(true);
                chargingPrefab[1].SetActive(false);

                this.GetComponent<BoxCollider>().size = new Vector3(1.0f, 0.1f, 1f);
                break;
            case PlayerBulletKind.CHARGE_2:
                this.transform.GetChild(0).gameObject.SetActive(false);
                chargingPrefab[0].SetActive(false);
                chargingPrefab[1].SetActive(true);

                this.GetComponent<BoxCollider>().size = new Vector3(1.2f, 0.1f, 1f);
                break;
        }
    }

    public override void Destroy()
    {
        base.Destroy();

        ObjectManager.PullObject("PlayerBullet", this.gameObject);
    }
}
