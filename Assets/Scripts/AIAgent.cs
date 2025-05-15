using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(AIPath))]
public class AIAgent : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Velocidade máxima deste agente")]
    [SerializeField] public float moveSpeed = 5f;

    [Header("Alvo")]
    [Tooltip("Quem este agente deve perseguir (ou Wander para movimento aleatório)")]
    [SerializeField] public EntityType targetType;

    [Header("Retargeting")]
    [Tooltip("Segundos entre buscas pelo alvo mais próximo")]
    [SerializeField] private float retargetInterval = 0.5f;

    [Header("Wander Settings (para RBC)")]
    [Tooltip("Quanto longe ele pode ir do ponto central")]
    [SerializeField] private float wanderRadius = 3f;
    [Tooltip("Desvio do centro de wander; 0 = centro é o próprio transform")]
    [SerializeField] private float wanderCenterDeviation = 0f;

    private AIPath path;
    private Transform target;
    private float retargetTimer;

    void Awake()
    {
        path = GetComponent<AIPath>();
        path.maxSpeed = moveSpeed;
        retargetTimer = 0f;
    }

    void Update()
    {
        if (path == null) return;

        retargetTimer -= Time.deltaTime;

        // Wander (RBC) ou Chase (outros)
        if (targetType == EntityType.Wander)
        {
            // Wander
            if (retargetTimer <= 0f ||
                Vector2.Distance(transform.position, path.destination) < 0.1f)
            {
                path.destination = GetRandomWanderPoint();
                retargetTimer = retargetInterval;
            }
        }
        else
        {
            // Chase
            if (retargetTimer <= 0f ||
                target == null ||
                !target.gameObject.activeInHierarchy)
            {
                switch (targetType)
                {
                    case EntityType.Virus:
                        target = FindClosest(VirusTracker.AllViruses);
                        break;
                    case EntityType.WBC:
                        target = FindClosest(VirusTracker.AllViruses);
                        break;
                    case EntityType.IRBC:
                    case EntityType.RBC:
                        target = FindClosest(RBCTracker.AllRBCs);
                        break;
                }
                retargetTimer = retargetInterval;
            }

            if (target != null)
                path.destination = target.position;
        }

        // Mantém a velocidade atualizada
        path.maxSpeed = moveSpeed;
    }

    private Transform FindClosest(List<Transform> list)
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        Vector3 pos = transform.position;

        foreach (var t in list)
        {
            if (t == null || !t.gameObject.activeInHierarchy) continue;
            float d = (t.position - pos).sqrMagnitude;
            if (d < bestSqr)
            {
                bestSqr = d;
                best = t;
            }
        }
        return best;
    }

    private Vector3 GetRandomWanderPoint()
    {
        // centro = transform.position + desvio (0 = usa o próprio transform)
        Vector2 center = (Vector2)transform.position + Random.insideUnitCircle * wanderCenterDeviation;
        Vector2 offset = Random.insideUnitCircle * wanderRadius;
        return center + offset;
    }
}
