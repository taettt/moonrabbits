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
    public float moveSpeed = 24.0f;

    public float teleportSpeed;
    [SerializeField]
    private bool teleported;
    private float teleportDelay = 0.4f;

    void Awake()
    {
        tr = this.transform;
        animator = tr.GetChild(0).GetComponent<Animator>();

        teleported = false;
    }

    void Start()
    {
        forwardVec = Camera.main.transform.forward;
        forwardVec.y = 0.0f;
        forwardVec = forwardVec.normalized;

        rightVec = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)) * forwardVec;
    }

    void Update()
    {
        Move();
        MoveAnim();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (teleported)
                return;

            teleported = true;
            tr.position += tr.forward * moveSpeed * 2;
        }

        if (teleported)
        {
            if (Time.time >= teleportDelay)
                teleported = false;
        }
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
