using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

public class InteractionUISignIn : MonoBehaviour
{
    public GameObject waitingScreen; 
    private GameObject backToHomeBtn; 
    private GameObject nextForgotPassBtn; 
    private GameObject nextSignUpBtn; 
    void Start()
    {
        InitUI(); 
        SetActions();
    }
    void Update()
    {  
    }
    void InitUI()
    {
        waitingScreen.SetActive(false); 
        backToHomeBtn = GameObject.Find("BackBtn"); 
        nextForgotPassBtn = GameObject.Find("ForgotPass"); 
        nextSignUpBtn = GameObject.Find("SignUp"); 

    }
    void SetActions()
    {
        backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToHomeNoSignIn); 
        nextForgotPassBtn.GetComponent<Button>().onClick.AddListener(NextToForgotPass); 
        nextSignUpBtn.GetComponent<Button>().onClick.AddListener(NextToSignUp); 
    }

    void BackToHomeNoSignIn()
    {
        StartCoroutine(LoadAsynchronously(SceneConfig.home_nosignin));
    }
    void NextToForgotPass()
    {
        SceneManager.LoadScene(SceneConfig.forgotPass); 
    }
    void NextToSignUp()
    {
        SceneManager.LoadScene(SceneConfig.signUp); 
    }
    public IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName); 
        waitingScreen.SetActive(true);
        while(!operation.isDone)
        {
            yield return new WaitForSeconds(0f); 
        }
    }
}
