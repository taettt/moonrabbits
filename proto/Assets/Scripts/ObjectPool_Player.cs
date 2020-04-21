using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPool_Player : MonoBehaviour
{
    public static ObjectPool_Player instance;

    public GameObject poolingPrefab;
    private Queue<PlayerBullet> poolingQueue = new Queue<PlayerBullet>();

    void Awake()
    {
        instance = this;
    }

    private void Initialize(int fullCount)
    {
        for(int i=0; i<fullCount; i++)
        {
            poolingQueue.Enqueue(CreateObject());
        }
    }

    private PlayerBullet CreateObject()
    {
        var obj = Instantiate(poolingPrefab).GetComponent<PlayerBullet>();
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.transform);

        return obj;
    }

    public static PlayerBullet PushObject()
    {
        if(instance.poolingQueue.Count>0)
        {
            var obj = instance.poolingQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);

            return obj;
        }
        else
        {
            var obj = instance.CreateObject();
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null);

            return obj;
        }
    }

    public static void PullObject(PlayerBullet obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.poolingQueue.Enqueue(obj);
    }

}
