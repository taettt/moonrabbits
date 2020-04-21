using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Enemy : MonoBehaviour
{
    public static ObjectPool_Enemy instance;

    public GameObject enemyBulletPrefab;
    private Queue<EnemyBullet> bullet_eQueue = new Queue<EnemyBullet>();

    void Awake()
    {
        instance = this;
        Initialize(150);
    }

    private void Initialize(int fullCount)
    {
        for(int i=0; i<fullCount; i++)
        {
            bullet_eQueue.Enqueue(CreateObject_e());
            //bullet_pQueue.Enqueue(CreateObject_p());
        }
    }

    private EnemyBullet CreateObject_e()
    {
        var obj = Instantiate(enemyBulletPrefab).GetComponent<EnemyBullet>();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);

        return obj;
    }

    /*
    private PlayerBullet CreateObject_p()
    {
        var obj = Instantiate(playerBulletPrefab).GetComponent<PlayerBullet>();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);

        return obj;
    }
    */

    public static EnemyBullet PushObject_e()
    {
        if(instance.bullet_eQueue.Count>0)
        {
            var obj = instance.bullet_eQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);

            return obj;
        }
        else
        {
            var obj = instance.CreateObject_e();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null);

            return obj;
        }
    }

    public static void PullObject(EnemyBullet obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.bullet_eQueue.Enqueue(obj);
    }
}
