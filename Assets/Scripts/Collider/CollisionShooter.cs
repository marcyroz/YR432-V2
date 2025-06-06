using UnityEngine;
using System;

public class CollisionShooter : MonoBehaviour
{
    public EntityType entityType;

    // Eventos para entrada e saída de trigger
    public static event Action<EntityType, EntityType, GameObject, GameObject> OnEntitiesCollidedEnter;
    public static event Action<EntityType, EntityType, GameObject, GameObject> OnEntitiesCollidedExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherShooter = other.GetComponent<CollisionShooter>();
        if (otherShooter == null) return;
        OnEntitiesCollidedEnter?.Invoke(
            entityType, otherShooter.entityType,
            this.gameObject, otherShooter.gameObject
        );
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherShooter = other.GetComponent<CollisionShooter>();
        if (otherShooter == null) return;
        OnEntitiesCollidedExit?.Invoke(
            entityType, otherShooter.entityType,
            this.gameObject, otherShooter.gameObject
        );
    }
}
