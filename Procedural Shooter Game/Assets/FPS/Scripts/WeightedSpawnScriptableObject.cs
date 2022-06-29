using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;

[CreateAssetMenu(fileName = "Weighted Spawn Config", menuName = "ScriptableObject/Weighted Spawn Config")]
public class WeightedSpawnScriptableObject : ScriptableObject
{
    public GameObject EnemyPrefab;
    [Range(0, 1)] public float MinWeight;
    [Range(0, 1)] public float MaxWeight;

    public float GetWeight()
    {
        return Random.Range(MinWeight, MaxWeight);
    }
}

public struct WeightsByDeifficulty
{
    [Range(0, 1)] public float MinWeight;
    [Range(0, 1)] public float MaxWeight;
}
