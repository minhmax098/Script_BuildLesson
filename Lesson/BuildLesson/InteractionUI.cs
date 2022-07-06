using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; 

namespace BuildLesson
{
    public class InteractionUI : MonoBehaviour
    {
        private static InteractionUI instance; 
        public static InteractionUI Instance
        {
            get
            {
                if (instance = null)
                {
                    instance = FindObjectOfType<InteractionUI>(); 
                }
                return instance;
            }
        }
        
        public Button btnShowExitPopup; 
        public GameObject popupExit; 
        public GameObject btnExitPopup; 
        public GameObject btnExitLesson; 
        public GameObject btnCancelExitLesson; 
        
        void Start()
        {
            SetActions(); 
            InitLayoutScreen();
        }

        void InitLayoutScreen()
        {
            Screen.orientation = ScreenOrientation.Landscape;
            StatusBarManager.statusBarState = StatusBarManager.States.Hidden;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        }

        void SetActions()
        {
            btnShowExitPopup.GetComponent<Button>().onClick.AddListener(HandlerShowExitPopup);
            btnExitPopup.GetComponent<Button>().onClick.AddListener(HandlerExitPopup);
            btnExitLesson.GetComponent<Button>().onClick.AddListener(HandlerExitLesson);
            btnCancelExitLesson.GetComponent<Button>().onClick.AddListener(HandlerCancelExitLesson);
        }

        void HandlerShowExitPopup()
        {
            popupExit.SetActive(true);
        }

        void HandlerExitPopup()
        {
            popupExit.SetActive(false);
        }

        void HandlerExitLesson()
        {
            if (PlayerPrefs.GetString("user_email") != "")
            {
                SceneManager.LoadScene(SceneConfig.lesson_edit);
            }
        }
        
        void HandlerCancelExitLesson()
        {
            popupExit.SetActive(false);
        }
    }
}
