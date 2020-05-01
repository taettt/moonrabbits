using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBullet : MonoBehaviour
{
    private Vector3 m_dir;
    private float m_speed;
    private float attack;

    private float m_rad;
    private float m_curAngle = 10.0f;
    private float m_angleLimit;

    void Start()
    {
        //StartCoroutine(ScaleUp());
    }

    void Update()
    {
        this.transform.Translate(m_dir * Time.smoothDeltaTime * m_speed);
    }

    private void OnCollisionEnter(Collision coll)
    {
        switch (coll.gameObject.tag)
        {
            case "WALL":
                Destroy();
                break;
            case "OBSTACLE":
                Destroy();
                break;
            case "PLAYER":
                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                pc.DecreaseHP(attack);
                Destroy();
                break;
        }
    }

    private void Destroy()
    {
        //StopCoroutine(ScaleUp());
        Destroy(this.gameObject);
    }

    private IEnumerator ScaleUp()
    {
        while (m_curAngle <= m_angleLimit)
        {
            this.transform.localScale = new Vector3((Mathf.PI * 2.0f * m_rad) * (m_curAngle / 360.0f),
            0.1f, 1.0f);

            yield return new WaitForSeconds(0.3f);
            m_curAngle++;
        }
    }

    public void SetStatus(Vector3 dir, float speed, float attack, float rad, float angleLimit)
    {
        m_dir = dir;
        m_dir.z = 0.0f;
        m_speed = speed;
        this.attack = attack;

        m_rad = rad;
        m_angleLimit = angleLimit;
    }
}
