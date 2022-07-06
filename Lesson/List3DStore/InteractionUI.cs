using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace List3DStore
{
    public class InteractionUI : MonoBehaviour
    {
        // public GameObject waitingScreen; 
        private GameObject backToCreateLessonBtn; 
        private static InteractionUI instance; 
        public static InteractionUI Instance
        { 
            get 
            { 
                if (instance == null)
                {
                    instance = FindObjectOfType<InteractionUI>();
                }
                return instance;
            }
        }
        void Start()
        {
            InitUI();
            SetActions();
        }
        void InitUI()
        {
            backToCreateLessonBtn = GameObject.Find("BackBtn"); 
        }
        void SetActions()
        {
            backToCreateLessonBtn.GetComponent<Button>().onClick.AddListener(BackToCreateLesson); 
        }
        void BackToCreateLesson()
        {
            StartCoroutine(LoadAsynchronously(SceneConfig.createLesson_main));
        }
        public void onClickItemLesson(int modelId)
        {
            Debug.Log("On click item lesson: ");
            ModelStoreManager.InitModelStore(modelId);
            SceneNameManager.setPrevScene(SceneConfig.storeModel); 
            if (PlayerPrefs.GetString("user_email") != "")
            {
                Debug.Log("Go to here: ");
                StartCoroutine(LoadAsynchronously(SceneConfig.createLesson)); 
            }
        }

        public IEnumerator LoadAsynchronously(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            // waitingScreen.SetActive(true);
            while(!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
