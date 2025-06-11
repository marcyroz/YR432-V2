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

    [HideInInspector] public bool isInfectedInstance = false;

    private Coroutine lifetimeCoroutine;
    private bool wasKilled = false;

    public void Initialize(CellData data, CellStats stats)
    {
        cellData = data;
        health = stats.health;
        resistance = stats.resistance;
        reproductionRate = stats.reproductionRate;
        velocity = stats.velocity;

        isInfectedInstance = false;
        wasKilled = false; // Reset ao inicializar

        if (cellData.entityType == "WBC")
        {
            if (lifetimeCoroutine != null)
                StopCoroutine(lifetimeCoroutine);

            float lifeTime = health + resistance;
            lifetimeCoroutine = StartCoroutine(LifeCycleTimer(lifeTime));
        }
    }

    private IEnumerator LifeCycleTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (gameObject.activeInHierarchy)
        {
            wasKilled = true; // Considera como morte natural
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (countBoard != null && cellData != null)
            countBoard.addEntity(cellData.entityType);
    }

    void OnDisable()
    {
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }

        if (countBoard != null && cellData != null)
        {
            if (cellData.entityType == "RBC" && isInfectedInstance)
            {
                countBoard.removeEntity("IRBC");
            }
            else
            {
                countBoard.removeEntity(cellData.entityType);

                if (cellData.entityType == "Virus")
                {
                    GameStatsTracker.Instance?.RegisterCellDeath("Virus");
                }
                else if (cellData.entityType == "WBC" && wasKilled)
                {
                    GameStatsTracker.Instance?.RegisterCellDeath("WBC");
                }
            }
        }

        wasKilled = false; // Reset para uso futuro
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            wasKilled = true;
            gameObject.SetActive(false);
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }
}
