using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [Header("References and Prefabs")]
        public DynamicDifficultyManager dynamicDifficultyManager;
        public SharedDifficultySettingsSO sharedDifficultySettings;
        public GameObject ObjectiveReachPoint;
        public GameObject ObjectiveKillBoss;
        public GameObject CentralComputer;
        public GameObject GameManager;
        public GameObject BossPrefab;
        public GameObject BossRoom;
        public GameObject ExitDoor;

        private GameObject BossEnemy;
        private int m_DatapointsExtracted = 0;

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
                Vector3 position = centralComputer.GetComponent<CentralComputer>().ParentRoom.transform.Find("Center").transform.position;

                GameObject newReachPointObjective = Instantiate(ObjectiveReachPoint, position, Quaternion.identity);
                
                centralComputer.GetComponent<DownloadManager>().ReachPointIndex = i;
                //centralComputer.GetComponent<DownloadManager>().ObjectiveReachPoint = newReachPointObjective.GetComponent<ObjectiveReachPoint>();

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
                dynamicDifficultyManager.CalculateNormalizedPlayerPerformance();
                
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

        public void ChangeBossRoomSetup(StaticDifficultyType currentDifficulty)
        {
            BossRoomManager bossRoomManager = BossRoom.GetComponent<BossRoomManager>();
            switch (currentDifficulty)
            {
                case StaticDifficultyType.Hard:
                {
                    bossRoomManager.ColorBossRoom(Color.red);
                    bossRoomManager.InternalWalls_Hard.SetActive(true);
                    bossRoomManager.InternalWalls_Medium.SetActive(false);
                    bossRoomManager.InternalWalls_Easy.SetActive(false);
                    break;
                }
                case StaticDifficultyType.Medium:
                {
                    bossRoomManager.ColorBossRoom(Color.magenta);
                    bossRoomManager.InternalWalls_Hard.SetActive(false);
                    bossRoomManager.InternalWalls_Medium.SetActive(true);
                    bossRoomManager.InternalWalls_Easy.SetActive(false);
                    break;
                }
                case StaticDifficultyType.Easy:
                {
                    bossRoomManager.ColorBossRoom(Color.yellow);
                    bossRoomManager.InternalWalls_Hard.SetActive(false);
                    bossRoomManager.InternalWalls_Medium.SetActive(false);
                    bossRoomManager.InternalWalls_Easy.SetActive(true);
                    break;
                }
                default:
                    break;
            }
            
        }

        public void ChangeBossRoomSetup(DynamicDifficultyType currentDifficulty)
        {
            BossRoomManager bossRoomManager = BossRoom.GetComponent<BossRoomManager>();
            
            switch (currentDifficulty)
            {
                case DynamicDifficultyType.Hard:
                {
                    bossRoomManager.ColorBossRoom(Color.red);
                    bossRoomManager.InternalWalls_Hard.SetActive(true);
                    bossRoomManager.InternalWalls_Medium.SetActive(false);
                    bossRoomManager.InternalWalls_Easy.SetActive(false);
                    break;
                }
                case DynamicDifficultyType.MediumToHard:
                {
                    bossRoomManager.ColorBossRoom(Color.magenta);
                    bossRoomManager.InternalWalls_Hard.SetActive(false);
                    bossRoomManager.InternalWalls_Medium.SetActive(true);
                    bossRoomManager.InternalWalls_Easy.SetActive(false);
                    break;
                }
                case DynamicDifficultyType.EasyToMedium:
                {
                    bossRoomManager.ColorBossRoom(Color.magenta);
                    bossRoomManager.InternalWalls_Hard.SetActive(false);
                    bossRoomManager.InternalWalls_Medium.SetActive(true);
                    bossRoomManager.InternalWalls_Easy.SetActive(false);
                    break;
                }
                case DynamicDifficultyType.Easy:
                {
                    bossRoomManager.ColorBossRoom(Color.yellow);
                    bossRoomManager.InternalWalls_Hard.SetActive(false);
                    bossRoomManager.InternalWalls_Medium.SetActive(false);
                    bossRoomManager.InternalWalls_Easy.SetActive(true);
                    break;
                }
                default:
                    break;
            }
        }

        public void SpawnBoss()
        {
            /*Vector3 position = new Vector3(BossRoom.transform.position.x, BossRoom.transform.position.y,
                BossRoom.transform.position.z);*/

            if (BossRoom == null)
                return;

            Vector3 position = BossRoom.GetComponent<BossRoomManager>().Center.position;
            
            GameObject BossToSpawn = sharedDifficultySettings.WeightsByDifficultyList[3].BossPrefab; //Defaulting the Easy boss
            
            if (!dynamicDifficultyManager.useDDA)
            {
                StaticDifficultyType currentDifficulty = dynamicDifficultyManager.difficulty;
                ChangeBossRoomSetup(currentDifficulty);
                Debug.Log("Current Difficulty: " + currentDifficulty);
                
                switch (currentDifficulty)
                {
                    case StaticDifficultyType.Hard:
                    {
                        //Spawn Hard Boss
                        Debug.Log(DynamicDifficultyType.Hard);
                        BossToSpawn = sharedDifficultySettings.WeightsByDifficultyList[0].BossPrefab;
                        break;
                    }
                    case StaticDifficultyType.Medium:
                    {
                        if (dynamicDifficultyManager.Player.GetComponent<Health>().CurrentHealth >= dynamicDifficultyManager.Player.GetComponent<Health>().MaxHealth / 2f)
                        {
                            //If player health is greater than half of MaxHP then spawn MediumToHard Boss
                            Debug.Log(DynamicDifficultyType.MediumToHard);
                            BossToSpawn = sharedDifficultySettings.WeightsByDifficultyList[1].BossPrefab;
                        }
                        else
                        {
                            //If player health is lower than half of MaxHP then spawn EasyToMedium Boss
                            Debug.Log(DynamicDifficultyType.EasyToMedium);
                            BossToSpawn = sharedDifficultySettings.WeightsByDifficultyList[2].BossPrefab;
                        }
                        break;
                    }
                    case StaticDifficultyType.Easy:
                    {
                        //Spawn Easy Boss
                        Debug.Log(DynamicDifficultyType.Easy);
                        BossToSpawn = sharedDifficultySettings.WeightsByDifficultyList[3].BossPrefab;
                        break;
                    }
                    default:
                        break;
                }
            }
            else
            {
                float maxPlayerPerformance = 0;
                DynamicDifficultyType currentDifficulty = DynamicDifficultyType.Easy;
                
                foreach (KeyValuePair<DynamicDifficultyType, float> normalizedPlayerPerformanceItem in dynamicDifficultyManager.NormalizedPlayerPerformanceDictionary)
                {
                    Debug.Log("key-value pair: " + normalizedPlayerPerformanceItem.Key + " " + normalizedPlayerPerformanceItem.Value);
                    if (normalizedPlayerPerformanceItem.Value > maxPlayerPerformance)
                    {
                        maxPlayerPerformance = normalizedPlayerPerformanceItem.Value;
                        currentDifficulty = normalizedPlayerPerformanceItem.Key;
                    }
                }

                ChangeBossRoomSetup(currentDifficulty);
                
                int idx = (int) currentDifficulty;
                Debug.Log("maxPlayerPerformance: " + maxPlayerPerformance);
                Debug.Log("Current Difficulty: " + currentDifficulty);
                Debug.Log("Current Difficulty Index: " + idx);
                BossToSpawn = sharedDifficultySettings.WeightsByDifficultyList[idx].BossPrefab;
            }
            Debug.Log("Boss To Spawn: " + BossToSpawn);
            BossEnemy = Instantiate(BossToSpawn, position, Quaternion.identity);
        }
    }
}