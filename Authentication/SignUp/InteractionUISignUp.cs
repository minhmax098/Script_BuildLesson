using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUISignUp : MonoBehaviour
{
    public GameObject waitingScreen; 
    public GameObject switchToHomeNoSignIn;
    public GameObject switchToSignInBtn; 
    void Start()
    {
        InitUI(); 
        SetActions(); 
    }
    void InitUI()
    {
        waitingScreen.SetActive(false); 
        switchToHomeNoSignIn = GameObject.Find("BackBtn"); 
        switchToSignInBtn = GameObject.Find("SignIn"); 
    }   
    void SetActions()
    {
        waitingScreen.SetActive(false); 
        switchToHomeNoSignIn.GetComponent<Button>().onClick.AddListener(SwitchToHomeNoSignIn); 
        switchToSignInBtn.GetComponent<Button>().onClick.AddListener(SwitchToSignIn); 
    }
    void SwitchToHomeNoSignIn()
    {
        StartCoroutine(LoadAsynchronously(SceneConfig.home_nosignin));  
    }
    void SwitchToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.signIn); 
    }
    public IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName); 
        waitingScreen.SetActive(true); 
        while(!operation.isDone)
        {
            yield return new WaitForSeconds(3f); 
        }
    }
}
