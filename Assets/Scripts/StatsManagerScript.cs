using UnityEngine;
using TMPro;

public class StatsManagerScript : MonoBehaviour
{
    [SerializeField] private CellSpawnerScript cellSpawner;
    [SerializeField] private TMP_Dropdown dropdown;

    private readonly string[] entityTypes = { "RBC", "WBC", "Virus" };
    private string CurrentEntityType => entityTypes[dropdown.value];

    public void ModifyStat(string statName, int delta)
    {
        string entityType = CurrentEntityType;
        cellSpawner.ModifyStat(entityType, statName, delta);
        var updatedStats = cellSpawner.GetStatsFor(entityType);
        string fullStats = $"[Saúde: {updatedStats.health}, Resistência: {updatedStats.resistance}, " +
                           $"Reprodução: {updatedStats.reproductionRate}, Velocidade: {(int)updatedStats.velocity}, " +
                           $"Força: {updatedStats.strength}]";

        Debug.Log($"Mestre Marcelly modificou {statName} de {entityType} em {delta}. Novo estado: {fullStats}");
    }

    // Métodos sem parâmetros para o Inspector
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
