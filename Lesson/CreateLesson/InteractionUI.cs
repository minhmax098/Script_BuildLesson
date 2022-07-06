using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace CreateLesson
{
    public class InteractionUI : MonoBehaviour
    {
        private GameObject backTo3DStore;
        private GameObject cancelBtn;
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
            backTo3DStore = GameObject.Find("BackBtn"); 
            cancelBtn = GameObject.Find("CancelBtn");
        }
        void SetActions()
        {
            backTo3DStore.GetComponent<Button>().onClick.AddListener(BackTo3DStore);
            cancelBtn.GetComponent<Button>().onClick.AddListener(CancelButtonToBack3DStore);
        }
        void BackTo3DStore()
        {
            SceneManager.LoadScene(SceneConfig.storeModel); 
        }
        void CancelButtonToBack3DStore()
        {
            SceneManager.LoadScene(SceneConfig.storeModel); 
        }
    }
}
