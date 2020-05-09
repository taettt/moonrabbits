using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFX : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = this.transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!ps.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
