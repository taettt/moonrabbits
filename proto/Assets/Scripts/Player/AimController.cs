using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    private Vector3 m_curPos;
    private Vector3 m_prevPos;
    private Vector3 m_dir;
    private float m_rad = 4.0f;
    private float m_moveSpeed = 1.0f;

    public Transform playerTr;

    void Update()
    {
        m_curPos = Input.mousePosition;
        m_curPos = Camera.main.ScreenToWorldPoint(new Vector3(m_curPos.x, m_curPos.y, -Camera.main.transform.position.z));
        m_dir = m_curPos - playerTr.position;
        m_dir = m_dir.normalized;

        AimMove();
    }

    private void AimMove()
    {
        this.transform.eulerAngles = new Vector3(0.0f, 90.0f + GetDeg(new Vector2(playerTr.position.x, playerTr.position.z),
            new Vector2(this.transform.position.x, this.transform.position.z)), 0.0f);
        Vector3 toDir = m_dir * m_rad;

        this.transform.position = new Vector3(playerTr.position.x + toDir.x,
            this.transform.position.y, playerTr.position.z + toDir.z);
    }

    private float GetDeg(Vector2 start, Vector2 end)
    {
        return Mathf.Atan2(start.y - end.y, start.x - end.x) * Mathf.Rad2Deg;
    }
}
