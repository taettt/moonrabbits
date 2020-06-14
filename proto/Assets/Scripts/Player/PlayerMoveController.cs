using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveController : MonoBehaviour
{
    private Transform tr;
    public Transform playerModelTr;
    public PlayerStateController psc;
    public LayerMask wallCollisionMask;
    private Animator animator;
    private Vector3 forwardVec, rightVec;
    private Vector3 m_movement;
    private Vector3 m_dir;

    public float moveSpeed = 8.0f;
    public float teleportSpeed = 10.0f;

    private float teleportTimer;
    [SerializeField]
    private bool m_teleported;
    public bool teleported { get { return m_teleported; } }
    private float teleportDelay = 0.4f;

    public UrgentManager um;

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
        if (psc.curState == PlayerState.RETIRE || psc.curState == PlayerState.NOCK || psc.curState == PlayerState.ATTACKED)
        {
            return;
        }

        Move();
        MoveAnim();

        //teleportText.text = "Dash Cool" + teleportTimer.ToString();

        /*
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (teleported)
                return;

            m_teleported = true;
            PlayTeleportFX();

            if (um.urgentRangeIn)
            {
                um.BonusOn();
            }
        }
        */

        if(!m_teleported)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                m_teleported = true;
                PlayTeleportFX();
                StartCoroutine(Teleport());

                if(um.urgentRangeIn)
                {
                    um.BonusOn();
                }
            }
        }
    }

    private void PlayTeleportFX()
    {
        GameObject go = Instantiate(m_teleportFX, this.transform.position, Quaternion.LookRotation(playerModelTr.forward));
        go.transform.SetParent(this.transform);
        Destroy(go, 1.0f);
    }

    private IEnumerator Teleport()
    {
        RaycastHit hit;
        if (Physics.Raycast(tr.position, playerModelTr.forward, out hit, teleportSpeed))
        {
            tr.Translate(new Vector3(hit.point.x, 0.0f, hit.point.z) * Time.deltaTime * teleportSpeed);
        }
        else
        {
            tr.Translate(playerModelTr.forward * teleportSpeed * Time.deltaTime);
        }

        yield return new WaitForSeconds(teleportDelay);
        m_teleported = false;
    }

    // 고려 -> spherecast(겹친 collider 해결), cast하는 길이(지름) 조정
    private void Move()
    {
        if (!Input.anyKey)
            return;

        Vector3 fMovement = forwardVec * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 rMovement = rightVec * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        m_movement = fMovement + rMovement;
        m_dir = Vector3.Normalize(m_movement);
        playerModelTr.forward = m_dir;

        if (m_dir != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(tr.position, playerModelTr.forward, out hit, 0.3f))
            {
                tr.position = new Vector3(hit.point.x, 0.2f, hit.point.z);
            }
            else
            {
                tr.position += m_movement;
            }
        }
    }

    private void MoveAnim()
    {
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            animator.SetBool("IsRun", true);
        }
        else
        {
            animator.SetBool("IsRun", false);
        }

        animator.SetFloat("DirX", Input.GetAxis("Horizontal"));
        animator.SetFloat("DirY", Input.GetAxis("Vertical"));
    }
}
