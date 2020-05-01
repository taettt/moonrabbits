﻿using System.Collections;
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

    void Update()
    {
        this.transform.Translate(Vector3.forward * Time.smoothDeltaTime * speed);
    }

    void OnTriggerEnter(Collider coll)
    {
        switch(coll.tag)
        {
            case "WALL":
                Instantiate(destroyPrefab_1, this.transform.position, Quaternion.identity);
                Destroy();
                break;
            case "OBSTACLE":
                Instantiate(destroyPrefab_1, this.transform.position, Quaternion.identity);
                Destroy();
                break;
            case "ENEMY":
                Instantiate(destroyPrefab_2, this.transform.position, Quaternion.identity);

                _EnemyController ec = coll.GetComponent<_EnemyController>();
                ec.DecreaseHP(attack);
                if (m_kind == PlayerBulletKind.CHARGE_2)
                {
                    ec.SetMoveSpeed(0.5f);
                    ec.SetAttackSpeed(0.5f);
                }
                Destroy();
                break;
        }
    }

    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, float attack)
    {
        base.Spawn(spawnPos, dir, speed, attack);
    }

    public void SetVisual(PlayerBulletKind kind, bool isAbsorp, Color color)
    {
        m_kind = kind;

        if (isAbsorp)
        {
            Color currentColor = this.GetComponent<MeshRenderer>().material.color;
            this.GetComponent<MeshRenderer>().material.color = new Color(
                color.r * currentColor.r, color.g * currentColor.g, color.b * currentColor.b);
        }

        switch (m_kind)
        {
            case PlayerBulletKind.DEF:
                break;
            case PlayerBulletKind.CHARGE_1:
                this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            case PlayerBulletKind.CHARGE_2:
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
