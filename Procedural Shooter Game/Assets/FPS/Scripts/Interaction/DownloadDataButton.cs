using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadDataButton : Interactable
{
    [SerializeField] private GameObject DownloadBar;
    [SerializeField] private Image fillImageDownload;
    [SerializeField] private GameObject downloadZoneVisualizer;
    public bool isDownloadBarActive = false;
    public bool isOn = false;
    public bool isPaused = false;

    private void Start()
    {
        //isDownloadBarActive = false;
        //DownloadBar.SetActive(isOn);
        //DownloadBar.SetActive(true);
        UpdateDownload();
    }

    public void UpdateDownload() {
        DownloadBar.SetActive(isOn);
        downloadZoneVisualizer.SetActive(isOn);
        Debug.Log("Download Updated");
    }

    public override string GetDescription() {
        if (isOn) return "Press [E] to <color=red>STOP</color> downloading.";
        if (isPaused) return "Press [E] to <color=green>CONTINUE</color> downloading.";
        return "Press [E] to <color=green>START</color> downloading.";
    }

    public override void Interact() {
        if (!isOn)
        {
            isOn = true;
            isPaused = false;
        }
        else
        {
            isPaused = true;
            isOn = false;
        }
        //isOn = !isOn;
        UpdateDownload();
    }

    public void UpdateDownloadBar(float fillAmount)
    {
        fillImageDownload.fillAmount = fillAmount;
    }
}
