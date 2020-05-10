using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    IDLE,
    ATTACKED,
    NOCK,
    RETIRE,
    DEATH,
    INVI,
    NUM
}

public class PlayerStateController : MonoBehaviour
{
    private PlayerState m_curState;
    public PlayerState curState { get { return m_curState; } set { m_curState = value; } }

    public GameManager gm;

    // timer
    private float m_waiter = 0.0f;
    public float waiter { get { return m_waiter; } set { m_waiter = value; } }
    private float m_attackedTime = 0.3f;
    private float m_absorpTime = 0.3f;
    private float m_invincibleTime = 1.0f;
    private float m_knockTime = 0.4f;
    private float m_retireTime = 5.0f;
    private float m_dieTime = 3.0f;

    public Text stateText;

    // fxs
    public GameObject attackedFX;

    void Update()
    {
        ProcessState();
    }

    // 코루틴도 ㄱㅊ을듯?
    private void ProcessState()
    {
        switch (m_curState)
        {
            case PlayerState.IDLE:
                stateText.text = "STATE : IDLE " + m_waiter;
                m_waiter = 0.0f;
                break;
            case PlayerState.ATTACKED:
                // anim start
                stateText.text = "STATE : ATTACKED " + m_waiter;
                Instantiate(attackedFX, this.transform);
                m_waiter += Time.deltaTime;
                if (m_waiter > m_attackedTime)
                {
                    curState = PlayerState.INVI;
                    m_waiter = 0.0f;
                    return;
                }
                break;
            case PlayerState.NOCK:
                stateText.text = "STATE : NOCK " + m_waiter;
                // anim start
                // standup anim start
                m_waiter += Time.deltaTime;
                if (m_waiter > m_knockTime)
                {
                    curState = PlayerState.INVI;
                    m_waiter = 0.0f;
                    return;
                }
                break;
            case PlayerState.RETIRE:
                stateText.text = "STATE : RETIRE " + m_waiter;
                // anim start
                m_waiter += Time.deltaTime;
                if (m_waiter > m_retireTime)
                {
                    Debug.Log("retire");
                    gm.PhaseRetry();
                    curState = PlayerState.IDLE;
                    m_waiter = 0.0f;
                    return;
                }
                break;
            case PlayerState.DEATH:
                stateText.text = "STATE : DEATH " + m_waiter;
                // anim start
                gm.GameOver();
                break;
            case PlayerState.INVI:
                stateText.text = "STATE : INVI " + m_waiter;
                m_waiter += Time.deltaTime;
                if (m_waiter > m_invincibleTime)
                {
                    curState = PlayerState.IDLE;
                }
                break;
        }
    }
}
