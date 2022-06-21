using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadDataButton : Interactable
{
    public bool isOn;

    private void Start() {
        UpdateDownload();
    }

    public void UpdateDownload() {
        Debug.Log("Download Updated");
    }

    public override string GetDescription() {
        if (isOn) return "Press [E] to <color=red>STOP</color> downloading.";
        return "Press [E] to <color=green>START</color> downloading.";
    }

    public override void Interact() {
        isOn = !isOn;
        UpdateDownload();
    }
}
