using System.Collections;
using System.Collections.Generic;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class SurveyButtonManager : MonoBehaviour
    {
        public GameObject SurveyPanel;
        
        public void OpenSurveyPopupWindow()
        {
            SurveyPanel.SetActive(true);
        }

        public void CloseSurveyPopupWindow()
        {
            SurveyPanel.SetActive(false);
        }
        public void OpenSurvey()
        {
            Application.OpenURL($"https://docs.google.com/forms/d/e/1FAIpQLSel3UWkBO2uG6V08bfXczNfC17dz17y0tUQ-G7PVoIyUAQgAA/viewform?&entry.2019003164={Telemetry.GUIDToShortString(Telemetry.playerID)}");
            https://docs.google.com/forms/d/e/1FAIpQLSel3UWkBO2uG6V08bfXczNfC17dz17y0tUQ-G7PVoIyUAQgAA/viewform?usp=sf_link
            //Application.OpenURL($"https://docs.google.com/forms/d/e/1FAIpQLSexfioAw0GMWuFcRcdXovB5emO7rI9aQoArBw9pvpoKNHAHrA/viewform?&entry.2082216164={Telemetry.GUIDToShortString(Telemetry.playerID)}");
            Quit();
        }
        
        public void Quit() {
#if UNITY_STANDALONE
            Application.Quit();
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
