using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillBoss : Objective
    {
        [Tooltip("Chose whether you need to kill every enemies or only a minimum amount")]
        public bool MustKillAllBosses = true;
        
        [Tooltip("If MustKillAllEnemies is false, this is the amount of enemy kills required")]
        public int KillsToCompleteObjective = 1;
        
        int m_KillTotal;
        
        protected override void Start()
        {
            base.Start();

            EventManager.AddListener<BossKillEvent>(OnBossKilled);

            // set a title and description specific for this type of objective, if it hasn't one
            /*if (string.IsNullOrEmpty(Title))
                Title = "Eliminate the Boss";*/
            
            if (string.IsNullOrEmpty(Description))
                Description = GetUpdatedCounterAmount();
        }

        void OnBossKilled(BossKillEvent evt)
        {
            if (IsCompleted)
                return;
            
            m_KillTotal++;
            
            if (MustKillAllBosses)
                KillsToCompleteObjective = evt.RemainingBossCount + m_KillTotal;
            
            int targetRemaining = MustKillAllBosses ? evt.RemainingBossCount : KillsToCompleteObjective - m_KillTotal;

            if (targetRemaining == 0)
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
        }
        
        string GetUpdatedCounterAmount()
        {
            return m_KillTotal + " / " + KillsToCompleteObjective;
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<BossKillEvent>(OnBossKilled);
        }
    }
}