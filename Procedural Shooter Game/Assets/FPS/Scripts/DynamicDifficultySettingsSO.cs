using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dynamic Difficulty Settings", menuName = "ScriptableObject/Dynamic Difficulty Settings")]
public class DynamicDifficultySettingsSO : ScriptableObject
{
    /*public enum DifficultyType{
        Easy = 0,
        EasyToMedium = 1,
        MediumToHard = 2,
        Hard = 3,
    }*/
    
    [System.Serializable]
    public struct WeightsByDifficulty
    {
        [Tooltip("This is the Estimated Difficulty, percieved by the player!" +
                 "If it is HARD it means the tension should be decreased by spawning fewer and weaker enemies!" +
                 "If it is EASY it means the tension should be increased by spawning more and stronger enemies!")]
        public DifficultyType Difficulty;
    
        [Range(0, 1)] public float MinDifficultyGauge;
        [Range(0, 1)] public float MaxDifficultyGauge;
        
    }
    
    public List<WeightsByDifficulty> WeightsByDifficultyList = new List<WeightsByDifficulty>();
}
