using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUIResetPass : MonoBehaviour
{
    public GameObject backtoForgotBtn; 
    void Start()
    {
        InitUI(); 
        SetActions(); 
    }
    void InitUI()
    {
        backtoForgotBtn = GameObject.Find("BackBtn");  
    }
    void SetActions()
    {
        backtoForgotBtn.GetComponent<Button>().onClick.AddListener(BackToSignIn); 
    }
    void BackToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.forgotPass); 
    }
}
