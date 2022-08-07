using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillTutorialBoss : Objective
    {
        public bool IsActive = false;

        public void SetKillEnemyObjective()
        {
            base.Start();

            //EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
            EventManager.AddListener<BossKillEvent>(OnBossKilled);
            IsActive = true;
        }

        void OnBossKilled(BossKillEvent evt)
        {
            if (IsCompleted)
                return;

            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);
            IsActive = false;

            /*if (gameObject)
            {
                Destroy(gameObject);
            }*/
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<BossKillEvent>(OnBossKilled);
        }
    }
}
