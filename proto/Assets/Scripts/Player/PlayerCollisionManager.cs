using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    private CameraController cm;
    public float zoomInSize;
    public float originSize = 7.5f;
    public float zoomOutSize;

    [SerializeField]
    private bool isCollEnemy;

    void Awake()
    {
        cm = Camera.main.GetComponent<CameraController>();

        isCollEnemy = false;
    }

    void OnTriggerStay(Collider coll)
    {
        if(coll.tag=="ENEMY")
        {
            isCollEnemy = true;
            cm.ControlSize(zoomInSize);
        }
        else if(coll.tag=="WALL")
        {
            if (isCollEnemy)
                return;

            cm.ControlSize(zoomOutSize);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if(coll.tag=="ENEMY")
        {
            isCollEnemy = false;
            cm.ControlSize(originSize);
        }
        else if(coll.tag=="WALL")
        {
            cm.ControlSize(originSize);
        }
    }
}
