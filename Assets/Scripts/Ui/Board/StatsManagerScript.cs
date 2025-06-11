using UnityEngine;
using TMPro;

public class StatsManagerScript : MonoBehaviour
{
    [SerializeField] private CellSpawnerScript cellSpawner;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Text pointsText;

    [SerializeField] private int startingPoints = 10;
    private int remainingPoints;

    private readonly string[] entityTypes = { "RBC", "WBC", "Virus" };
    private string CurrentEntityType => entityTypes[dropdown.value];

    private void UpdatePointsUI()
    {
        if (pointsText != null)
            pointsText.text = $"Doses: {remainingPoints}";
    }

    public void ResetDoses()
    {
        remainingPoints = startingPoints;
        UpdatePointsUI();
    }

    public void AddDoses(int amount)
    {
        remainingPoints += amount;
        UpdatePointsUI();
        Debug.Log($"Doses atualizadas: +{amount} → Total: {remainingPoints}");
    }

    public void ModifyStat(string statName, int delta)
    {
        if (remainingPoints <= 0)
        {
            Debug.LogWarning("Todas as doses foram utilizadas.");
            return;
        }

        string entityType = CurrentEntityType;

        bool changed = cellSpawner.ModifyStat(entityType, statName, delta);

        if (!changed)
        {
            Debug.LogWarning($"Modificação inválida: {statName} de {entityType} não pode ser alterado com delta {delta}.");
            return;
        }

        remainingPoints--; // Só gasta se realmente modificou
        UpdatePointsUI();

        var updatedStats = cellSpawner.GetStatsFor(entityType);
        string fullStats = $"[Saúde: {updatedStats.health}, Resistência: {updatedStats.resistance}, " +
                           $"Reprodução: {updatedStats.reproductionRate}, Velocidade: {(int)updatedStats.velocity}, " +
                           $"Força: {updatedStats.strength}]";

        Debug.Log($"Modificado {statName} de {entityType} em {delta}. Novo estado: {fullStats}");
    }


    // Métodos para o Inspector
    public void AddHealth() => ModifyStat("health", +1);
    public void RemoveHealth() => ModifyStat("health", -1);

    public void AddVelocity() => ModifyStat("velocity", +1);
    public void RemoveVelocity() => ModifyStat("velocity", -1);

    public void AddResistance() => ModifyStat("resistance", +1);
    public void RemoveResistance() => ModifyStat("resistance", -1);

    public void AddReproduction() => ModifyStat("reproductionRate", +1);
    public void RemoveReproduction() => ModifyStat("reproductionRate", -1);

    public void AddStrength() => ModifyStat("strength", +1);
    public void RemoveStrength() => ModifyStat("strength", -1);
}
