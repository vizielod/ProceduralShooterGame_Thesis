using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject StartButton;

        public bool _resetPlayerPrefs = false;
        // Start is called before the first frame update
        void Start()
        {
            if (_resetPlayerPrefs)
            {
                ResetPlayerPrefs();
            }
            bool wasTutorialPlayed = PlayerPrefs.GetInt("WasTutorialPlayed") == 0 ? false : true;
            StartButton.GetComponent<Button>().interactable = wasTutorialPlayed;
            /*if (!wasTutorialPlayed)
            {
                StartButton.GetComponent<Button>().interactable
            }*/
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ResetPlayerPrefs()
        {
            PlayerPrefs.SetInt("WasTutorialPlayed", 0);
            PlayerPrefs.SetInt("IsFirstTimePlaying", 1);
            PlayerPrefs.SetInt("difficulty", 0);
            PlayerPrefs.SetInt("useDDA", 0); //0 - false, 1 - true
            PlayerPrefs.SetInt("DDAUseWasSet", 0);
        }
    }
}