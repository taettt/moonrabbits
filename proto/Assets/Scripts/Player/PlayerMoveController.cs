using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveController : MonoBehaviour
{
    private Transform tr;
    public Transform playerModelTr;
    public LayerMask wallCollisionMask;
    private Animator animator;
    private Vector3 forwardVec, rightVec;
    private Vector3 m_movement;
    [SerializeField]
    private Vector3 m_dir;
    public float moveSpeed = 8.0f;

    private float teleportTimer;
    public float teleportSpeed = 10.0f;
    [SerializeField]
    private bool m_teleported;
    public bool teleported { get { return m_teleported; } }
    private float teleportDelay = 0.4f;

    public Text teleportText;

    public GameObject m_teleportFX;

    void Awake()
    {
        tr = this.transform;
        animator = tr.GetChild(0).GetComponent<Animator>();
    }

    void Start()
    {
        forwardVec = Camera.main.transform.forward;
        forwardVec.y = 0.0f;
        forwardVec = forwardVec.normalized;
        rightVec = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)) * forwardVec;

        teleportTimer = 0.0f;
        m_teleported = false;
    }

    void Update()
    {
        teleportText.text = "Dash Cool" + teleportTimer.ToString();

        if (m_teleported)
        {
            CheckWallAndTeleport();

            teleportTimer += Time.deltaTime;
            if (teleportTimer >= teleportDelay)
            {
                teleportTimer = 0.0f;
                m_teleported = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (teleported)
                return;

            m_teleported = true;
            PlayTeleportFX();
        }
    }

    void FixedUpdate()
    {
        Move();
        MoveAnim();
    }

    private void PlayTeleportFX()
    {
        GameObject go = Instantiate(m_teleportFX, this.transform.position, Quaternion.LookRotation(playerModelTr.forward));
        go.transform.SetParent(this.transform);
    }

    private void CheckWallAndTeleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(tr.position, playerModelTr.forward, out hit, teleportSpeed, wallCollisionMask))
        {
            tr.Translate(new Vector3(hit.point.x, 1.2f, hit.point.z) * Time.deltaTime);
        }
        else
        {
            tr.Translate(playerModelTr.forward * teleportSpeed * Time.deltaTime);
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
            if (teleported)
            {
                tr.position += m_movement * 1.2f;
            }
            else
            {
                tr.position += m_movement;
            }
        }
    }

    private void MoveAnim()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("IsRun", true);
        }
        else
        {
            animator.SetBool("IsRun", false);
        }
    }
}
