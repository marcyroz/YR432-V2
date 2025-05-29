using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSpawnerScript : MonoBehaviour
{
    [System.Serializable]
    public class CellSpawnData
    {
        public string poolTag;
        public CellData baseData;
    }

    [SerializeField] private List<CellSpawnData> cellTypes;

    private Dictionary<string, CellStats> modifiedStats = new();
    [SerializeField] private float minDistanceBetweenCells = 1.5f;

    private List<GameObject> activeCells = new List<GameObject>();
    private List<Coroutine> spawnCoroutines = new List<Coroutine>();
    private bool isSpawning = false;

    public delegate void StatChanged(string entityType, CellStats newStats);
    public event StatChanged OnStatsChanged;

    void Awake()
    {
        // Cria uma cópia modificável dos stats para cada tipo
        foreach (var cell in cellTypes)
        {
            modifiedStats[cell.baseData.entityType] = new CellStats(cell.baseData);
        }
    }

    public CellStats GetStatsFor(string entityType)
    {
        return modifiedStats[entityType];
    }

    public bool ModifyStat(string entityType, string statName, int delta)
    {
        if (!modifiedStats.ContainsKey(entityType)) return false;

        CellStats stats = modifiedStats[entityType];
        CellData baseData = cellTypes.Find(c => c.baseData.entityType == entityType).baseData;

        bool changed = false;

        switch (statName)
        {
            case "health":
                int newHealth = stats.health + delta;
                if (newHealth >= baseData.health)
                {
                    stats.health = newHealth;
                    changed = true;
                }
                break;

            case "resistance":
                int newRes = stats.resistance + delta;
                if (newRes >= baseData.resistance)
                {
                    stats.resistance = newRes;
                    changed = true;
                }
                break;

            case "reproductionRate":
                int newRepro = stats.reproductionRate + delta;
                if (newRepro >= baseData.reproductionRate)
                {
                    stats.reproductionRate = newRepro;
                    changed = true;
                }
                break;

            case "velocity":
                float newVel = stats.velocity + delta;
                if (newVel >= baseData.velocity)
                {
                    stats.velocity = newVel;
                    changed = true;
                }
                break;

            case "strength":
                if (baseData is WhiteBloodCellData wbc)
                {
                    int newStr = stats.strength + delta;
                    if (newStr >= wbc.strength)
                    {
                        stats.strength = newStr;
                        changed = true;
                    }
                }
                else if (baseData is VirusData virus)
                {
                    int newStr = stats.strength + delta;
                    if (newStr >= virus.strength)
                    {
                        stats.strength = newStr;
                        changed = true;
                    }
                }
                break;
        }

        if (changed)
            OnStatsChanged?.Invoke(entityType, stats);

        return changed;
    }

    public void StartSpawning()
    {
        StopSpawning();          // ← Primeiro para qualquer loop anterior
        isSpawning = true;       // ← Agora ativa o novo ciclo

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
        yield return new WaitForSeconds(1f);

        while (isSpawning) // ✅ checagem da flag
        {
            float interval = 20f / Mathf.Max(1, modifiedStats[cellData.baseData.entityType].reproductionRate);

            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject cell = ObjectPooler.Instance.SpawnFromPool(cellData.poolTag, spawnPos, Quaternion.identity);

            if (cell != null)
            {
                CellScript script = cell.GetComponent<CellScript>();
                if (script != null)
                {
                    CellStats stats = modifiedStats[cellData.baseData.entityType];
                    script.Initialize(cellData.baseData, stats);
                    Debug.Log($"Nova {cellData.baseData.entityType} criada com stats: ...");
                }

                activeCells.Add(cell);

                var agent = cell.GetComponent<AIAgent>();
                if (agent != null)
                {
                    agent.moveSpeed = modifiedStats[cellData.baseData.entityType].velocity;

                    if (cellData.baseData.entityType == "Virus")
                        agent.targetType = EntityType.RBC;
                    else if (cellData.baseData.entityType == "WBC")
                        agent.targetType = EntityType.Virus;
                    else if (cellData.baseData.entityType == "RBC")
                        agent.targetType = EntityType.Wander;
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
    }

}
