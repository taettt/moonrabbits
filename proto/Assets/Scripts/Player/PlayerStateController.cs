﻿using System.Collections;
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

    public SkinnedMeshRenderer renderer;
    public Color attackColor;
    public Color originColor;

    private int curPlayerHp = 40;
    public int hpLimit;
    public GameObject hitFX;
    public GameObject lowHpFX;
    public BloodScreenUtility bsu;

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
        {
            renderer.materials[0].SetColor("_EmissionColor", originColor);
            hitFX.SetActive(false);
            return;
        }

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
            renderer.materials[0].SetColor("_EmissionColor", attackColor);
            bsu.PlayHitFX();
            if (curPlayerHp <= hpLimit)
            {
                lowHpFX.SetActive(true);
            }
        }
        else
        {
            StartCoroutine(ProcessState(m_delayTime[(int)m_curState - 1], PlayerState.IDLE));
        }
    }

    public void SetHpState(int hp)
    {
        curPlayerHp = hp;
    }

    private IEnumerator ProcessState(float delay, PlayerState nextState)
    {
        yield return new WaitForSeconds(delay);
        SetState(nextState);
    }
}
