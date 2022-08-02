using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS;
using Unity.FPS.AI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Unity.FPS.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {

        [System.Serializable]
        public struct SpawnWeightsByDifficulty
        {
            [FormerlySerializedAs("Difficulty")] [SerializeField]
            public DynamicDifficultyType dynamicDifficulty;

            [SerializeField] public float[] Weights;
        }

        public List<WeightedSpawnScriptableObject> WeightedEnemies = new List<WeightedSpawnScriptableObject>();
        public DynamicDifficultySettingsSO DynamicDifficultySettings;
        public DynamicDifficultyType currentDynamicDifficulty = DynamicDifficultyType.Easy;

        private int DifficultyTypeCount = 4;

        [SerializeField]
        private List<SpawnWeightsByDifficulty> SpawnWeightsByDifficultyList = new List<SpawnWeightsByDifficulty>();

        //[SerializeField] private float[] Weights;
        [SerializeField] private DynamicDifficultyManager dynamicDifficultyManager;

        private void Awake()
        {
            CheckWeightedSpawnSOSetup();
            CheckDifficultySettingsSetup();

            for (int j = 0; j < DifficultyTypeCount; j++)
            {
                SpawnWeightsByDifficulty newElement = new SpawnWeightsByDifficulty();
                newElement.dynamicDifficulty = (DynamicDifficultyType) j;
                newElement.Weights = new float[WeightedEnemies.Count];

                SpawnWeightsByDifficultyList.Add(newElement);
            }

            //Weights = new float[WeightedEnemies.Count];
        }

        private void CheckDifficultySettingsSetup()
        {
            if (DynamicDifficultySettings.DifficultyTypeByGaugeValueList.Count != DifficultyTypeCount)
            {
                Debug.LogError(
                    "Dynamic Difficulty Settings SO is not set up correctly. One or more Difficulty option is missing. " +
                    "Make sure all 4 different Difficulty options are added to the Config file. (Hint: Easy, Easy-to-Medium, Medium-to-Hard, Hard)");
            }

            for (int j = 0; j < DifficultyTypeCount; j++)
            {
                if (!DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j].dynamicDifficulty
                        .Equals((DynamicDifficultyType) j))
                {
                    Debug.LogError(
                        $"DynamicDifficultySettings does not have Element {j} set up correctly in the list! " +
                        $"Hint: It should be {(DynamicDifficultyType) j}");
                }
            }
        }

        private void CheckWeightedSpawnSOSetup()
        {
            foreach (var weightedEnemy in WeightedEnemies)
            {
                if (weightedEnemy.WeightsByDifficultyList.Count != DifficultyTypeCount)
                {
                    Debug.LogError("The Weighted Enemy SO " + weightedEnemy +
                                   " does not have all Difficulty Types Set Up Correctly!");
                    //return;
                }

                for (int j = 0; j < weightedEnemy.WeightsByDifficultyList.Count; j++)
                {
                    if (!weightedEnemy.WeightsByDifficultyList[j].dynamicDifficulty
                            .Equals((DynamicDifficultyType) j))
                    {
                        Debug.LogError(weightedEnemy + " Does not have Difficulty " + (DynamicDifficultyType) j +
                                       " Set up correctly!");
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
            if (WeightedEnemies[idx].EnemyPrefab == null)
            {
                Debug.Log("SPAWNER: NO ENEMY SPAWNED");
                return;
            }
            
            
            Debug.Log("Spawn Enemy");
            /*float angle = Mathf.PI * 2f / 10 * i;
            i++;
            Vector3 newPos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);*/
            Vector3 randomSpawnCoordinate = Vector3.zero;
            Vector3 randomRange = Vector3.zero;
            bool isSpawnPositionTooCloseToPlayer = true;
            do
            {
                randomRange = new Vector3(Random.Range(-35f, 35f), 0, Random.Range(-35f, 35f));
                randomSpawnCoordinate = center + randomRange;

                //Making sure that enemies does not spawn too close to the player right away.
                if (Vector3.Distance(dynamicDifficultyManager.Player.transform.position, randomSpawnCoordinate) <=
                    WeightedEnemies[idx].EnemyPrefab.GetComponent<Enemy>().DetectionModule.AttackRange)
                {
                    isSpawnPositionTooCloseToPlayer = true;
                }
                else
                {
                    isSpawnPositionTooCloseToPlayer = false;
                }
            } while (isSpawnPositionTooCloseToPlayer);

            InstantiateEnemy(WeightedEnemies[idx].EnemyPrefab, randomSpawnCoordinate);
            Debug.Log($"SPAWNER: {WeightedEnemies[idx].EnemyPrefab} SPAWNED");

            /*if (WeightedEnemies[idx].EnemyPrefab != null)
            {
                InstantiateEnemy(WeightedEnemies[idx].EnemyPrefab, randomSpawnCoordinate);
                Debug.Log($"SPAWNER: {WeightedEnemies[idx].EnemyPrefab} SPAWNED");
            }
            else
            {
                Debug.Log("SPAWNER: NO ENEMY SPAWNED");
            }*/
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

            DynamicDifficultyType currentDynamicDifficulty = GetCurrentDifficulty();

            switch (currentDynamicDifficulty)
            {
                case DynamicDifficultyType.Hard:
                {
                    Debug.Log(DynamicDifficultyType.Hard);
                    SpawnWeightedRandomEnemy(0, center, radius);
                    break;
                }
                case DynamicDifficultyType.MediumToHard:
                {
                    Debug.Log(DynamicDifficultyType.EasyToMedium);
                    SpawnWeightedRandomEnemy(1, center, radius);
                    break;
                }
                case DynamicDifficultyType.EasyToMedium:
                {
                    Debug.Log(DynamicDifficultyType.EasyToMedium);
                    SpawnWeightedRandomEnemy(2, center, radius);
                    break;
                }
                case DynamicDifficultyType.Easy:
                {
                    Debug.Log(DynamicDifficultyType.Easy);
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

        private DynamicDifficultyType GetCurrentDifficulty()
        {
            float currentDifficultyGauge = dynamicDifficultyManager.DifficultyGauge;
            //DynamicDifficultyType currentDynamicDifficulty = DynamicDifficultyType.Easy;

            for (int j = 0; j < DynamicDifficultySettings.DifficultyTypeByGaugeValueList.Count; j++)
            {
                if (j != DynamicDifficultySettings.DifficultyTypeByGaugeValueList.Count - 1)
                {
                    if (currentDifficultyGauge >=
                        DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j].MinDifficultyGauge &&
                        currentDifficultyGauge < DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .MaxDifficultyGauge)
                    {
                        currentDynamicDifficulty = DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .dynamicDifficulty;
                    }
                }
                else //This is needed so if the Gauge is at 1 it also takes it into account
                {
                    if (currentDifficultyGauge >=
                        DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j].MinDifficultyGauge &&
                        currentDifficultyGauge <= DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .MaxDifficultyGauge)
                    {
                        currentDynamicDifficulty = DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j]
                            .dynamicDifficulty;
                    }
                }
            }

            return currentDynamicDifficulty;
        }

        private void InstantiateEnemy(GameObject enemyPrefab, Vector3 position)
        {
            //Debug.Log("Enemy type spawned: " + enemyPrefab);
            
            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);

            Vector3 dir = (dynamicDifficultyManager.Player.transform.position - newEnemy.transform.position).normalized;

            newEnemy.transform.forward = dir;

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
                    SpawnWeightsByDifficultyList[j].Weights[i] =
                        WeightedEnemies[i].WeightsByDifficultyList[j].GetWeight();
                    TotalWeight += SpawnWeightsByDifficultyList[j].Weights[i];
                }

                for (int i = 0; i < SpawnWeightsByDifficultyList[j].Weights.Length; i++)
                {
                    SpawnWeightsByDifficultyList[j].Weights[i] =
                        SpawnWeightsByDifficultyList[j].Weights[i] / TotalWeight;
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
}
