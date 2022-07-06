using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ClearInput : MonoBehaviour
{
    public GameObject xButton; 
    public InputField inputField;
    void Start()
    {
        xButton.SetActive(false);
        xButton.transform.GetComponent<Button>().onClick.AddListener(clearInput); 
        inputField.onValueChanged.AddListener(HandleInput);
    }
    void clearInput()
    {
        inputField.text = "";
        xButton.SetActive(false);
    }

    void HandleInput(string data){
        if (data == "")
        {
            xButton.SetActive(false);
        }
        else
        {
            xButton.SetActive(true);
        }
    }
}
