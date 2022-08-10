using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Unity.FPS.UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        public string SceneName = "";
        public int difficultyIDX = 0;

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject
                && Input.GetButtonDown(GameConstants.k_ButtonNameSubmit))
            {
                LoadTargetScene();
            }
        }

        public void SimpleLoadTargetScene()
        {
            SceneManager.LoadScene(SceneName);
        }

        public void LoadTargetScene()
        {
            bool useDDA = PlayerPrefs.GetInt("useDDA") == 0 ? false : true;
            bool DDAUseWasSet = PlayerPrefs.GetInt("DDAUseWasSet") == 0 ? false : true;
            bool IsFirstTimePlaying = PlayerPrefs.GetInt("IsFirstTimePlaying") == 0 ? false : true;
            
            int rand = Random.Range(0, 2);

            if (IsFirstTimePlaying)
            {
                PlayerPrefs.SetInt("IsFirstTimePlaying", 0);
                PlayerPrefs.SetInt("difficulty", difficultyIDX);
                PlayerPrefs.SetInt("useDDA", rand); //0 - false, 1 - true
                if (rand == 1) //Means DDA should be used
                {
                    PlayerPrefs.SetInt("DDAUseWasSet", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("DDAUseWasSet", 0);
                }
            }
            else
            {
                if (DDAUseWasSet)
                {
                    PlayerPrefs.SetInt("useDDA", 1);
                    PlayerPrefs.SetInt("difficulty", difficultyIDX);
                }
                else
                {
                    PlayerPrefs.SetInt("useDDA", 0);
                    PlayerPrefs.SetInt("difficulty", difficultyIDX);
                }
                
            }

            SceneManager.LoadScene(SceneName);
            
        }

        public void LoadTutorialScene()
        {
            SceneManager.LoadScene(SceneName);
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