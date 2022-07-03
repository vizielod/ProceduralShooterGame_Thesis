using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.AI;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{

    [System.Serializable]
    public struct SpawnWeightsByDifficulty
    {
        [SerializeField] public DifficultyType Difficulty;
        [SerializeField] public float[] Weights;
    }
    
    public List<WeightedSpawnScriptableObject> WeightedEnemies = new List<WeightedSpawnScriptableObject>();
    public DynamicDifficultySettingsSO DynamicDifficultySettings;

    private int DifficultyTypeCount = 4;

    [SerializeField] private List<SpawnWeightsByDifficulty> SpawnWeightsByDifficultyList = new List<SpawnWeightsByDifficulty>();
    //[SerializeField] private float[] Weights;
    [SerializeField] private DynamicDifficultyManager dynamicDifficultyManager;

    private void Awake()
    {
        CheckWeightedSpawnSOSetup();
        CheckDifficultySettingsSetup();
        
        for (int j = 0; j < DifficultyTypeCount; j++)
        {
            SpawnWeightsByDifficulty newElement = new SpawnWeightsByDifficulty();
            newElement.Difficulty = (DifficultyType)j;
            newElement.Weights = new float[WeightedEnemies.Count];
            
            SpawnWeightsByDifficultyList.Add(newElement);
        }

        //Weights = new float[WeightedEnemies.Count];
    }

    private void CheckDifficultySettingsSetup()
    {
        if (DynamicDifficultySettings.WeightsByDifficultyList.Count != DifficultyTypeCount)
        {
            Debug.LogError("Dynamic Difficulty Settings SO is not set up correctly. One or more Difficulty option is missing. " +
                           "Make sure are 4 different Difficulty options are added to the Config file. (Hint: Easy, Easy-to-Medium, Medium-to-Hard, Hard)");
        }

        for (int j = 0; j < DifficultyTypeCount; j++)
        {
            if(!DynamicDifficultySettings.WeightsByDifficultyList[j].Difficulty.Equals((DifficultyType)j))
            {
                Debug.LogError($"DynamicDifficultySettings does not have Element {j} set up correctly in the list! " +
                               $"Hint: It should be {(DifficultyType)j}");
            }
        }
    }
    private void CheckWeightedSpawnSOSetup()
    {
        foreach (var weightedEnemy in WeightedEnemies)
        {
            if (weightedEnemy.WeightsByDifficultyList.Count != DifficultyTypeCount)
            {
                Debug.LogError("The Weighted Enemy SO " + weightedEnemy + " does not have all Difficulty Types Set Up Correctly!");
                //return;
            }
            for (int j = 0; j < weightedEnemy.WeightsByDifficultyList.Count; j++)
            {
                if (!weightedEnemy.WeightsByDifficultyList[j].Difficulty
                        .Equals((DifficultyType) j))
                {
                    Debug.LogError(weightedEnemy + " Does not have Difficulty " + (DifficultyType) j + " Set up correctly!");
                    //return;
                }
            }
        }
    }
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
        //Debug.Log("Enemy spawn position: " + newPos);

        /*float random = Random.Range(0, 100);
        Debug.Log("Random enemy: " + random);*/

        if (WeightedEnemies[idx].EnemyPrefab != null)
        {
            InstantiateEnemy(WeightedEnemies[idx].EnemyPrefab, newPos);
            Debug.Log($"SPAWNER: {WeightedEnemies[idx].EnemyPrefab} SPAWNED");
        }
        else
        {
            Debug.Log("SPAWNER: NO ENEMY SPAWNED");
        }
    }

    public void SpawnEnemyByDynamicDifficulty(Vector3 center, float radius)
    {
        /*float Value = Random.value;

        for (int i = 0; i < Weights.Length; i++)
        {
            if (Value < Weights[i])
            {
                //Call Enemy Spawn method
                SpawnEnemy(i, center, radius);
                return;
            }

            Value -= Weights[i];
        }*/
        
        DifficultyType currentDifficulty = GetCurrentDifficulty();

        switch (currentDifficulty)
        {
            case DifficultyType.Hard:
            {
                Debug.Log(DifficultyType.Hard);
                SpawnWeightedRandomEnemy(0, center, radius);
                break;
            }
            case DifficultyType.MediumToHard:
            {
                Debug.Log(DifficultyType.EasyToMedium);
                SpawnWeightedRandomEnemy(1, center, radius);
                break;
            }
            case DifficultyType.EasyToMedium:
            {
                Debug.Log(DifficultyType.EasyToMedium);
                SpawnWeightedRandomEnemy(2, center, radius);
                break;
            }
            case DifficultyType.Easy:
            {
                Debug.Log(DifficultyType.Easy);
                SpawnWeightedRandomEnemy(3, center, radius);
                break;
            }
            default:
                break;
        }
    }

    public void SpawnWeightedRandomEnemy(int idx, Vector3 center, float radius)
    {
        float Value = Random.value;

        for (int i = 0; i < SpawnWeightsByDifficultyList[idx].Weights.Length; i++)
        {
            if (Value < SpawnWeightsByDifficultyList[idx].Weights[i])
            {
                //Call Enemy Spawn method
                SpawnEnemy(i, center, radius);
                return;
            }

            Value -= SpawnWeightsByDifficultyList[idx].Weights[i];
        }
    }
    //SpawnWeightedRandomEnemy

    private DifficultyType GetCurrentDifficulty()
    {
        float currentDifficultyGauge = dynamicDifficultyManager.DifficultyGauge;
        DifficultyType currentDifficulty = DifficultyType.Easy;

        for (int j = 0; j < DynamicDifficultySettings.WeightsByDifficultyList.Count; j++)
        {
            if (j != DynamicDifficultySettings.WeightsByDifficultyList.Count - 1)
            {
                if (currentDifficultyGauge >= DynamicDifficultySettings.WeightsByDifficultyList[j].MinDifficultyGauge &&
                    currentDifficultyGauge < DynamicDifficultySettings.WeightsByDifficultyList[j].MaxDifficultyGauge)
                {
                    currentDifficulty = DynamicDifficultySettings.WeightsByDifficultyList[j].Difficulty;
                }
            }
            else
            {
                if (currentDifficultyGauge >= DynamicDifficultySettings.WeightsByDifficultyList[j].MinDifficultyGauge &&
                    currentDifficultyGauge <= DynamicDifficultySettings.WeightsByDifficultyList[j].MaxDifficultyGauge)
                {
                    currentDifficulty = DynamicDifficultySettings.WeightsByDifficultyList[j].Difficulty;
                }
            }
        }

        return currentDifficulty;
    }
    
    private void InstantiateEnemy(GameObject enemyPrefab, Vector3 position)
    {
        //Debug.Log("Enemy type spawned: " + enemyPrefab);
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        
        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        dynamicDifficultyManager.EnemyControllers.Add(enemyController);
        dynamicDifficultyManager.RegisterEnemyController(enemyController);
    }

    private void ResetSpawnWeights()
    {
        for (int j = 0; j < DifficultyTypeCount; j++)
        {
            float TotalWeight = 0;
            for (int i = 0; i < WeightedEnemies.Count; i++)
            {
                SpawnWeightsByDifficultyList[j].Weights[i] = WeightedEnemies[i].WeightsByDifficultyList[j].GetWeight();
                TotalWeight += SpawnWeightsByDifficultyList[j].Weights[i];
            }
            
            for (int i = 0; i < SpawnWeightsByDifficultyList[j].Weights.Length; i++)
            {
                SpawnWeightsByDifficultyList[j].Weights[i] = SpawnWeightsByDifficultyList[j].Weights[i] / TotalWeight;
            }
        }
        
        /*float TotalWeight = 0;
        for (int i = 0; i < WeightedEnemies.Count; i++)
        {
            Weights[i] = WeightedEnemies[i].GetWeight();
            TotalWeight += Weights[i];
        }

        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Weights[i] / TotalWeight;
        }*/
        
        
    }
}
