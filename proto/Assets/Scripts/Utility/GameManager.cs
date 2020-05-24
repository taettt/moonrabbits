using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject overUI;
    public GameObject clearUI;

    public BossController bc;
    public PlayerController pc;

    public Boss[] bosses;

    public void GameOver()
    {
        overUI.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void GameClear()
    {
        clearUI.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Retry()
    {
        overUI.SetActive(false);
        Time.timeScale = 1.0f;

        GameInitialize();
    }

    public void OutGame()
    {
        Application.Quit();
    }

    public void StopGame()
    {
        SceneController.ChangeScene("MainScene");
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
