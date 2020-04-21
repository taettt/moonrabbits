using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 페이즈 6
// 시점 상공뷰로 변하면서(안대면 사이즈값 높여서) 위쪽에서 떨어뜨리는? 떨어뜨리는 지점 지정은 일단 스테이지 범위 내 랜덤으로
public class BoomShoot : MonoBehaviour
{
    public float m_speed;
    private float m_fireRate = 0.5f;
    private float nextFire = 0.0f;

    private bool m_isColl = false;

    public GameObject sphere;
    private float[] dirs = new float[4] { 60.0f, 120.0f, 180.0f, 240.0f };

    void Update()
    {
        if (!m_isColl)
        {
            this.transform.Translate(Vector3.down * m_speed);
        }
        else
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + m_fireRate;
                for (int i = 0; i < 4; i++)
                {
                    var bullet = ObjectPool_Enemy.PushObject_e();
                    bullet.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 5.0f,
                        this.transform.position.z);
                    bullet.transform.rotation = this.transform.rotation;
                    //bullet.Shoot(GetDirection(dirs[i]), 0.5f);
                }
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.tag=="STAGE")
        {
            m_isColl = true;
            sphere.SetActive(false);
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
}
