using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSpawnerScript : MonoBehaviour
{
    [System.Serializable]
    public class CellSpawnData
    {
        public string poolTag;
        public CellData data;
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
        float interval = 20f / Mathf.Max(1, cellData.data.reproductionRate);

        yield return new WaitForSeconds(1f);

        while (true)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject cell = ObjectPooler.Instance.SpawnFromPool(cellData.poolTag, spawnPos, Quaternion.identity);

            if (cell != null)
            {
                CellScript script = cell.GetComponent<CellScript>();
                if (script != null)
                    script.Initialize(cellData.data);

                activeCells.Add(cell);
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
