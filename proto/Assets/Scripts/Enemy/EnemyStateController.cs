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
    INIT,
    NUM
}

public class EnemyStateController : MonoBehaviour
{
    private EnemyState m_curState;
    public EnemyState curState { get { return m_curState; } set { m_curState = value; } }

    public Text stateText;

    public GameManager gm;

    // timer
    private float[] m_delayTime = new float[4] { 0.3f, 0.4f, 5.0f, 3.0f };

    void Update()
    {
        stateText.text = "Boss : " + m_curState;
    }

    public void SetState(EnemyState state)
    {
        m_curState = state;
        if (m_curState == EnemyState.IDLE)
            return;

        if(m_curState == EnemyState.INIT)
        {
            StartCoroutine(ProcessState(5.0f, EnemyState.IDLE));
        }

        if(m_curState == EnemyState.DEATH)
        {
            gm.GameClear();
        }

        // 수정 필요
        StartCoroutine(ProcessState(m_delayTime[(int)m_curState - 1], EnemyState.IDLE));
    }

    private IEnumerator ProcessState(float delay, EnemyState nextState)
    {
        yield return new WaitForSeconds(delay);
        SetState(nextState);
    }
}
