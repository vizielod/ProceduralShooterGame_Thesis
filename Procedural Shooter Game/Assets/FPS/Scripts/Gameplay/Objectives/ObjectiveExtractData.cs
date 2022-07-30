using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    /*
     * This is basically and Objective Manager Class
     */
    public class ObjectiveExtractData : Objective
    {
        [Tooltip("Chose whether you need to kill every enemies or only a minimum amount")]
        public bool MustExtractAllData = true;

        [Tooltip("If MustExtractAllData is false, this is the amount of data extract required")]
        public int DatapointsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining Datapoints when this amount of Datapoints is left")]
        public int NotificationDatapointsRemainingThreshold = 3;

        public int targetRemaining = 0;
        public bool playerPerformanceNormalized = false;

        public List<GameObject> CentralComputers = new List<GameObject>();
        public GameObject ObjectiveReachPoint;
        public GameObject ObjectiveKillBoss;
        //public GameObject ObjectiveKillEnemies;
        public GameObject CentralComputer;
        public GameObject ExitDoor;

        public GameObject GameManager;
        public GameObject BossPrefab;
        public GameObject BossRoom;
        private GameObject BossEnemy;

        //public BossManager BossManager;

        int m_DatapointsExtracted = 0;
        
        //public float HardDifficultyGaugeTimer = 0f;
        public float NormalizedHardDifficultyGaugeTimer = 0f;
        //public float MediumToHardDifficultyGaugeTimer = 0f;
        public float NormalizedMediumToHardDifficultyGaugeTimer = 0f;
        //public float EasyToMediumDifficultyGaugeTimer = 0f;
        public float NormalizedEasyToMediumDifficultyGaugeTimer = 0f;
        //public float EasyDifficultyGaugeTimer = 0f;
        public float NormalizedEasyDifficultyGaugeTimer = 0f;

        private void Awake()
        {
            targetRemaining = DatapointsToCompleteObjective - m_DatapointsExtracted;
        }
        protected override void Start()
        {
            //targetRemaining = DatapointsToCompleteObjective - m_DatapointsExtracted;
            /*base.Start();
            
            EventManager.AddListener<ExtractDataEvent>(OnDataExtracted);

            // set a title and description specific for this type of objective, if it hasn't one
            if (string.IsNullOrEmpty(Title))
                Title = "Extract data";

            string description = "Extract data from " + (MustExtractAllData ? "all the" : DatapointsToCompleteObjective.ToString()) +
                                 " Datapoints";

            DatapointsToCompleteObjective = CentralComputers.Count;

            UpdateObjective(description, GetUpdatedCounterAmount(), string.Empty);

            InstantiateReachPointObjectives();*/
        }

        public void SetExtractDataObjectives()
        {
            base.Start();
            
            EventManager.AddListener<ExtractDataEvent>(OnDataExtracted);
            
            if (string.IsNullOrEmpty(Title))
                Title = "Extract data";

            string description = "Extract data from " + (MustExtractAllData ? "all the" : DatapointsToCompleteObjective.ToString()) +
                                 " Datapoints";
            
            DatapointsToCompleteObjective = CentralComputers.Count;

            UpdateObjective(description, GetUpdatedCounterAmount(), string.Empty);

            InstantiateReachPointObjectives();
        }
        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                SpawnComputerAtRandomLocation();
            }
        }

        void InstantiateReachPointObjectives()
        {
            int i = 1;
            foreach (var centralComputer in CentralComputers)
            {
                Vector3 position = new Vector3(centralComputer.transform.position.x + 1f,
                    centralComputer.transform.position.y, centralComputer.transform.position.z - 1f);
                GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
                
                newReachPointObjective.transform.SetParent(centralComputer.transform);

                newReachPointObjective.GetComponent<ObjectiveReachPoint>().Title = $"Reach Extraction Area {i++}";
            }
        }

        public void InstantiateBossRoomReachPointObjective()
        {
            Vector3 position = new Vector3(ExitDoor.transform.position.x,
                ExitDoor.transform.position.y, ExitDoor.transform.position.z);
            GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
                
            newReachPointObjective.transform.SetParent(this.transform);

            newReachPointObjective.GetComponent<ObjectiveReachPoint>().Title = $"Proceed to the Boss Room";
            
            //Destroy(ExitDoor);
            ExitDoor.SetActive(false);
        }

        public void InstantiateKillBossObjective()
        {
            Vector3 position = new Vector3(0, 0, 0);
            GameObject newKillBossObjective = Instantiate(ObjectiveKillBoss, position, Quaternion.identity);
                
            //newReachPointObjective.transform.SetParent(this.transform);

            //newKillBossObjective.GetComponent<ObjectiveReachPoint>().Title = $"Proceed to the Boss Room";
        }

        void OnDataExtracted(ExtractDataEvent evt)
        {
            if (IsCompleted)
                return;
            
            m_DatapointsExtracted++;
            
            /*if (MustExtractAllData)
                DatapointsToCompleteObjective = evt.RemainingDatapoints + m_DatapointsExtracted;*/
            
            //int targetRemaining = MustExtractAllData ? evt.RemainingDatapoints : DatapointsToCompleteObjective - m_DatapointsExtracted;
            targetRemaining = DatapointsToCompleteObjective - m_DatapointsExtracted;
            
            // update the objective text according to how many enemies remain to kill
            if (targetRemaining == 0)
            {
                //BossManager.SpawnBoss();
                SpawnBoss();
                
                InstantiateBossRoomReachPointObjective();
                InstantiateKillBossObjective();
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
                //Spawn the boss when Extraction objectives completed
            }
            else if (targetRemaining == 1)
            {
                string notificationText = NotificationDatapointsRemainingThreshold >= targetRemaining
                    ? "One Datapoint left"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
            else
            {
                // create a notification text if needed, if it stays empty, the notification will not be created
                string notificationText = NotificationDatapointsRemainingThreshold >= targetRemaining
                    ? targetRemaining + " datapoints to extract data from left"
                    : string.Empty;

                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
        }

        string GetUpdatedCounterAmount()
        {
            return m_DatapointsExtracted + " / " + DatapointsToCompleteObjective;
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<ExtractDataEvent>(OnDataExtracted);
        }
        
        private void SpawnComputerAtRandomLocation()
        {
            Debug.Log("Spawn Computer");

            Vector3 position = new Vector3(3.0f, 0.25f, 68.0f);
            GameObject newComputer = Instantiate(CentralComputer, position, Quaternion.identity);
            
            CentralComputers.Add(newComputer);
            //RegisterEnemyController(enemyController);
        }
        
        public void SpawnBoss()
        {
            Vector3 position = new Vector3(BossRoom.transform.position.x, BossRoom.transform.position.y,
                BossRoom.transform.position.z);

            BossEnemy = Instantiate(BossPrefab, position, Quaternion.identity);

            //ObjectiveExtractData.InstantiateKillBossObjective();
        }

        private void CalculateNormalizedPlayerPerformance()
        {
            /*dynamicDifficultyManager.NormalizedMediumToHardDifficultyGaugeTimer =
                timeSpentInDownload / dynamicDifficultyManager.MediumToHardDifficultyGaugeTimer;*/
        }
    }
}