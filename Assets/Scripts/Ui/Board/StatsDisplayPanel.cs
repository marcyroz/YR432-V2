using UnityEngine;
using TMPro;

public class StatsDisplayPanel : MonoBehaviour
{
    [SerializeField] private string entityType; // "RBC", "WBC", "Virus"
    [SerializeField] private CellSpawnerScript cellSpawner;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text resistanceText;
    [SerializeField] private TMP_Text reproductionText;
    [SerializeField] private TMP_Text velocityText;
    [SerializeField] private TMP_Text strengthText;  // Só usado por WBC/Virus

    void Start()
    {
        if (cellSpawner != null)
        {
            cellSpawner.OnStatsChanged += UpdateStats;
            UpdateUI(cellSpawner.GetStatsFor(entityType)); // Mostra stats iniciais
        }
    }

    void OnDestroy()
    {
        if (cellSpawner != null)
            cellSpawner.OnStatsChanged -= UpdateStats;
    }

    private void UpdateStats(string changedEntity, CellStats newStats)
    {
        if (changedEntity != entityType) return;
        UpdateUI(newStats);
    }

    private void UpdateUI(CellStats stats)
    {
        // Comuns a todos
        healthText.text = $"Saude: {stats.health}";
        resistanceText.text = $"Resistencia: {stats.resistance}";
        reproductionText.text = $"Reproducao: {stats.reproductionRate}";
        velocityText.text = $"Velocidade: {(int)stats.velocity}";


        // Mostrar força apenas para entidades que usam
        bool hasStrength = entityType == "WBC" || entityType == "Virus";
        if (strengthText != null)
        {
            strengthText.gameObject.SetActive(hasStrength);
            if (hasStrength)
                strengthText.text = $"Forca: {stats.strength}";
        }
    }
}
