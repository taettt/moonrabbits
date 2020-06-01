using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 pos = new Vector3(-11.0f, 18.0f, -11.0f);
    public float speed;
    public float zoomDis;
    public float zoomInFov;
    public float zoomOutFov;

    public Transform playerTr;
    public Transform enemyTr;
    private Transform tr;
    private Camera thisCamera;

    void Awake()
    {
        tr = this.transform;
        thisCamera = this.GetComponent<Camera>();
    }

    void Update()
    {
        tr.position = Vector3.Lerp(playerTr.position, enemyTr.position, 0.4f) + pos;

        //DistanceDirect();
    }

    private void DistanceDirect()
    {
        float dis = Vector3.Distance(playerTr.position, enemyTr.position);
        thisCamera.fieldOfView = Mathf.Lerp(zoomInFov, zoomOutFov, dis / 20.0f);
    }
}
