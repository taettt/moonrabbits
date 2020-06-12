using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if(!ps.isPlaying)
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
