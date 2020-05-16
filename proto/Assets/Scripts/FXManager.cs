using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        if (this.tag == "TELEPORTFX")
        {
            ps = this.transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
        }
        else
        {
            ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
        }
    }

    void Update()
    {
        if(!ps.isPlaying)
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
