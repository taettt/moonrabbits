using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CurrentBossWeaponState
{
    DEFAULT,
    LASER

}

public class BossWeapon : MonoBehaviour
{
    public int currentBossWeaponState;
    public GameObject circularSplitSphere;
    public GameObject laserSphere;
    // Start is called before the first frame update
    void Awake()
    {
        LR = GetComponent<LineRenderer>();
        LR.enabled = false;
    }
    void Start()
    {
        onceDone = false;
        readyToShootLaser = false;
        rotateDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBossWeaponState == (int)CurrentBossWeaponState.DEFAULT)
        {
            LR.enabled = false;
            return;
        }
        else if(currentBossWeaponState == (int)CurrentBossWeaponState.LASER)
        {
            ShootLaser();
            if (this.transform.localRotation.y <= 0.0f)
            {
                rotateDirection = 1;

            }
            else if (this.transform.localRotation.eulerAngles.y >= 90.0f)
            {
                rotateDirection = -1;
            }

            RotateSphere();
        }
    }

    LineRenderer LR;
    float laserDistance = 15f;
    [SerializeField]
    LayerMask layer;
    [SerializeField]
    int rotateDirection;
    void ShootLaser()
    {
        Debug.Log("shooting laser from boss");

        LR.enabled = true;
        LR.SetPosition(0, this.transform.position);

        RaycastHit hit;
        Debug.DrawRay(this.transform.position, this.transform.forward * laserDistance, Color.red);

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, laserDistance, layer))
        {
            Debug.Log("laser hit");
            FindObjectOfType<PlayerController>().DecreaseHP(5);
            return;

        }
        Vector3 laserPoint = this.transform.position + this.transform.forward * laserDistance;
        LR.SetPosition(1, laserPoint);

    }
    GameObject duplication;
    bool onceDone;
    public bool readyToShootLaser;
    public void LaserPattern()
    {
        Debug.Log("laser pattern by boss weapon");
        if (onceDone == false)
        {
            Instantiate(laserSphere,this.transform.position,Quaternion.identity);
            onceDone = true;
        }

    }
    private void RotateSphere()
    {

        this.transform.Rotate(new Vector3(0f, 50f * rotateDirection, 0f) * Time.deltaTime);


    }
}
