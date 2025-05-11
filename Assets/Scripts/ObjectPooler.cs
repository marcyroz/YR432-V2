using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size = 10;
    }

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool com tag " + tag + " não existe.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        int poolSize = pool.Count;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = pool.Dequeue();

            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                pool.Enqueue(obj);
                return obj;
            }

            // Mantém a ordem da fila
            pool.Enqueue(obj);
        }

        // Nenhum objeto disponível
        return null;
    }

}
