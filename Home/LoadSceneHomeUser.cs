using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class LoadSceneHomeUser : MonoBehaviour
{
    public Text username; 
    public Text email; 
    void Start()
    {
        username.text = PlayerPrefs.GetString("user_name"); 
        email.text = PlayerPrefs.GetString("user_email"); 
        Debug.Log(PlayerPrefs.GetString("user_name")); 
        Debug.Log(PlayerPrefs.GetString("user_email")); 
    }
}

