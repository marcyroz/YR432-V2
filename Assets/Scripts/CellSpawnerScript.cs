using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSpawnerScript : MonoBehaviour
{
    [System.Serializable]
    public class CellSpawnData
    {
        public string poolTag;
        public CellData data;   // mantém seu CellData com string entityType e float velocity
    }

    [SerializeField] private List<CellSpawnData> cellTypes;
    [SerializeField] private float minDistanceBetweenCells = 1.5f;

    private List<GameObject> activeCells = new List<GameObject>();
    private List<Coroutine> spawnCoroutines = new List<Coroutine>();
    private bool isSpawning = false;

    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;
        StopSpawning();

        foreach (var cellData in cellTypes)
            spawnCoroutines.Add(StartCoroutine(SpawnLoop(cellData)));
    }

    public void StopSpawning()
    {
        if (!isSpawning) return;
        isSpawning = false;

        foreach (var routine in spawnCoroutines)
            if (routine != null)
                StopCoroutine(routine);

        spawnCoroutines.Clear();
    }

    private IEnumerator SpawnLoop(CellSpawnData cellData)
    {
        float interval = 20f / Mathf.Max(1, cellData.data.reproductionRate);
        yield return new WaitForSeconds(1f);

        while (true)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject cell = ObjectPooler.Instance.SpawnFromPool(cellData.poolTag, spawnPos, Quaternion.identity);

            if (cell != null)
            {
                // 1) Inicializa CellScript
                cell.GetComponent<CellScript>()?.Initialize(cellData.data);
                activeCells.Add(cell);

                // 2) Se tiver AIAgent, configure targetType e moveSpeed
                var agent = cell.GetComponent<AIAgent>();
                if (agent != null)
                {
                    // velocity vem do CellData
                    agent.moveSpeed = cellData.data.velocity;

                    // targetType (enum) define quem perseguir
                    // mapeando a string data.entityType para o enum
                    if (cellData.data.entityType == "Virus")
                        agent.targetType = EntityType.RBC;
                    else if (cellData.data.entityType == "WBC")
                        agent.targetType = EntityType.Virus;
                    else if (cellData.data.entityType == "RBC")
                        agent.targetType = EntityType.IRBC; // por exemplo, ou outro caso
                    // adicione outros mapeamentos conforme necessário
                }
            }

            CleanupList();
            yield return new WaitForSeconds(interval);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 candidate;
        int attempts = 0;

        do
        {
            float camH = 2f * Camera.main.orthographicSize;
            float camW = camH * Camera.main.aspect;
            candidate = new Vector3(
                Random.Range(-camW / 2f, camW / 2f),
                Random.Range(-camH / 2f, camH / 2f),
                0);
            attempts++;
            if (attempts > 20) break;
        }
        while (!IsFarEnough(candidate));

        return candidate;
    }

    private bool IsFarEnough(Vector3 pos)
    {
        foreach (var cell in activeCells)
            if (cell != null && Vector3.Distance(pos, cell.transform.position) < minDistanceBetweenCells)
                return false;
        return true;
    }

    private void CleanupList()
    {
        activeCells.RemoveAll(c => c == null || !c.activeInHierarchy);
    }

    public void ResetSpawning()
    {
        StopSpawning();
        foreach (var cell in activeCells)
            if (cell != null) cell.SetActive(false);
        activeCells.Clear();
        StartSpawning();
    }
}
