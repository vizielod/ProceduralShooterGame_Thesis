using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DownloadManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private float downloadRange = 30;
    [SerializeField] private float progress = 0f;
    [SerializeField] private float playerDistance = 0f;
    
    private DownloadDataButton downloadDataButton;
    
    // Start is called before the first frame update
    void Start()
    {
        downloadDataButton = GetComponent<DownloadDataButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (downloadDataButton.isOn)
        {
            CalculatePlayerDistance();
            if (playerDistance <= downloadRange)
            {
                if (progress < 100)
                {
                    progress++;
                }
                else
                {
                    progress = 0f; //test
                }
            }

            if (playerDistance > downloadRange)
            {
                downloadDataButton.isOn = !downloadDataButton.isOn;
                downloadDataButton.UpdateDownload();
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
