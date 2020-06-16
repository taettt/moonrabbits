﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaserSphere : EnemyBullet
{
    bool shouldMove;
    Vector3 startPosition;

    // Start is called before the first frame update
    void Awake()
    {
        LR = GetComponent<LineRenderer>();
        LR.enabled = false;
    }
    float originY;
    void Start()
    {
        shouldMove = true;
        originY = this.transform.position.y;
        timer = 0;
        rotateDirection = 1;
        moveSpeed = 5;
        laserDistance = 30.0f;
        timeToReach = 2.0f;
        sphereRotateSpeed = 50.0f;
        startPosition = this.transform.position;
        isNegative = false;

    }
    float timer;
    // Update is called once per frame
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    bool isNegative;

    [SerializeField]
    float timeToReach;

    float moveTimer;
    void Update()
    {
        Vector3 oppositePosi = FindObjectOfType<Boss_1Control>().transform.position;
        oppositePosi.x *= -1;
        oppositePosi.y = originY;
        oppositePosi.z *= -1;

        
        if ((int)this.transform.position.x == oppositePosi.x && (int)this.transform.position.z == oppositePosi.z){
            timer += Time.deltaTime;
            shouldMove = false;
            readyToShoot = true;
            FindObjectOfType<BossWeapon>().currentBossWeaponState = (int)CurrentBossWeaponState.LASER;
            ShootLaser();


            if (this.transform.localEulerAngles.y >= 110.0f && isNegative==false)
            {
                rotateDirection = -1;
            }
            if (this.transform.localEulerAngles.y <= 340.0f && isNegative == true)
            {
                rotateDirection = 1;
            }

            if (this.transform.localRotation.y <= 0.0f && isNegative==false)
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

            if (timer >= 10.0f)
            {
                FindObjectOfType<BossWeapon>().currentBossWeaponState = (int)CurrentBossWeaponState.DEFAULT;
                FindObjectOfType<Boss_1Control>().ForceExcutePhase();
                Destroy(this.gameObject);
            }
        }


        if (shouldMove == true)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, oppositePosi, moveSpeed * Time.deltaTime);

            moveTimer += Time.deltaTime / timeToReach;
            transform.position = Vector3.Lerp(startPosition, oppositePosi, moveTimer);
        }
    }
    

    public bool readyToShoot;
    private void OnTriggerEnter(Collider coll)
    {
        Debug.Log("collided"+coll.tag);
        switch (coll.tag)
        {
            case "PLAYER":
                if (coll.GetComponent<PlayerStateController>().curState == PlayerState.ATTACKED ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.NOCK ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.INVI ||
                    coll.GetComponent<PlayerStateController>().curState == PlayerState.RETIRE)
                {
                    return;
                }

                PlayerController pc = coll.gameObject.GetComponent<PlayerController>();
                pc.DecreaseHP(5);
                break;
        }
    }

    LineRenderer LR;
    float laserDistance;
    [SerializeField]
    LayerMask layer;
    [SerializeField]
    Vector3 laserPoint;
    private void ShootLaser()
    {
        //from sphere
        LR.enabled = true;
        LR.SetPosition(0, this.transform.position);
        Vector3 endPosi = this.transform.position;
        
        RaycastHit hit;
        Debug.DrawRay(this.transform.position, this.transform.forward*laserDistance, Color.red);
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, laserDistance, layer))
        {
            Debug.Log("laser hit");
            FindObjectOfType<PlayerController>().DecreaseHP(5);
            return;

        }
        Vector3 laserPoint = this.transform.position + this.transform.forward * laserDistance;
        LR.SetPosition(1, laserPoint);


        //from boss
        
    }
    [SerializeField]
    int rotateDirection;
    [SerializeField]
    float sphereRotateSpeed;
    private void RotateSphere()
    {
        Debug.Log(this.transform.localEulerAngles);
        this.transform.Rotate(new Vector3(0f, sphereRotateSpeed*rotateDirection, 0f) * Time.deltaTime);


    }

}
