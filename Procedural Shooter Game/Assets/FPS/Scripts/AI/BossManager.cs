using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
//using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.AI
{
    public class BossManager : MonoBehaviour
    {
        public List<EnemyController> Bosses { get; /*private */set; }
        public int NumberOfBossesTotal { get; /*private */set; }
        public int NumberOfBossesRemaining => Bosses.Count;

        void Awake()
        {
            Bosses = new List<EnemyController>();
        }

        public void RegisterBoss(EnemyController enemy)
        {
            Bosses.Add(enemy);

            NumberOfBossesTotal++;
        }

        public void UnregisterBoss(EnemyController enemyKilled)
        {
            int enemiesRemainingNotification = NumberOfBossesRemaining - 1;

            BossKillEvent evt = Events.BossKillEvent;
            evt.Boss = enemyKilled.gameObject;
            evt.RemainingBossCount = enemiesRemainingNotification;
            EventManager.Broadcast(evt);

            // removes the enemy from the list, so that we can keep track of how many are left on the map
            Bosses.Remove(enemyKilled);
        }
    }
}
