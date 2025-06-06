using System.Collections.Generic;
using UnityEngine;

public class VirusTracker : MonoBehaviour
{
    public static readonly List<Transform> AllViruses = new List<Transform>();

    void Awake()
    {
        AllViruses.Add(transform);
    }

    void OnDestroy()
    {
        AllViruses.Remove(transform);
    }
}
