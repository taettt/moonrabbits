using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbsorpUIShower : MonoBehaviour
{
    public Transform playerTr;
    public float pivot;

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerTr.position);

        this.transform.position = new Vector3(screenPos.x, screenPos.y + pivot, screenPos.z);
    }
}
