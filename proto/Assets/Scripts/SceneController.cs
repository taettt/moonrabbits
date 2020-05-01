using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ChagneScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
