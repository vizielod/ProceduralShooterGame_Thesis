using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Shared Difficulty Settings", menuName = "ScriptableObject/Shared Difficulty Settings")]
public class SharedDifficultySettingsSO : ScriptableObject
{

    [System.Serializable]
    public struct WeightsByDifficulty
    {
        [FormerlySerializedAs("Difficulty")]
        [Tooltip("This is the Estimated Difficulty, percieved by the player!" +
                 "If it is HARD it means the tension should be decreased by spawning fewer and weaker enemies!" +
                 "If it is EASY it means the tension should be increased by spawning more and stronger enemies!")]
        public DynamicDifficultyType dynamicDifficulty;

        [Range(0, 10)] public int ComputersToSpawnMin;
        [Range(0, 10)] public int ComputersToSpawnMax;
        
        [Range(0, 100)] public int RoomsToSpawnMin;
        [Range(0, 100)] public int RoomsToSpawnMax;
        
    }
    
    public List<WeightsByDifficulty> WeightsByDifficultyList = new List<WeightsByDifficulty>();
}
