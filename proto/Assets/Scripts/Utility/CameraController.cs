using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 pos = new Vector3(-11.0f, 18.0f, -11.0f);
    public float offset;
    public float speed;
    public float zoomDis;

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
        //tr.position = new Vector3(playerTr.position.x, playerTr.position.y + offset, playerTr.position.z);
        this.transform.position = playerTr.position + pos;

        //DistanceDirect();
    }

    private void DistanceDirect()
    {
        float distance = Vector3.Distance(playerTr.position, enemyTr.position);
        if(distance<zoomDis)
        {
            ControlSize(7.0f);
        }
        else
        {
            ControlSize(12.0f);
        }
    }

    public void ControlSize(float size)
    {
        thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize,
            size, Time.deltaTime * speed);
    }

    public void ControlAngleX(float angle)
    {
        Vector3 curRot = new Vector3(tr.rotation.x, tr.rotation.y, tr.rotation.z);
        tr.eulerAngles =Vector3.Lerp(curRot, new Vector3(angle, curRot.y, curRot.z), Time.deltaTime * speed);
    }

    public void ControlAngleY(float angle)
    {
        Vector3 curRot = new Vector3(tr.rotation.x, tr.rotation.y, tr.rotation.z);
        tr.eulerAngles = Vector3.Lerp(curRot, new Vector3(curRot.x, angle, curRot.z), Time.deltaTime * speed);
    }
}
