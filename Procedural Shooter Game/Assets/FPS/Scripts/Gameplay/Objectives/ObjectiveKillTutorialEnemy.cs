using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillTutorialEnemy : Objective
    {
        public bool IsObjectiveSet = false;
        public void SetKillEnemyObjective()
        {
            base.Start();

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
            IsObjectiveSet = true;
        }

        void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (IsCompleted)
                return;
            
            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}
