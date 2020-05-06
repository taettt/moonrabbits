using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFX : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 1.0f);
    }
}
