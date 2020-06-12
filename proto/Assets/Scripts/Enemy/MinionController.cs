using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionController : MonoBehaviour
{
    public GameObject seedPrefab;
    public Transform seedParent;

    public Transform playerTr;
    private Vector3 m_dir;

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

    void Start()
    {
        Init();
        originY = this.transform.position.y;

        StartCoroutine(ShootBulletCoroutine());
    }
    
    Vector3 toPlayer;
    [SerializeField]
    float originY;
    void Update()
    {
        toPlayer = playerTr.position;
        toPlayer.y = originY;
        this.transform.position = Vector3.MoveTowards(this.transform.position, toPlayer, m_moveSpeed * Time.deltaTime);
    }

    void OnTriggernEnter(Collider coll)
    {
        if (coll.tag == "PLAYERBULLET")
        {
            DecreaseHP(coll.GetComponent<PlayerBullet>().attack);
        }
        else if (coll.tag == "PLAYER")
        {
            Instantiate(m_minionFXPrefabs[1], this.transform.position, Quaternion.identity);
            coll.GetComponent<PlayerController>().DecreaseHP(m_attackStat);
            Destroy();
        }
    }

    private void Init()
    {
        m_hp = 4;
        m_moveSpeed = 4.0f;
        m_attackDelay = 0.5f;
        m_attackSpeed = 4.0f;
        m_attackStat = 2;

        Instantiate(m_minionFXPrefabs[0], this.transform);
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
        DestroyImmediate(this.gameObject);
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
