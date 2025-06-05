using System.Collections;
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

    private Coroutine lifetimeCoroutine;

    public void Initialize(CellData data, CellStats stats)
    {
        cellData = data;
        health = stats.health;
        resistance = stats.resistance;
        reproductionRate = stats.reproductionRate;
        velocity = stats.velocity;

        isInfectedInstance = false;

        // Se for um WBC, começa o ciclo de vida baseado em saúde + resistência
        if (cellData.entityType == "WBC")
        {
            if (lifetimeCoroutine != null)
                StopCoroutine(lifetimeCoroutine);

            float lifeTime = health + resistance; // Tempo em segundos
            lifetimeCoroutine = StartCoroutine(LifeCycleTimer(lifeTime));
        }
    }

    private IEnumerator LifeCycleTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Mata a célula se ainda estiver ativa
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
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
        // Para o ciclo de vida se estiver em execução
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }

        // Lógica de contagem
        if (countBoard != null && cellData != null)
        {
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
