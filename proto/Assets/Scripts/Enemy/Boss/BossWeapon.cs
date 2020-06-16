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
    bool isNegative;
    void Start()
    {
        onceDone = false;
        readyToShootLaser = false;
        rotateDirection = 1;
        laserDistance = 30.0f;
        sphereRotateSpeed = 50.0f;
        isNegative = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBossWeaponState == (int)CurrentBossWeaponState.DEFAULT)
        {
            this.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            onceDone = false;
            LR.enabled = false;
            return;
            
        }
        else if(currentBossWeaponState == (int)CurrentBossWeaponState.LASER)
        {
            ShootLaser();
            if (this.transform.localEulerAngles.y >= 110.0f && isNegative == false)
            {
                rotateDirection = -1;
            }
            if (this.transform.localEulerAngles.y <= 340.0f && isNegative == true)
            {
                rotateDirection = 1;
            }

            if (this.transform.localRotation.y <= 0.0f && isNegative == false)
            {
                Debug.Log("smaller than 0");
                isNegative = true;
            }
            if (this.transform.localRotation.y > 0)
            {
                Debug.Log("bigger than 0");
                isNegative = false;
            }

            RotateSphere();
        }
    }


    LineRenderer LR;
    float laserDistance;
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
    public void CircularSplitPattern()
    {
        Debug.Log("circular split pattern by boss weapon");
        if (onceDone == false)
        {
            Instantiate(circularSplitSphere, this.transform.position, Quaternion.identity);
            onceDone = true;
        }
        currentBossWeaponState = (int)CurrentBossWeaponState.DEFAULT;
    }


    public void LaserPattern()
    {
        Debug.Log("laser pattern by boss weapon");
        if (onceDone == false)
        {
            Instantiate(laserSphere,this.transform.position,Quaternion.identity);
            onceDone = true;
        }

    }

    [SerializeField]
    float sphereRotateSpeed;
    private void RotateSphere()
    {

        this.transform.Rotate(new Vector3(0f, sphereRotateSpeed * rotateDirection, 0f) * Time.deltaTime);


    }
}
