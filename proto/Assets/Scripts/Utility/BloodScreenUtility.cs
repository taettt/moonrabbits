using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodScreenUtility : MonoBehaviour
{
    public Material mat;
    public Animator fadeAnim;
    public PlayerController pc;

    void Update()
    {
        float pos = Camera.main.nearClipPlane + 0.01f;
        this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * pos;
        float height = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2.0f;

        this.transform.localScale = new Vector3(height * Camera.main.aspect, height, 0.0f);

        if (this.gameObject.tag != "HIT")
        {
            if (this.gameObject.activeSelf)
            {
                mat.SetFloat("강도", (2.2f - (2.2f / pc.hp)));
            }
        }
    }

    public void PlayHitFX()
    {
        this.gameObject.SetActive(true);
        fadeAnim.Play("BloodScreenFade", -1, 0f);
    }
}
