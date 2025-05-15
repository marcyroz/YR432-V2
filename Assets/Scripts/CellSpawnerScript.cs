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

    public void ModifyStat(string entityType, string statName, int delta)
    {
        if (!modifiedStats.ContainsKey(entityType)) return;

        CellStats stats = modifiedStats[entityType];
        CellData baseData = cellTypes.Find(c => c.baseData.entityType == entityType).baseData;

        switch (statName)
        {
            case "health":
                stats.health = Mathf.Max(baseData.health, stats.health + delta);
                break;
            case "resistance":
                stats.resistance = Mathf.Max(baseData.resistance, stats.resistance + delta);
                break;
            case "reproductionRate":
                stats.reproductionRate = Mathf.Max(baseData.reproductionRate, stats.reproductionRate + delta);
                break;
            case "velocity":
                stats.velocity = Mathf.Max(baseData.velocity, stats.velocity + delta);
                break;

            case "strength":
                if (baseData is WhiteBloodCellData wbc)
                    stats.strength = Mathf.Max(wbc.strength, stats.strength + delta);
                else if (baseData is VirusData virus)
                    stats.strength = Mathf.Max(virus.strength, stats.strength + delta);
                break;

            case "infected":
                if (baseData is RedBloodCellData)
                    stats.infected = !stats.infected; // Alterna true/false
                break;
        }

        OnStatsChanged?.Invoke(entityType, stats);
    }


    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;

        StopSpawning(); // safety
        foreach (var cell in cellTypes)
        {
            Coroutine routine = StartCoroutine(SpawnLoop(cell));
            spawnCoroutines.Add(routine);
        }
    }

    public void StopSpawning()
    {
        if (!isSpawning) return;
        isSpawning = false;

        foreach (var routine in spawnCoroutines)
        {
            if (routine != null)
                StopCoroutine(routine);
        }
        spawnCoroutines.Clear();
    }


    private IEnumerator SpawnLoop(CellSpawnData cellData)
    {
        while (true)
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
            float camHeight = 2f * Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            float x = Random.Range(-camWidth / 2f, camWidth / 2f);
            float y = Random.Range(-camHeight / 2f, camHeight / 2f);

            candidate = new Vector3(x, y, 0);
            attempts++;

            if (attempts > 20) break;
        }
        while (!IsFarEnough(candidate));

        return candidate;
    }

    private bool IsFarEnough(Vector3 pos)
    {
        foreach (var cell in activeCells)
        {
            if (cell == null) continue;
            if (Vector3.Distance(pos, cell.transform.position) < minDistanceBetweenCells)
                return false;
        }
        return true;
    }

    private void CleanupList()
    {
        activeCells.RemoveAll(c => c == null || !c.activeInHierarchy);
    }

    public void ResetSpawning()
    {
        StopSpawning();          // Para todas as corrotinas
        ClearActiveCells();      // Desativa e limpa a lista de células
        StartSpawning();         // Recomeça o spawn
    }

    private void ClearActiveCells()
    {
        foreach (var cell in activeCells)
        {
            if (cell != null)
            {
                cell.SetActive(false); // Devolvendo pra pool
            }
        }

        activeCells.Clear();
    }

}
