using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    public List<WeightedSpawnScriptableObject> WeightedEnemies = new List<WeightedSpawnScriptableObject>();

    [SerializeField] private float[] Weights;

    private void Awake()
    {
        Weights = new float[WeightedEnemies.Count];
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetSpawnWeights();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnWeightedRandomEnemy()
    {
        float Value = Random.value;

        for (int i = 0; i < Weights.Length; i++)
        {
            if (Value < Weights[i])
            {
                //Call Enemy Spawn method
                return;
            }

            Value -= Weights[i];
        }
        
    }
    private void SpawnEnemies()
    {
        //Call this before we spawn enemies
        ResetSpawnWeights();
    }
    private void ResetSpawnWeights()
    {
        float TotalWeight = 0;
        for (int i = 0; i < WeightedEnemies.Count; i++)
        {
            Weights[i] = WeightedEnemies[i].GetWeight();
            TotalWeight += Weights[i]; 
        }

        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Weights[i] / TotalWeight;
        }
    }
}
