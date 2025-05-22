using UnityEngine;
using System;

public class CollisionShooter : MonoBehaviour
{
    [Header("Identificação desta entidade")]
    public EntityType entityType;

    // Evento que dispara sempre que há colisão
    public static event Action<EntityType, EntityType> OnEntitiesCollided;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherShooter = other.GetComponent<CollisionShooter>();
        if (otherShooter == null) return;

        // Dispara o evento com (quem colidiu, com quem)
        OnEntitiesCollided?.Invoke(entityType, otherShooter.entityType);

        // Console temporário para verificar
        Debug.Log($"{entityType} colidiu com {otherShooter.entityType}");
    }
}
