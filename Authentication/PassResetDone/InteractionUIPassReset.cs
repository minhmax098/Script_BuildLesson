using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 

public class InteractionUIPassReset : MonoBehaviour
{
    public GameObject waitingScreen; 
    public GameObject nexttoSignInBtn; 
    public GameObject backtoResetPassBtn; 
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        
        InitUI(); 
        SetActions(); 
        waitingScreen.SetActive(false); 
    }
    void InitUI()
    {
        nexttoSignInBtn = GameObject.Find("SignIn"); 
        backtoResetPassBtn = GameObject.Find("BtnBackResetPass"); 
    }
    void SetActions()
    {
        nexttoSignInBtn.GetComponent<Button>().onClick.AddListener(NextToSignIn); 
        backtoResetPassBtn.GetComponent<Button>().onClick.AddListener(BackToResetPass); 
    }
    public IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName); 
        waitingScreen.SetActive(true); 
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(.1f); 
        }
    }
    public IEnumerator WaitForAPIResponse(UnityWebRequest request)
    {
        waitingScreen.SetActive(true); 
        Debug.Log("calling API"); 
        while(!request.isDone)
        {
            yield return new WaitForSeconds(.1f); 
        }
    }
    void NextToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.signIn); 
    }
    void BackToResetPass()
    {
        SceneManager.LoadScene(SceneConfig.resetpass); 
    }
}
