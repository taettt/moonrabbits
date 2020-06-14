using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public GameObject seedPrefab;
    public Transform seedParent;

    public Transform playerTr;
    private Vector3 m_dir;

    [SerializeField]
    private int m_hp;
    private float m_moveSpeed;
    private float m_attackSpeed;
    private float m_attackDelay;
    private int m_attackStat;

    public GameObject[] m_minionFXPrefabs;

    void Awake()
    {
        seedParent = GameObject.Find("Traps").transform;
        playerTr = GameObject.FindWithTag("PLAYER").transform;
    }
    [SerializeField]
    float originY;
    void Start()
    {
        originY = this.transform.position.y;
        Init();

        StopAllCoroutines();
        StartCoroutine(ShootBulletCoroutine());
    }
    [SerializeField]
    Vector3 toPlayer;
    
    void Update()
    {
        if (col != null) //might use better collision check
        {
            toPlayer = playerTr.position - (col.transform.position - this.transform.position);
            toPlayer.y = originY;
        }
        else
        {
            toPlayer = playerTr.position;
            toPlayer.y = originY;
        }

            
        
        this.transform.position = Vector3.MoveTowards(this.transform.position, toPlayer, m_moveSpeed * Time.deltaTime);
    }
    Collider col;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "PLAYERBULLET")
        {
            Debug.Log("Minion Collided With PlayerBullet");
            DecreaseHP(coll.GetComponent<PlayerBullet>().attack);
        }
        else if (coll.tag == "PLAYER")
        {
            Debug.Log("Minion Collided With Player");
            //Instantiate(m_minionFXPrefabs[1], this.transform.position, Quaternion.identity);
            coll.GetComponent<PlayerController>().DecreaseHP(m_attackStat);
            Destroy();
        }
        else if(coll.tag=="MINION")
        {
            Debug.Log("colliding w/ other minion");
            col = coll;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "MINION")
        {
            col = null;
        }
    }


    private void Init()
    {
        m_hp = 4;
        m_moveSpeed = 4.0f;
        m_attackDelay = 0.5f;
        m_attackSpeed = 4.0f;
        m_attackStat = 2;

        //Instantiate(m_minionFXPrefabs[0], this.transform);
    }

    public void DropSeed()
    {
        StopCoroutine(ShootBulletCoroutine());

        GameObject go = Instantiate(seedPrefab, this.transform.position, Quaternion.identity);
        go.transform.SetParent(seedParent);
    }

    public void Destroy()
    {
        StopCoroutine(ShootBulletCoroutine());
        Destroy(this.gameObject);
    }

    private IEnumerator ShootBulletCoroutine()
    {
        while (true)
        {
            m_dir = (playerTr.position-this.transform.position).normalized;
            m_dir.y = 0;
            ShootBullet();

            yield return new WaitForSeconds(m_attackDelay);
        }
    }

    private void DecreaseHP(int attackVal)
    {
        if (m_hp - attackVal <= 0)
        {
            Destroy(this.gameObject);
            DropSeed();
        }

        m_hp -= attackVal;
    }

    private void ShootBullet()
    {
        var bullet = ObjectManager.PushObject("EnemyBullet").GetComponent<EnemyBullet>();
        bullet.transform.position = this.transform.position;
        bullet.Spawn(this.transform.position, m_dir, m_attackSpeed, m_attackStat);
    }
}
