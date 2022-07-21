using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEditor;
using UnityEngine;

public class DownloadManager : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] private GameObject Player;
    [SerializeField] private DynamicDifficultyManager dynamicDifficultyManager;
    //[SerializeField] private GameObject DownloadBar;
    
    [Header("General")]
    [SerializeField] private float downloadRange = 30;
    [SerializeField] private float maxTime = 10;
    [SerializeField] private float progress = 0f;
    [SerializeField] private float playerDistance = 0f;
    [SerializeField] private float spawnTimer = 0f;
    [Tooltip("Spawn an enemy in every X second defined by spawnPeriod")]
    [SerializeField] private float spawnPeriod = 5f;

    private DownloadDataButton downloadDataButton;
    private float timeLeft;
    private EnemySpawner EnemySpawner;
    private bool downloadCompleted = false;

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
            
            CalculatePlayerDistance();
            if (playerDistance <= downloadRange)
            {
                if (/*timeLeft > 0*/timeLeft <= maxTime)
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

            if (playerDistance > downloadRange)
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

        }
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
    
    
    private void OnDrawGizmos()
    {
        // Draw DDA area around player. This range is used to calculate enemy count for adjusting difficulty
        Handles.color = Color.blue;
        Vector3 _centre = new Vector3(transform.position.x, transform.position.y ,
            transform.position.z);
        Handles.DrawWireDisc(_centre, Vector3.up, downloadRange);
    }
}
