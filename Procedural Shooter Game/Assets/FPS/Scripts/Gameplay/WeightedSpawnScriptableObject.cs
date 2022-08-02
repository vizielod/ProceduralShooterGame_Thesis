using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Unity.FPS.Gameplay
{
    [CreateAssetMenu(fileName = "Weighted Spawn Config", menuName = "ScriptableObject/Weighted Spawn Config")]
    public class WeightedSpawnScriptableObject : ScriptableObject
    {
        /*public enum DifficultyType{
            Easy = 0,
            EasyToMedium = 1,
            MediumToHard = 2,
            Hard = 3,
        }*/

        [System.Serializable]
        public class WeightsByDifficulty
        {
            [FormerlySerializedAs("Difficulty")]
            [Tooltip("This is the Estimated Difficulty, percieved by the player!" +
                     "If it is HARD it means the tension should be decreased by spawning fewer and weaker enemies!" +
                     "If it is EASY it means the tension should be increased by spawning more and stronger enemies!")]
            public DynamicDifficultyType dynamicDifficulty;

            [Range(0, 1)] public float MinWeight;
            [Range(0, 1)] public float MaxWeight;

            public float GetWeight()
            {
                return Random.Range(MinWeight, MaxWeight);
            }
        }

        public GameObject EnemyPrefab;
        public List<WeightsByDifficulty> WeightsByDifficultyList = new List<WeightsByDifficulty>();
        public int DifficultyTypeCount = Enum.GetValues(typeof(DynamicDifficultyType)).Length;

    }
}