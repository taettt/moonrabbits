using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSChecker : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    public Text fpsText;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        string text = string.Format("{0:0.0} ms, {1:0.} fps", msec, fps);
        fpsText.text = text;
    }
}
