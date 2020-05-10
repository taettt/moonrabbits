using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private Transform tr;
    public Transform playerModelTr;
    private Animator animator;
    private Vector3 forwardVec, rightVec;
    private Vector3 m_movement;
    [SerializeField]
    private Vector3 m_dir;
    public float moveSpeed = 6.0f;

    private float teleportTimer;
    public float teleportSpeed;
    [SerializeField]
    private bool teleported;
    private float teleportDelay = 0.4f;

    public GameObject m_teleportTrail;
    public GameObject m_teleportFX;

    void Awake()
    {
        tr = this.transform;
        m_teleportTrail = tr.GetChild(2).gameObject;
        animator = tr.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        forwardVec = Camera.main.transform.forward;
        forwardVec.y = 0.0f;
        forwardVec = forwardVec.normalized;
        rightVec = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)) * forwardVec;

        teleportTimer = 0.0f;
        teleported = false;
    }

    void FixedUpdate()
    {
        Move();
        MoveAnim();
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (teleported)
                return;

            teleported = true;
            PlayTeleportFX();
            CheckWallAndTeleport();
        }

        if (teleported)
        {
            teleportTimer += Time.deltaTime;
            if (teleportTimer >= teleportDelay)
            {
                teleportTimer = 0.0f;
                teleported = false;
            }
        }
    }

    private void PlayTeleportFX()
    {
        m_teleportTrail.SetActive(true);
        Instantiate(m_teleportFX, this.transform.position, Quaternion.LookRotation(playerModelTr.forward));
    }

    private void CheckWallAndTeleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, playerModelTr.forward, out hit, moveSpeed * 2, LayerMask.NameToLayer("WallLayer")))
        {
            tr.position = new Vector3(hit.point.x, 1.2f, hit.point.z);
        }
        else
        {
            tr.position += playerModelTr.forward * moveSpeed * 2;
        }

        m_teleportTrail.SetActive(false);
    }

    private void Move()
    {
        if (!Input.anyKey)
            return;

        Vector3 fMovement = forwardVec * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 rMovement = rightVec * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        m_movement = fMovement + rMovement;
        m_dir = Vector3.Normalize(m_movement);

        if (m_dir != Vector3.zero)
        {
            playerModelTr.forward = m_dir;
            tr.position += m_movement;
        }
    }

    private void MoveAnim()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }
}
