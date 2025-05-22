using UnityEngine;
using System.Collections.Generic;

public class GameCollisionManager : MonoBehaviour
{
    // Struct para chave de dicionário
    private struct Pair { public GameObject wbc, virus; }

    // Tempo acumulado de contato por par
    private Dictionary<Pair, float> contactTimers = new Dictionary<Pair, float>();

    void OnEnable()
    {
        CollisionShooter.OnEntitiesCollidedEnter += OnColliderEnter;
        CollisionShooter.OnEntitiesCollidedExit += OnColliderExit;
    }

    void OnDisable()
    {
        CollisionShooter.OnEntitiesCollidedEnter -= OnColliderEnter;
        CollisionShooter.OnEntitiesCollidedExit -= OnColliderExit;
    }

    // Quando qualquer colisão entra
    private void OnColliderEnter(
        EntityType who, EntityType whom,
        GameObject whoObj, GameObject whomObj
    )
    {
        if (who == EntityType.WBC && whom == EntityType.Virus)
        {
            var key = new Pair { wbc = whoObj, virus = whomObj };
            if (!contactTimers.ContainsKey(key))
                contactTimers[key] = 0f;
        }
    }

    // Quando sai do contato
    private void OnColliderExit(
        EntityType who, EntityType whom,
        GameObject whoObj, GameObject whomObj
    )
    {
        if (who == EntityType.WBC && whom == EntityType.Virus)
        {
            var key = new Pair { wbc = whoObj, virus = whomObj };
            contactTimers.Remove(key);
        }
    }

    void Update()
    {
        // Liste keys para evitar modificação durante iteração
        var keys = new List<Pair>(contactTimers.Keys);
        foreach (var pair in keys)
        {
            // Se algum foi destruído/desativado, limpe
            if (!pair.wbc || !pair.virus ||
                !pair.wbc.activeInHierarchy ||
                !pair.virus.activeInHierarchy)
            {
                contactTimers.Remove(pair);
                continue;
            }

            // Acumula tempo
            contactTimers[pair] += Time.deltaTime;
            if (contactTimers[pair] >= 3f)
            {
                // Destrói o vírus após 3s de contato
                pair.virus.SetActive(false);
                Debug.Log($"WBC destruiu vírus após 3s de contato");

                contactTimers.Remove(pair);
            }
        }
    }
}
