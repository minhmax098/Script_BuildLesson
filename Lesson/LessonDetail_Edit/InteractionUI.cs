using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace LessonDetail_Edit 
{
    public class InteractionUI : MonoBehaviour
    {
        private GameObject startLessonBtn;
        private GameObject startMeetingBtn;
        private GameObject backToHomeBtn;
        private GameObject buildLessonBtn;
        private GameObject lessonObjectivesBtn; 
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
            backToHomeBtn = GameObject.Find("BackBtn");
            startLessonBtn = GameObject.Find("StartLessonBtn");
            if (PlayerPrefs.GetString("user_email") != "") 
            {
                startMeetingBtn = GameObject.Find("StartMeetingBtn"); 
            }
            buildLessonBtn = GameObject.Find("BtnBuildLesson");
            lessonObjectivesBtn = GameObject.Find("BtnLessonObjectives");
        }
        void SetActions()
        {
            backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToMyLesson);
            startLessonBtn.GetComponent<Button>().onClick.AddListener(StartExperience);
            if (PlayerPrefs.GetString("user_email") != "")
            {
                startMeetingBtn.GetComponent<Button>().onClick.AddListener(StartMeeting);
            }
            buildLessonBtn.GetComponent<Button>().onClick.AddListener(BuildLesson);
            lessonObjectivesBtn.GetComponent<Button>().onClick.AddListener(UpdateLessonObjectives);
        }
        void BackToMyLesson()
        {
            if (PlayerPrefs.GetString("user_email") != "")
            {
                SceneManager.LoadScene(SceneConfig.myLesson);
            }
        }
        void StartExperience()
        {
            SceneNameManager.setPrevScene(SceneConfig.myLesson); 
            SceneManager.LoadScene(SceneConfig.experience); 
        }
        void StartMeeting()
        {
            SceneManager.LoadScene(SceneConfig.meetingStarting); 
        }
        void BuildLesson()
        {
            SceneManager.LoadScene(SceneConfig.buildLesson);
        }
        void UpdateLessonObjectives()
        {
            SceneManager.LoadScene(SceneConfig.update_lesson);
        }
    }
}
