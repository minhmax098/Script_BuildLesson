using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

public class InteractionUI : MonoBehaviour
{
    public GameObject waitingScreen; 
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
        
    }
  
    public void onClickItemLesson(int lessonId)
    {
        LessonManager.InitLesson(lessonId);
        SceneNameManager.setPrevScene(SceneConfig.myLesson); 
        if (PlayerPrefs.GetString("user_email") != "")
        {
            StartCoroutine(LoadAsynchronously(SceneConfig.lesson_edit)); 
        }
    }
    
    public IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        waitingScreen.SetActive(true);
        while(!operation.isDone)
        {
            yield return null;
        }
    }
}
