using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCollisionManager : MonoBehaviour
{
    // 1) Estrutura para par WBC→Vírus (para dano)
    private struct DamagePair { public GameObject wbc, virus; }
    private Dictionary<DamagePair, float> contactTimers = new Dictionary<DamagePair, float>();

    // 2) Estrutura para par Vírus→RBC (para infecção)
    private struct InfectionPair { public GameObject virus, rbc; }
    private Dictionary<InfectionPair, float> infectionTimers = new Dictionary<InfectionPair, float>();

    [Header("Configurações de Tick/Dano")]
    [SerializeField, Tooltip("Intervalo em segundos entre ticks de dano do WBC no vírus")]
    private float damageInterval = 3f;

    [Header("Configurações de Infecção")]
    [SerializeField, Tooltip("Tempo em contato para infectar o RBC")]
    private float infectionInterval = 3f;
    [SerializeField, Tooltip("Tempo em segundos que o RBC fica infectado antes de virar vírus")]
    private float conversionDelay = 4f;

    // Referência ao CellSpawnerScript, para obter CellData/CellStats ao converter RBC→Vírus
    private CellSpawnerScript spawner;

    [System.Obsolete]
    void Awake()
    {
        // Apenas um CellSpawnerScript em cena
        spawner = FindObjectOfType<CellSpawnerScript>();
        if (spawner == null)
            Debug.LogError("GameCollisionManager: não encontrou nenhum CellSpawnerScript em cena.");
    }

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

    private void OnColliderEnter(
        EntityType who, EntityType whom,
        GameObject whoObj, GameObject whomObj
    )
    {
        // **1) WBC tocando Vírus → comece a contar para aplicar dano** 
        if (who == EntityType.WBC && whom == EntityType.Virus)
        {
            var key = new DamagePair { wbc = whoObj, virus = whomObj };
            if (!contactTimers.ContainsKey(key))
                contactTimers[key] = 0f;
        }

        // **2) Vírus tocando RBC → comece a contar para infectar**
        if (who == EntityType.Virus && whom == EntityType.RBC)
        {
            var key = new InfectionPair { virus = whoObj, rbc = whomObj };
            if (!infectionTimers.ContainsKey(key))
                infectionTimers[key] = 0f;
        }
    }

    private void OnColliderExit(
        EntityType who, EntityType whom,
        GameObject whoObj, GameObject whomObj
    )
    {
        // **1) Se WBC sair do contato com Vírus, zere o contador de dano**
        if (who == EntityType.WBC && whom == EntityType.Virus)
        {
            var key = new DamagePair { wbc = whoObj, virus = whomObj };
            contactTimers.Remove(key);
        }

        // **2) Se Vírus sair do contato com RBC, zere o contador de infecção**
        if (who == EntityType.Virus && whom == EntityType.RBC)
        {
            var key = new InfectionPair { virus = whoObj, rbc = whomObj };
            infectionTimers.Remove(key);
        }
    }

    void Update()
    {
        ProcessDamageTicks();
        ProcessInfectionTicks();
    }


    //            1) Lógica de dano (WBC → Vírus)
    private void ProcessDamageTicks()
    {
        var keys = new List<DamagePair>(contactTimers.Keys);
        foreach (var pair in keys)
        {
            // Se algum GameObject foi destruído/desativado, limpe
            if (!pair.wbc || !pair.virus ||
                !pair.wbc.activeInHierarchy ||
                !pair.virus.activeInHierarchy)
            {
                contactTimers.Remove(pair);
                continue;
            }

            // Acumula tempo até o próximo tick
            contactTimers[pair] += Time.deltaTime;

            if (contactTimers[pair] >= damageInterval)
            {
                // 1.1 Recupera strength do WBC via CellScript
                var wbcScript = pair.wbc.GetComponent<CellScript>();
                int baseDamage = 0;
                if (wbcScript?.cellData is WhiteBloodCellData wbcData)
                    baseDamage = wbcData.strength;

                // 1.2 Mitigação pelo atributo resistance do Vírus
                var virusScript = pair.virus.GetComponent<CellScript>();
                if (virusScript != null)
                {
                    int resistance = virusScript.resistance;
                    float factor = 100f / (100f + resistance);
                    float rawDamageF = baseDamage * factor;
                    int effectiveDamage = Mathf.FloorToInt(rawDamageF);

                    if (effectiveDamage > 0)
                    {
                        int beforeHealth = virusScript.health;
                        virusScript.TakeDamage(effectiveDamage);
                        int afterHealth = virusScript.health;

                        Debug.Log(
                            $"WBC aplicou {effectiveDamage} dmg ao Vírus " +
                            $"(bruto {baseDamage}, res {resistance:.##}, raw {rawDamageF:F2}) — " +
                            $"Vida: {beforeHealth} → {afterHealth}"
                        );

                        if (afterHealth <= 0)
                        {
                            Debug.Log("Vírus foi destruído!");
                            contactTimers.Remove(pair);
                            continue;
                        }
                    }
                    else
                    {
                        // nenhum dano aplicado neste tick, mostra log simplificado
                        Debug.Log(
                            $"WBC tentou aplicar {baseDamage} dmg no Vírus, " +
                            $"mas resist {resistance} mitiga para 0."
                        );
                    }
                }

                // Remove o intervalo, mas mantém sobras
                contactTimers[pair] -= damageInterval;
            }
        }
    }


    // 2) Lógica de infecção (Vírus → RBC)
    private void ProcessInfectionTicks()
    {
        // Pegamos todos os pares atuais de “vírus→rbc” que estão drenando vida
        var keys = new List<InfectionPair>(infectionTimers.Keys);
        foreach (var pair in keys)
        {
            // 1) Se algum GameObject foi destruído ou ficou inativo, removemos o par
            if (!pair.virus || !pair.rbc ||
                !pair.virus.activeInHierarchy ||
                !pair.rbc.activeInHierarchy)
            {
                infectionTimers.Remove(pair);
                continue;
            }

            // 2) Já recuperamos o CellScript do RBC para checar se já está infectado
            var rbcScript = pair.rbc.GetComponent<CellScript>();
            if (rbcScript == null)
            {
                infectionTimers.Remove(pair);
                continue;
            }

            // não deixamos ele “infectar” novamente. Só descartamos este par.
            if (rbcScript.isInfectedInstance)
            {
                infectionTimers.Remove(pair);
                continue;
            }

            // 3) Acumulamos tempo até o próximo tick de “dreno de vida”
            infectionTimers[pair] += Time.deltaTime;

            if (infectionTimers[pair] >= infectionInterval)
            {
                // 4) Recupera strength do Vírus via CellScript (quem está infectando)
                var virusScript = pair.virus.GetComponent<CellScript>();
                int baseDamage = 0;
                if (virusScript?.cellData is VirusData vData)
                    baseDamage = vData.strength;

                // 5) Agora aplicamos dano mitigado ao RBC manualmente (sem TakeDamage())
                int resistance = rbcScript.resistance;
                float factor = 100f / (100f + resistance);
                float rawDamageF = baseDamage * factor;
                int effectiveDamage = Mathf.FloorToInt(rawDamageF);

                int beforeHealth = rbcScript.health;
                rbcScript.health -= effectiveDamage;
                int afterHealth = rbcScript.health;

                Debug.Log(
                    $"Vírus aplicou {effectiveDamage} dmg ao RBC  " +
                    $"(bruto {baseDamage}, res {resistance}, raw {rawDamageF:F2})  " +
                    $"— Vida: {beforeHealth} → {afterHealth}"
                );

                // 6) Se o RBC ainda tiver vida (>0), subtraímos só o intervalo e aguardamos o próximo tick
                if (afterHealth > 0)
                {
                    infectionTimers[pair] -= infectionInterval;
                    continue;
                }

                // 7) Quando a vida chega a zero (ou menos), “infectamos de vez” chamando InfectRBC()
                InfectRBC(pair.rbc);
                GameStatsTracker.Instance?.RegisterInfection();

                // 8) Removemos este par para não drenar de novo
                infectionTimers.Remove(pair);
            }
        }
    }




    //    Infecta o RBC (quando atingir 0 de vida), sem alterar
    //    o ScriptableObject original (para não macular futuros spawns)
    private void InfectRBC(GameObject rbcObj)
    {
        var cellScript = rbcObj.GetComponent<CellScript>();
        if (cellScript == null) return;

        // 0) Dizemos que esta instância virou um IRBC
        cellScript.isInfectedInstance = true;

        // 1) Remove um “RBC” do CountBoard e adiciona “IRBC”
        if (CellScript.countBoard != null)
        {
            CellScript.countBoard.removeEntity("RBC");
            CellScript.countBoard.addEntity("IRBC");
        }

        // 2) Muda o sprite para infectado
        var rbcBehavior = rbcObj.GetComponent<RBCBehavior>();
        if (rbcBehavior != null)
            rbcBehavior.Infect();

        Debug.Log($"RBC {rbcObj.name} foi infectado (HP ≤ 0) pelo Vírus!");

        // 3) Para de ser alvo de pathfinding
        RBCTracker.AllRBCs.Remove(rbcObj.transform);

        // 4) Agendamos a conversão em Vírus após o delay
        StartCoroutine(ConvertRBCtoVirus(rbcObj));
    }

    private IEnumerator ConvertRBCtoVirus(GameObject rbcObj)
    {
        yield return new WaitForSeconds(conversionDelay);

        if (rbcObj == null || !rbcObj.activeInHierarchy)
            yield break;

        // Guarda a posição para spawnar o vírus
        Vector3 pos = rbcObj.transform.position;

        // 1) Desativa o próprio RBC infectado.
        //    Como isInfectedInstance == true e cellData.entityType == "RBC",
        //    o OnDisable de CellScript removerá exatamente um “IRBC” do painel.
        rbcObj.SetActive(false);
        Debug.Log($"RBC {rbcObj.name} agora será convertido em Vírus.");
        GameStatsTracker.Instance?.RegisterTransformation();

        // 2) Spawn do novo Vírus no mesmo lugar (permitindo expansão da pool)
        GameObject newVirus = ObjectPooler.Instance.SpawnFromPool(
            "Virus",
            pos,
            Quaternion.identity,
            true   // permite expandir além do size original
        );

        spawner.RegisterExternalSpawn(newVirus);

        if (newVirus != null)
        {
            // 2.1) Inicializa CellScript do novo Virus
            CellData virusData = spawner.GetBaseData("Virus");
            CellStats virusStats = spawner.GetStatsFor("Virus");
            var newScript = newVirus.GetComponent<CellScript>();
            newScript?.Initialize(virusData, virusStats);

            // 2.2) Atualiza o AIAgent, se houver
            var agent = newVirus.GetComponent<AIAgent>();
            if (agent != null)
            {
                agent.moveSpeed = virusStats.velocity;
                agent.targetType = EntityType.RBC;
            }

            Debug.Log("Novo Vírus criado a partir do RBC infectado.");
        }
        else
        {
            Debug.LogWarning("Falha ao spawnar o novo Vírus da pool.");
        }
    }

}
