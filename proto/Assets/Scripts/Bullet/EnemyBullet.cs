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

    void Update()
    {
        if (!m_direct)
        {
            this.transform.Translate(Vector3.forward * Time.smoothDeltaTime * speed);
        }
        else
        {
            this.transform.Translate(dir * Time.smoothDeltaTime * speed);
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        switch(coll.gameObject.tag)
        {
            case "WALL":
                Destroy();
                break;
            case "OBSTACLE":
                Destroy();
                break;
            case "PLAYER":
                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                if (coll.gameObject.GetComponent<PlayerStateController>().curState == PlayerState.ABSORP &&
                    pc.absorpValue < 30)
                {
                    if(pc.absorpValue>20)
                    {
                        float damage = (10 - (30 - pc.absorpValue));
                        pc.InputAbsorpBuff(0, 4.0f, (30-pc.absorpValue), this.GetComponent<MeshRenderer>().material.color);

                        if(damage==1)
                        {
                            pc.DecreaseHP(attack);
                        }
                        else
                        {
                            pc.DecreaseHP(attack / 2);
                        }
                    }
                    else if(pc.absorpValue <= 20)
                    {
                        pc.InputAbsorpBuff(0, 4.0f, 10, this.GetComponent<MeshRenderer>().material.color);
                    }
                }
                else
                {
                    pc.DecreaseHP(attack);
                }

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
    public override void Spawn(Vector3 spawnPos, Vector3 dir, float speed, float attack)
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
