using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    public GameObject enemyBulletPrefab;
    public GameObject playerBulletPrefab;

    private Queue<EnemyBullet> bullet_eQueue = new Queue<EnemyBullet>();
    private Queue<PlayerBullet> bullet_pQueue = new Queue<PlayerBullet>();

    void Awake()
    {
        instance = this;
        Initialize(150);
    }

    private void Initialize(int fullCount)
    {
        for(int i=0; i<fullCount; i++)
        {
            bullet_eQueue.Enqueue(CreateObject("EnemyBullet").GetComponent<EnemyBullet>());
            bullet_pQueue.Enqueue(CreateObject("PlayerBullet").GetComponent<PlayerBullet>());
        }
    }

    private GameObject CreateObject(string objectType)
    {
        GameObject obj = null;

        switch (objectType)
        {
            case "EnemyBullet":
                obj = Instantiate(enemyBulletPrefab);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(instance.transform);
                break;
            case "PlayerBullet":
                obj = Instantiate(playerBulletPrefab);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(instance.transform);
                break;
        }

        return obj;
    }

    public static GameObject PushObject(string type)
    {
        switch(type)
        {
            case "EnemyBullet":
                if (instance.bullet_eQueue.Count > 0)
                {
                    var obj = instance.bullet_eQueue.Dequeue().gameObject;
                    obj.transform.SetParent(instance.transform);
                    obj.gameObject.SetActive(true);

                    return obj;
                }
                else
                {
                    var obj = instance.CreateObject("EnemyBullet");
                    obj.gameObject.SetActive(true);
                    obj.transform.SetParent(instance.transform);

                    return obj;
                }
            case "PlayerBullet":
                if (instance.bullet_pQueue.Count > 0)
                {
                    var obj = instance.bullet_pQueue.Dequeue().gameObject;
                    obj.transform.SetParent(instance.transform);
                    obj.gameObject.SetActive(true);

                    return obj;
                }
                else
                {
                    var obj = instance.CreateObject("PlayerBullet");
                    obj.gameObject.SetActive(true);
                    obj.transform.SetParent(instance.transform);

                    return obj;
                }
        }

        return null;
    }

    public static void PullObject(string type, GameObject obj)
    {
        switch(type)
        {
            case "EnemyBullet":
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(instance.transform);
                instance.bullet_eQueue.Enqueue(obj.GetComponent<EnemyBullet>());
                break;
            case "PlayerBullet":
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(instance.transform);
                instance.bullet_pQueue.Enqueue(obj.GetComponent<PlayerBullet>());
                break;
        }
    }
}
