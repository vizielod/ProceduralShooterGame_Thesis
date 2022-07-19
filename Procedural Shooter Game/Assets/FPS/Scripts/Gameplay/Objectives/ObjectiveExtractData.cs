using System;
using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveExtractData : Objective
    {
        [Tooltip("Chose whether you need to kill every enemies or only a minimum amount")]
        public bool MustExtractAllData = true;

        [Tooltip("If MustExtractAllData is false, this is the amount of data extract required")]
        public int DatapointsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining Datapoints when this amount of Datapoints is left")]
        public int NotificationDatapointsRemainingThreshold = 3;

        public List<GameObject> CentralComputers = new List<GameObject>();
        public GameObject ObjectiveReachPoint;
        public GameObject CentralComputer;

        int m_DatapointsExtracted = 0;

        protected override void Start()
        {
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

        void OnDataExtracted(ExtractDataEvent evt)
        {
            if (IsCompleted)
                return;
            
            m_DatapointsExtracted++;
            
            /*if (MustExtractAllData)
                DatapointsToCompleteObjective = evt.RemainingDatapoints + m_DatapointsExtracted;*/
            
            //int targetRemaining = MustExtractAllData ? evt.RemainingDatapoints : DatapointsToCompleteObjective - m_DatapointsExtracted;
            int targetRemaining = DatapointsToCompleteObjective - m_DatapointsExtracted;
            
            // update the objective text according to how many enemies remain to kill
            if (targetRemaining == 0)
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
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
    }
}