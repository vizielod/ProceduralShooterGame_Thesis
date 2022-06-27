using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.AI
{
    [CreateAssetMenu(fileName = "Enemy Loot Table Config", menuName = "ScriptableObject/Enemy Loot Table Config")]
    public class LootTableScriptableObject : ScriptableObject
    {
        [System.Serializable]
        public struct SpawnObject
        {
            public GameObject spawnObject;
            public int minProbabilityRange;
            public int maxProbabilityRange;
        }

        public SpawnObject[] HealthPickupPrefabs;
    }
}
