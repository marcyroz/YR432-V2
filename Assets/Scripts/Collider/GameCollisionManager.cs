using UnityEngine;

public class GameCollisionManager : MonoBehaviour
{
    private void OnEnable()
    {
        CollisionShooter.OnEntitiesCollided += HandleCollision;
    }

    private void OnDisable()
    {
        CollisionShooter.OnEntitiesCollided -= HandleCollision;
    }

    private void HandleCollision(EntityType who, EntityType whom)
    {
        Debug.Log($"[Manager] {who} bateu em {whom}");

        // if (who == EntityType.Virus && whom == EntityType.RBC) Infect(…);
        // if (who == EntityType.WBC   && whom == EntityType.Virus) DestroyVirus(…);
    }
}
