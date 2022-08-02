using System.Collections;
using System.Collections.Generic;
using Unity.FPS;
using Unity.FPS.Game;
using UnityEditor;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class DownloadManager : MonoBehaviour
    {

        [Header("References")] [SerializeField]
        private GameObject Player;

        [SerializeField] private DynamicDifficultyManager dynamicDifficultyManager;
        //[SerializeField] private GameObject DownloadBar;

        [Header("General")] [SerializeField] private float downloadRange = 30;
        [SerializeField] private float maxTime = 10;
        [SerializeField] private float progress = 0f;
        [SerializeField] private float playerDistance = 0f;
        [SerializeField] private float spawnTimer = 0f;

        [Tooltip("Spawn an enemy in every X second defined by spawnPeriod")] [SerializeField]
        private float spawnPeriod = 5f;

        private DownloadDataButton downloadDataButton;
        private float timeLeft;
        private EnemySpawner EnemySpawner;
        private bool downloadCompleted = false;
        private bool isPlayerInRoom = false;

        private bool wasPlayerAccuracyReseted = false;

        // Start is called before the first frame update
        void Start()
        {
            downloadDataButton = GetComponent<DownloadDataButton>();
            EnemySpawner = GetComponent<EnemySpawner>();
            //timeLeft = maxTime;
            timeLeft = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (downloadDataButton.isOn && !downloadCompleted)
            {
                if (!wasPlayerAccuracyReseted)
                {
                    ResetPlayerAccuracy();
                    wasPlayerAccuracyReseted = true;
                }

                CheckIsPlayerInRoom();
                //CalculatePlayerDistance();
                if (/*playerDistance <= downloadRange*/isPlayerInRoom)
                {
                    if ( /*timeLeft > 0*/timeLeft <= maxTime)
                    {
                        //timeLeft -= Time.deltaTime;
                        timeLeft += Time.deltaTime;
                        float downloadBarFillAmount = timeLeft / maxTime;
                        downloadDataButton.UpdateDownloadBar(downloadBarFillAmount);
                    }
                    else
                    {
                        downloadCompleted = true;
                        downloadDataButton.isOn = false;
                        downloadDataButton.isCompleted = true;
                        downloadDataButton.UpdateDownload();

                        ExtractDataEvent evt = Events.ExtractDataEvent;
                        evt.Datapoint = this.gameObject;
                        //evt.RemainingEnemyCount = enemiesRemainingNotification;
                        EventManager.Broadcast(evt);
                    }
                }

                if (/*playerDistance > downloadRange*/!isPlayerInRoom)
                {
                    downloadDataButton.isPaused = true;
                    downloadDataButton.isOn = false;
                    downloadDataButton.UpdateDownload();
                }

                spawnTimer += Time.deltaTime;
                if (spawnTimer > spawnPeriod)
                {
                    //dynamicDifficultyManager.SpawnEnemy(this.transform.position, downloadRange - 10f);
                    EnemySpawner.SpawnEnemyByDynamicDifficulty(this.transform.position, downloadRange - 10f);
                    spawnTimer = 0f;
                }

                dynamicDifficultyManager.timeSpentInDownload += Time.deltaTime;
                CalculateAvaragePlayerPerformance();
            }
        }

        public void CalculateAvaragePlayerPerformance()
        {
            switch (EnemySpawner.currentDynamicDifficulty)
            {
                case DynamicDifficultyType.Hard:
                {
                    //Debug.Log(DynamicDifficultyType.Hard);
                    dynamicDifficultyManager.HardDifficultyGaugeTimer += Time.deltaTime;
                    /*dynamicDifficultyManager.NormalizedHardDifficultyGaugeTimer =
                        timeSpentInDownload / dynamicDifficultyManager.HardDifficultyGaugeTimer;*/
                    break;
                }
                case DynamicDifficultyType.MediumToHard:
                {
                    //Debug.Log(DynamicDifficultyType.EasyToMedium);
                    dynamicDifficultyManager.MediumToHardDifficultyGaugeTimer += Time.deltaTime;
                    /*dynamicDifficultyManager.NormalizedMediumToHardDifficultyGaugeTimer =
                        timeSpentInDownload / dynamicDifficultyManager.MediumToHardDifficultyGaugeTimer;*/
                    break;
                }
                case DynamicDifficultyType.EasyToMedium:
                {
                    //Debug.Log(DynamicDifficultyType.EasyToMedium);
                    dynamicDifficultyManager.EasyToMediumDifficultyGaugeTimer += Time.deltaTime;
                    /*dynamicDifficultyManager.NormalizedEasyToMediumDifficultyGaugeTimer =
                        timeSpentInDownload / dynamicDifficultyManager.EasyToMediumDifficultyGaugeTimer;*/
                    break;
                }
                case DynamicDifficultyType.Easy:
                {
                    //Debug.Log(DynamicDifficultyType.Easy);
                    dynamicDifficultyManager.EasyDifficultyGaugeTimer += Time.deltaTime;
                    /*dynamicDifficultyManager.NormalizedEasyDifficultyGaugeTimer =
                        timeSpentInDownload / dynamicDifficultyManager.EasyDifficultyGaugeTimer;*/
                    break;
                }
                default:
                    break;
            }



            /*if (currentDifficultyGauge <= EnemySpawner.DynamicDifficultySettings.DifficultyTypeByGaugeValueList[j].MaxDifficultyGauge)
            {
                
            }*/
        }

        private void ResetPlayerAccuracy()
        {
            dynamicDifficultyManager.SetPlayerAccuracy(0.5f);
            dynamicDifficultyManager.playerAccuracyScaledForEstimatedDifficulty = 0.5f;
        }

        private void CalculatePlayerDistance()
        {
            playerDistance = Vector3.Distance(this.transform.position, Player.transform.position);
        }

        private void CheckIsPlayerInRoom()
        {
            float playerDistance_x = Mathf.Abs(Player.transform.position.x - transform.position.x);
            float playerDistance_z = Mathf.Abs(Player.transform.position.z - transform.position.z);

            if (playerDistance_x <= 35.5f && playerDistance_z<= 35.5f)
            {
                Debug.Log("Player is in Computer Room");
                isPlayerInRoom = true;
                //return true;
            }
            else
            {
                Debug.Log("Player LEFT Computer Room");
                isPlayerInRoom = false;
                //return false;
            }
        }


        private void OnDrawGizmos()
        {
            // Draw DDA area around player. This range is used to calculate enemy count for adjusting difficulty
            Handles.color = Color.blue;
            Vector3 _centre = new Vector3(transform.position.x, transform.position.y,
                transform.position.z);
            Handles.DrawWireDisc(_centre, Vector3.up, downloadRange);
        }
    }
}
