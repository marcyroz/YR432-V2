using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(AIPath))]
public class AIAgent : MonoBehaviour
{
    [Header("Movimento")]
    [Tooltip("Velocidade máxima deste agente (configure no prefab)")]
    [SerializeField] public float moveSpeed = 5f;

    [Header("Alvo")]
    [Tooltip("Quem este agente deve perseguir")]
    [SerializeField] public EntityType targetType;

    [Header("Retargeting")]
    [Tooltip("Segundos entre buscas pelo alvo mais próximo")]
    [SerializeField] private float retargetInterval = 0.5f;

    private AIPath path;
    private Transform target;
    private float retargetTimer;

    void Awake()
    {
        path = GetComponent<AIPath>();
        // inicializa o path usando o valor configurado no Inspector
        path.maxSpeed = moveSpeed;
        retargetTimer = 0f;
    }

    void Update()
    {
        if (path == null) return;

        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f ||
            target == null ||
            !target.gameObject.activeInHierarchy)
        {
            // escolhe a lista certa conforme targetType
            switch (targetType)
            {
                case EntityType.RBC:
                    target = FindClosest(RBCTracker.AllRBCs);
                    break;
                case EntityType.Virus:
                    target = FindClosest(VirusTracker.AllViruses);
                    break;
                // adicione mais cases se precisar
            }
            retargetTimer = retargetInterval;
        }

        if (target != null)
        {
            // garante que a velocidade aplicada seja a do prefab
            path.maxSpeed = moveSpeed;
            path.destination = target.position;
        }
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
}
