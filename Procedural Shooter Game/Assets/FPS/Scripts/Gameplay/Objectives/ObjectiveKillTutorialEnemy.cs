using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillTutorialEnemy : Objective
    {
        public bool IsActive = false;
        public GameObject ObjectiveReachPoint;
        public GameObject CentralComputer;
        public ObjectiveExtractDataTutorial ObjectiveExtractDataTutorial;
        public void SetKillEnemyObjective()
        {
            base.Start();

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
            IsActive = true;
        }

        void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (IsCompleted)
                return;
            
            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);
            IsActive = false;

            //InstantiateReachPointObjectives();
            ObjectiveExtractDataTutorial.SetExtractDataObjectives();

            /*if (gameObject)
            {
                Destroy(gameObject);
            }*/
        }
        
        void InstantiateReachPointObjectives()
        {
            Vector3 position = CentralComputer.GetComponent<CentralComputer>().ParentRoom.transform.Find("Center").transform.position;
            GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
            newReachPointObjective.GetComponent<ObjectiveReachPoint>().Title = $"Reach Extraction Area";
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
        }
    }
}
