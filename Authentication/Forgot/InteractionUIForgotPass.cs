using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUIForgotPass : MonoBehaviour
{
    public GameObject backtoSignInBtn; 
    void Start()
    {
        InitUI(); 
        SetActions(); 
    }
    void InitUI()
    {
        backtoSignInBtn = GameObject.Find("BackBtn");  
    }
    void SetActions()
    {
        backtoSignInBtn.GetComponent<Button>().onClick.AddListener(BackToSignIn); 
    }
    void BackToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.signIn); 
    }
}
