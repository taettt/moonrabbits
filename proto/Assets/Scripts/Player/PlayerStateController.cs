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

    public Text stateText;
    public GameManager gm;

    // timer
    private float[] m_delayTime = new float[5] { 0.3f, 0.4f, 5.0f, 3.0f, 1.0f };

    void Update()
    {
        stateText.text = "Player : " + m_curState;
    }

    public void SetState(PlayerState state)
    {
        m_curState = state;
        if (m_curState == PlayerState.IDLE)
            return;

        if(m_curState == PlayerState.RETIRE)
        {
            gm.PhaseRetry();
        }
        else if(m_curState == PlayerState.DEATH)
        {
            gm.GameOver();
        }

        if (m_curState == PlayerState.ATTACKED || m_curState == PlayerState.NOCK)
        {
            StartCoroutine(ProcessState(m_delayTime[(int)m_curState - 1], PlayerState.INVI));
        }
        else
        {
            StartCoroutine(ProcessState(m_delayTime[(int)m_curState - 1], PlayerState.IDLE));
        }
    }

    private IEnumerator ProcessState(float delay, PlayerState nextState)
    {
        yield return new WaitForSeconds(delay);
        SetState(nextState);
    }
}
