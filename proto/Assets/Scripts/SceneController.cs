using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    void Update()
    {
        if(Input.anyKeyDown)
        {
            ChangeScene("AssstsScene");
        }
    }
    public static void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ChangeStageScene()
    {
        SceneManager.LoadScene(1);
    }
}
