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

        public void LoadTargetScene()
        {
            int rand = Random.Range(0, 2);

            SceneManager.LoadScene(SceneName);
            PlayerPrefs.SetInt("difficulty", difficultyIDX);
            PlayerPrefs.SetInt("useDDA", rand); //0 - false, 1 - true
            
            /*if (rand == 0)
            {
                //SceneManager.LoadScene(SceneName);
                PlayerPrefs.SetInt("difficulty", difficultyIDX);
                PlayerPrefs.SetInt("useDDA", 0); //0 - false, 1 - true
            }
            else
            {
                PlayerPrefs.SetInt("useDDA", 1);
            }
            SceneManager.LoadScene(SceneName);*/
        }
    }
}