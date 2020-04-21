using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyState
{
    IDLE,
    ATTACKED,
    NOCK,
    RETIRE,
    DEATH,
    NUM
}

public class EnemyStateController : MonoBehaviour
{
    private EnemyState m_curState;
    public EnemyState curState { get { return m_curState; } set { m_curState = value; } }

    public Text stateText;

    public GameManager gm;

    // timer
    private float m_waiter = 0.0f;
    private float m_attackedTime = 0.3f;
    private float m_knockTime = 0.4f;
    private float m_retireTime = 5.0f;
    private float m_dieTime = 3.0f;

    void Update()
    {
        ProcessState();
    }

    private void ProcessState()
    {
        switch (curState)
        {
            case EnemyState.IDLE:
                stateText.text = "STATE : IDLE" + m_waiter;
                m_waiter = 0.0f;
                break;
            case EnemyState.ATTACKED:
                // anim start
                stateText.text = "STATE : ATTACKED" + m_waiter;
                m_waiter += Time.deltaTime;
                if (m_waiter > m_attackedTime)
                {
                    m_curState = EnemyState.IDLE;
                    m_waiter = 0.0f;
                    return;
                }
                break;
            case EnemyState.NOCK:
                stateText.text = "STATE : NOCK" + m_waiter;
                m_waiter += Time.deltaTime;
                if(m_waiter > m_knockTime)
                {
                    curState = EnemyState.NOCK;
                    m_waiter = 0.0f;
                    return;
                }
                break;
            case EnemyState.RETIRE:
                stateText.text = "STATE : RETIRE" + m_waiter;
                // anim start
                m_waiter += Time.deltaTime;
                if(m_waiter > m_retireTime)
                {
                    curState = EnemyState.RETIRE;
                    m_waiter = 0.0f;
                    return;
                }
                break;
            case EnemyState.DEATH:
                stateText.text = "STATE : DEATH" + m_waiter;
                gm.GameClear();
                break;
        }
    }
}
