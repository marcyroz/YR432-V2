using UnityEngine;
using Pathfinding;

public class AIAgent : MonoBehaviour
{
    [HideInInspector] public float moveSpeed;
    private AIPath path;
    private Transform target;

    [SerializeField] private float retargetInterval = 0.5f;
    private float retargetTimer;

    void Awake()
    {
        path = GetComponent<AIPath>();
        retargetTimer = 0f;
    }

    /// <summary>
    /// Permite ao spawner definir um alvo inicial ao instanciar o vírus.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (path == null) return;

        // Decrementa o timer
        retargetTimer -= Time.deltaTime;

        // Se chegou a hora de retarget ou target inválido
        if (retargetTimer <= 0f ||
            target == null ||
            !target.gameObject.activeInHierarchy)
        {
            target = FindClosestRBC();
            retargetTimer = retargetInterval;
        }

        // Atualiza destino
        if (target != null)
        {
            path.maxSpeed = moveSpeed;
            path.destination = target.position;
        }
    }

    /// <summary>
    /// Varre todos os RBC em cena e retorna o mais próximo.
    /// </summary>
    private Transform FindClosestRBC()
    {
        Transform best = null;
        float bestSqr = float.MaxValue;
        Vector3 pos = transform.position;

        foreach (var rbc in RBCTracker.AllRBCs)
        {
            if (rbc == null || !rbc.gameObject.activeInHierarchy) continue;
            float d = (rbc.position - pos).sqrMagnitude;
            if (d < bestSqr)
            {
                bestSqr = d;
                best = rbc;
            }
        }
        return best;
    }
}
