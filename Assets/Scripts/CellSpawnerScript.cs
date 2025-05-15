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
        // float interval = 20f / Mathf.Max(1, cellData.baseData.reproductionRate);
        yield return new WaitForSeconds(1f);

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

                    // DEBUG: Mostrar as propriedades completas
                    Debug.Log(
                        $"Nova {cellData.baseData.entityType} criada com stats: " +
                        $"Saude={stats.health}, " +
                        $"Resistencia={stats.resistance}, " +
                        $"Reproducao={stats.reproductionRate}, " +
                        $"Velocidade={stats.velocity}, " +
                        $"Forca={stats.strength}, " +
                        $"Infectada={stats.infected}");
                }
                // 1) Inicializa CellScript
                // cell.GetComponent<CellScript>()?.Initialize(cellData.baseData);
                activeCells.Add(cell);

                // 2) Se tiver AIAgent, configure targetType e moveSpeed
                var agent = cell.GetComponent<AIAgent>();
                if (agent != null)
                {
                    // velocity vem do CellData
                    agent.moveSpeed = modifiedStats[cellData.baseData.entityType].velocity;

                    // targetType (enum) define quem perseguir
                    // mapeando a string data.entityType para o enum
                    if (cellData.baseData.entityType == "Virus")
                        agent.targetType = EntityType.RBC; // Vírus persegue RBC
                    else if (cellData.baseData.entityType == "WBC")
                        agent.targetType = EntityType.Virus; // WBC persegue Vírus
                    else if (cellData.baseData.entityType == "RBC")
                        agent.targetType = EntityType.Wander;  // RBC vagueia
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
