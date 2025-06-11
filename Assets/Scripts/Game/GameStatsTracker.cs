using UnityEngine;

public class GameStatsTracker : MonoBehaviour
{
    public static GameStatsTracker Instance;

    public int virusKilled = 0;
    public int wbcKilled = 0;
    public int rbcTransformed = 0;
    public int rbcInfected = 0;
    public int rbcCured = 0;
    public int totalCellsBorn = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameStatsTracker inicializado com sucesso.");
        }
        else
        {
            Debug.LogWarning("GameStatsTracker duplicado detectado e destruído.");
            Destroy(gameObject);
        }
    }

    public void RegisterCellBorn(string entityType)
    {
        totalCellsBorn++;
    }

    public void RegisterCellDeath(string entityType)
    {
        if (entityType == "Virus") virusKilled++;
        else if (entityType == "WBC") wbcKilled++;
    }

    public void RegisterInfection() => rbcInfected++;
    public void RegisterCure() => rbcCured++;
    public void RegisterTransformation() => rbcTransformed++;

    public void ResetStats()
    {
        virusKilled = 0;
        wbcKilled = 0;
        rbcTransformed = 0;
        rbcInfected = 0;
        rbcCured = 0;
        totalCellsBorn = 0;
    }
}
