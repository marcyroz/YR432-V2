using UnityEngine;

public class CellScript : MonoBehaviour
{
    public CellData cellData;
    public static CountBoardScript countBoard; // 🔗 Referência estática

    [HideInInspector] public int health;
    [HideInInspector] public int resistance;
    [HideInInspector] public int reproductionRate;
    [HideInInspector] public float velocity;

    public void Initialize(CellData data)
    {
        cellData = data;
        health = data.health;
        resistance = data.resistance;
        reproductionRate = data.reproductionRate;
        velocity = data.velocity;
    }

    void OnEnable()
    {
        if (countBoard != null && cellData != null)
            countBoard.addEntity(cellData.entityType);
    }

    void OnDisable()
    {
        if (countBoard != null && cellData != null)
            countBoard.removeEntity(cellData.entityType);
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
