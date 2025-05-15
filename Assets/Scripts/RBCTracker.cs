using System.Collections.Generic;
using UnityEngine;

public class RBCTracker : MonoBehaviour
{
    // Lista estática com todos os Transforms de RBC ativos em cena
    public static readonly List<Transform> AllRBCs = new List<Transform>();

    void Awake()
    {
        AllRBCs.Add(transform);
    }

    void OnDestroy()
    {
        AllRBCs.Remove(transform);
    }
}
