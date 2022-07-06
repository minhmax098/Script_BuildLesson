using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GenerateCapcha : MonoBehaviour
{
    public static Text capchaText; 
    public Button genCodeBtn; 
    private static string capchaString;
    void Start()
    {
        capchaText = GameObject.Find("TextCapcha").GetComponent<Text>(); 
        genCodeBtn = transform.GetComponent<Button>();
        genCapchaCode(6);
        genCodeBtn.onClick.AddListener(() => genCapchaCode(6));
    }
    public static void genCapchaCode(int length)
    {
        capchaString = VerificationCode.SetDeleKey(length);
        capchaText.text = capchaString;
        PlayerPrefs.SetString("capcha", capchaString);
    } 
    
}
