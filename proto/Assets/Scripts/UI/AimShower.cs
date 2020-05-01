using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimShower : MonoBehaviour
{
    void Update()
    {
        this.transform.position = Input.mousePosition;
    }
}
