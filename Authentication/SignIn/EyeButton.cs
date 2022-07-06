using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class EyeButton : MonoBehaviour
{
    public Button eyeButton;
    public InputField passwordField;
    public bool isOpen = true;
    void Start()
    {
        eyeButton = transform.GetComponent<Button>();
        eyeButton.onClick.AddListener(ChangeEyeShape);
    }
    void ChangeEyeShape()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            eyeButton.image.sprite = Resources.Load<Sprite>("Sprites/eyeclose");
            passwordField.contentType = InputField.ContentType.Standard;
            passwordField.ForceLabelUpdate();
        }
        else
        {
            eyeButton.image.sprite =  Resources.Load<Sprite>("Sprites/eyeopen");
            passwordField.contentType = InputField.ContentType.Password;
            passwordField.ForceLabelUpdate();
        }
    }
}
