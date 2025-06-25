using System.Collections.Generic;
using UnityEngine;

public class VictoryTracker : MonoBehaviour
{
    public static VictoryTracker Instance;

    public List<VictoryStats> results = new List<VictoryStats>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Clear() => results.Clear();
}
