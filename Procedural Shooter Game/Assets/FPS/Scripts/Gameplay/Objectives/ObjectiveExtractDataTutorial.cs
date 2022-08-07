using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveExtractDataTutorial : Objective
    {
        public bool IsActive = false;

        public GameObject CentralComputer;
        public GameObject ObjectiveReachPoint;
        public Transform BossRoomEntrance;
        public GameObject Boss;
        public ObjectiveKillTutorialBoss ObjectiveKillTutorialBoss;

        [Header("References and Prefabs")]
        public DynamicDifficultyManager dynamicDifficultyManager;
        public SharedDifficultySettingsSO sharedDifficultySettings;
        

        public void SetExtractDataObjectives()
        {
            base.Start();
            
            EventManager.AddListener<ExtractDataEvent>(OnDataExtracted);

            IsActive = true;

            InstantiateReachPointObjectives(CentralComputer.GetComponent<CentralComputer>().ParentRoom.transform.Find("Center").transform.position, $"Reach Extraction Area");
        }

        void InstantiateReachPointObjectives(Vector3 position, string title)
        {
            GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
            newReachPointObjective.GetComponent<ObjectiveReachPoint>().Title = title;
        }

        /*public void InstantiateBossRoomReachPointObjective()
        {
            Vector3 position = new Vector3(ExitDoor.transform.position.x,
                ExitDoor.transform.position.y, ExitDoor.transform.position.z);
            GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
                
            newReachPointObjective.transform.SetParent(this.transform);

            newReachPointObjective.GetComponent<ObjectiveReachPoint>().Title = $"Proceed to the Boss Room";
            
            ExitDoor.SetActive(false);
        }*/

        /*public void InstantiateKillBossObjective()
        {
            Vector3 position = new Vector3(0, 0, 0);
            GameObject newKillBossObjective = Instantiate(ObjectiveKillBoss, position, Quaternion.identity);
        }*/

        void OnDataExtracted(ExtractDataEvent evt)
        {
            if (IsCompleted)
                return;
            
            InstantiateReachPointObjectives(BossRoomEntrance.position, $"Reach Boss Room");
            ObjectiveKillTutorialBoss.SetKillEnemyObjective();
            Boss.SetActive(true);
            
            CompleteObjective(string.Empty, string.Empty, "Objective complete : " + Title);

            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<ExtractDataEvent>(OnDataExtracted);
        }

    }
}
