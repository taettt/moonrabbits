using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _CameraController : MonoBehaviour
{
    public Vector3 rot = new Vector3(60.0f, 45.0f, 0.0f);
    public Vector3 pos = new Vector3(-11.0f, 18.0f, -11.0f);

    public Transform playerTr;

    void LateUpdate()
    {
        this.transform.position = playerTr.position + pos;
        this.transform.localRotation = Quaternion.Euler(rot);
    }
}
