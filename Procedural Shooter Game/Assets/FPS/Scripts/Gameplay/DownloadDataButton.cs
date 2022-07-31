using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.Gameplay
{
    public class DownloadDataButton : Interactable
    {
        [SerializeField] private GameObject DownloadBar;
        [SerializeField] private Image fillImageDownload;
        [SerializeField] private GameObject downloadZoneVisualizer;
        public bool isDownloadBarActive = false;
        public bool isOn = false;
        public bool isPaused = false;
        public bool isCompleted = false;

        private void Start()
        {
            //isDownloadBarActive = false;
            //DownloadBar.SetActive(isOn);
            //DownloadBar.SetActive(true);
            UpdateDownload();
        }

        public void UpdateDownload()
        {
            DownloadBar.SetActive(isOn);
            downloadZoneVisualizer.SetActive(isOn);
            //Debug.Log("Download Updated");
        }

        public override string GetDescription()
        {
            if (isOn) return "Press [E] to <color=red>STOP</color> downloading.";
            if (isPaused) return "Press [E] to <color=green>CONTINUE</color> downloading.";
            if (isCompleted) return "Dara already Downloaded from this Computer";
            return "Press [E] to <color=green>START</color> downloading.";
        }

        public override void Interact()
        {
            if (!isCompleted)
            {
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

                UpdateDownload();
            }
        }

        public void UpdateDownloadBar(float fillAmount)
        {
            fillImageDownload.fillAmount = fillAmount;
        }
    }
}
