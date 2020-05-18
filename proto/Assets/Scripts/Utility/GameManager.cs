using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text finishText;
    public GameObject retryButton;

    public BossController bc;
    public PlayerController pc;

    public Boss[] bosses;

    public void GameOver()
    {
        finishText.text = "Game Over!";
        finishText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void GameClear()
    {
        finishText.text = "Game Clear!";
        finishText.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Retry()
    {
        finishText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        Time.timeScale = 1.0f;

        GameInitialize();
    }

    public void StopGame()
    {

    }

    public void GameInitialize()
    {
        pc.Initialize();
        bc.Initialize();
    }

    public void PhaseRetry()
    {
        bc.PhaseInit();
    }

    public void InitializeBoss(int index)
    {
        bc.ConnectBoss(bosses[index]);
    }
}
