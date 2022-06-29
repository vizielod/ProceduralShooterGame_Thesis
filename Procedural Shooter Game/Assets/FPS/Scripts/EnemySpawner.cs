using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    public List<WeightedSpawnScriptableObject> WeightedEnemies = new List<WeightedSpawnScriptableObject>();

    [SerializeField] private float[] Weights;
    [SerializeField] private DynamicDifficultyManager dynamicDifficultyManager;

    private void Awake()
    {
        Weights = new float[WeightedEnemies.Count];
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetSpawnWeights();
    }

    private int i = 0;
    public void SpawnEnemy(int idx, Vector3 center, float radius)
    {
        Debug.Log("Spawn Enemy");
        //int i = Random.Range(0, 100);
        float angle = Mathf.PI * 2f / 10 * i;
        i++;
        Vector3 newPos = center + new Vector3(Mathf.Cos(angle)*radius, 0, Mathf.Sin(angle)*radius);
        Debug.Log("Enemy spawn position: " + newPos);
        //Vector3 position = new Vector3(-45f, 0, 70f);

        float random = Random.Range(0, 100);
        Debug.Log("Random enemy: " + random);

        if (WeightedEnemies[idx].EnemyPrefab != null)
        {
            InstantiateEnemy(WeightedEnemies[idx].EnemyPrefab, newPos);
            Debug.Log("SPAWNER: ENEMY SPAWNED");
        }
        else
        {
            Debug.Log("SPAWNER: NO ENEMY SPAWNED");
        }
    }

    public void SpawnWeightedRandomEnemy(Vector3 center, float radius)
    {
        float Value = Random.value;

        for (int i = 0; i < Weights.Length; i++)
        {
            if (Value < Weights[i])
            {
                //Call Enemy Spawn method
                SpawnEnemy(i, center, radius);
                return;
            }

            Value -= Weights[i];
        }
        
    }
    
    private void InstantiateEnemy(GameObject enemyPrefab, Vector3 position)
    {
        Debug.Log("Enemy type spawned: " + enemyPrefab);
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        dynamicDifficultyManager.EnemyControllers.Add(enemyController);
        dynamicDifficultyManager.RegisterEnemyController(enemyController);
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
