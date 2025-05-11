using UnityEngine;

public class CellScript : MonoBehaviour
{
    public CellData cellData;

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

    // Não precisamos mais do Start
    // protected virtual void Start() { ... }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        health += amount;
    }
}
