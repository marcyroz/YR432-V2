using UnityEngine;
using System.Collections.Generic;

public class GameCollisionManager : MonoBehaviour
{
    // Struct para chave de dicionário
    private struct Pair { public GameObject wbc, virus; }

    // Tempo acumulado de contato por par
    private Dictionary<Pair, float> contactTimers = new Dictionary<Pair, float>();

    // A cada quantos segundos o WBC aplica dano
    [SerializeField, Tooltip("Intervalo em segundos entre ticks de dano")]
    private float damageInterval = 3f;

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

            if (contactTimers[pair] >= damageInterval)
            {
                // Pega o strength do WBC (via dados)
                var wbcScript = pair.wbc.GetComponent<CellScript>();
                int damage = 0;
                if (wbcScript?.cellData is WhiteBloodCellData wbcData)
                    damage = wbcData.strength;

                // Aplica dano ao vírus
                var virusScript = pair.virus.GetComponent<CellScript>();
                if (virusScript != null)
                {
                    // Mitigação percentual pelo atributo resistance
                    int resistance = virusScript.resistance;
                    int effectiveDamage = Mathf.Max(
                        1,
                        Mathf.RoundToInt(damage * 100f / (100f + resistance))
                    );

                    // Captura vida antes do hit
                    int beforeHealth = virusScript.health;

                    // Aplica o dano mitigado
                    virusScript.TakeDamage(effectiveDamage);

                    // Captura vida depois do hit
                    int afterHealth = virusScript.health;

                    // Log detalhado
                    Debug.Log(
                        $"WBC aplicou {effectiveDamage} de dano ao vírus " +
                        $"(bruto {damage}, res {resistance}) — Vida: {beforeHealth} → {afterHealth}"
                    );

                    // Se o vírus morreu, remova o par
                    if (afterHealth <= 0)
                    {
                        Debug.Log("Vírus foi destruído!");
                        contactTimers.Remove(pair);
                        continue;
                    }
                }

                // Subtrai apenas o intervalo, mantendo sobra de tempo
                contactTimers[pair] -= damageInterval;
            }
        }
    }
}
