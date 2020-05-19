using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ChangeStageScene()
    {
        SceneManager.LoadScene(1);
    }
}
