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

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, bool allowExpand = false)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool com tag “" + tag + "” não existe.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        int poolSize = pool.Count;

        // 1) Verifica se existe algum inativo na fila
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

            // se estava ativo, re-enfileira e continua iterando
            pool.Enqueue(obj);
        }

        // 2) Se não encontrou nenhum inativo: decide se expande ou não
        if (!allowExpand)
        {
            // NÃO permitir expansão: retorna null
            return null;
        }

        // Permitir expansão: instanciar um clone “extra”
        GameObject prefab = pools.Find(p => p.tag == tag)?.prefab;
        if (prefab != null)
        {
            GameObject newObj = Instantiate(prefab, position, rotation);
            newObj.name = prefab.name; // opcional, para evitar “(Clone)” no nome
            newObj.SetActive(true);
            pool.Enqueue(newObj);
            return newObj;
        }

        Debug.LogWarning("Pool com tag “" + tag + "” não tem prefab para instanciar extras.");
        return null;
    }
}
