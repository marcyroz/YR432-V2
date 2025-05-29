using UnityEngine;

public class CellScript : MonoBehaviour
{
    public CellData cellData;
    public static CountBoardScript countBoard; // 🔗 Referência estática

    [HideInInspector] public int health;
    [HideInInspector] public int resistance;
    [HideInInspector] public int reproductionRate;
    [HideInInspector] public float velocity;

    public void Initialize(CellData data, CellStats stats)
    {
        cellData = data;
        health = stats.health;
        resistance = stats.resistance;
        reproductionRate = stats.reproductionRate;
        velocity = stats.velocity;
    }


    void OnEnable()
    {
        if (countBoard != null && cellData != null)
            countBoard.addEntity(cellData.entityType);

        if (GameStatsTracker.Instance != null && cellData != null)
            GameStatsTracker.Instance.RegisterCellBorn(cellData.entityType);
    }

    void OnDisable()
    {
        if (countBoard != null && cellData != null)
            countBoard.removeEntity(cellData.entityType);

        if (GameStatsTracker.Instance != null && cellData != null)
            GameStatsTracker.Instance.RegisterCellDeath(cellData.entityType);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            gameObject.SetActive(false); // Desativa (devolve à pool)
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }
}
