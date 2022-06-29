using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    void Start()
    {
        downloadDataButton = GetComponent<DownloadDataButton>();
        EnemySpawner = GetComponent<EnemySpawner>();
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (downloadDataButton.isOn)
        {
            CalculatePlayerDistance();
            if (playerDistance <= downloadRange)
            {
                if (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                    float downloadBarFillAmount = timeLeft / maxTime;
                    downloadDataButton.UpdateDownloadBar(downloadBarFillAmount);
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
                EnemySpawner.SpawnWeightedRandomEnemy(this.transform.position, downloadRange - 10f);
                spawnTimer = 0f;
            }

        }
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
