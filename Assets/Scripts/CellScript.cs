using UnityEngine;

public class CellScript : MonoBehaviour
{
    public CellData cellData;
    public static CountBoardScript countBoard;

    [HideInInspector] public int health;
    [HideInInspector] public int resistance;
    [HideInInspector] public int reproductionRate;
    [HideInInspector] public float velocity;

    // Flag para saber se esta instância de RBC foi infectada
    [HideInInspector] public bool isInfectedInstance = false;

    public void Initialize(CellData data, CellStats stats)
    {
        cellData = data;
        health = stats.health;
        resistance = stats.resistance;
        reproductionRate = stats.reproductionRate;
        velocity = stats.velocity;

        // Toda vez que a pool (re)ativa este objeto, ele começa NÃO infectado
        isInfectedInstance = false;
    }

    void OnEnable()
    {
        // Sempre adiciona o próprio cellData.entityType (ex.: "RBC", "Virus" ou "WBC")
        if (countBoard != null && cellData != null)
            countBoard.addEntity(cellData.entityType);

        if (GameStatsTracker.Instance != null && cellData != null)
            GameStatsTracker.Instance.RegisterCellBorn(cellData.entityType);
    }

    void OnDisable()
    {
        if (countBoard != null && cellData != null)
        {
            // Se for um RBC que já foi marcado como infectado nesta instância,
            // removemos "IRBC". Caso contrário, removemos somente o próprio tipo original.
            if (cellData.entityType == "RBC" && isInfectedInstance)
            {
                countBoard.removeEntity("IRBC");
            }
            else
            {
                countBoard.removeEntity(cellData.entityType);
            }
        }

        if (GameStatsTracker.Instance != null && cellData != null)
            GameStatsTracker.Instance.RegisterCellDeath(cellData.entityType);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Quando a vida chega a zero, simplesmente desativa o objeto
            // → Isso vai chamar OnDisable() e removerá “IRBC” ou “RBC” conforme o caso.
            gameObject.SetActive(false);
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }
}
