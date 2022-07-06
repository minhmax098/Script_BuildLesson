using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 
using System.Text;
using System.Text.RegularExpressions;

public class ForgotScript : MonoBehaviour
{
    public GameObject waitingScreen; 
    public Slider slider; 
    public Button nextBtn; 
    public InputField emailInput; 
    public GameObject EmailWarning; 
    public GameObject validateEmail; 
    public GameObject notificationBox; 
    private PopupSystem pop; 
    private Regex emailRgx = new Regex(@"^(([^<>()[\]\\.,;:\s@']+(\.[^<>()[\]\\.,;:\s@']+)*)|('.+'))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        
        nextBtn = transform.GetComponent<Button>(); 
        nextBtn.onClick.AddListener(ValidateForgotPass); 
        EmailWarning.SetActive(false); 
        validateEmail.SetActive(false); 
        notificationBox.SetActive(false);
        waitingScreen.SetActive(false);
        emailInput.keyboardType = TouchScreenKeyboardType.EmailAddress;
        emailInput.onValueChanged.AddListener(CheckEmailFormat);
        pop = notificationBox.GetComponent<PopupSystem>();
    }
    
    private void ValidateForgotPass()
    {
        string email = emailInput.text; 
        bool check = false;
        if (email == "")
        {
            ChangeUIStatus(emailInput, EmailWarning, true);
            validateEmail.SetActive(false); 
            check = true; 
            nextBtn.interactable = false; 
        }
        if (email != "")
        {
            EmailWarning.SetActive(false); 
        }
        if (!check && !EmailWarning.activeSelf)
        {
            StartCoroutine(CallForgotPass(email));
        }
    }
    private void CheckEmailFormat(string data)
    {
        if (data == "")
        {
            ChangeUIStatus(emailInput, EmailWarning, true);
            validateEmail.SetActive(false);
            // nextBtn.enabled = false;
            nextBtn.interactable = false; 
        }
        else if (!emailRgx.IsMatch(data))
        {
            Debug.Log("Email ko hop le !!!");
            ChangeUIStatus(emailInput, validateEmail, true);
            EmailWarning.SetActive(false);  
            // nextBtn.enabled = false;   
            nextBtn.interactable = false;
        }
        else 
        {
            ChangeUIStatus(emailInput, EmailWarning, false);
            validateEmail.SetActive(false);
            // nextBtn.enabled = true;
            nextBtn.interactable = true; 
        }
    }
    private void ChangeUIStatus(InputField input, GameObject warning, bool status)
    {
        warning.SetActive(status); 
        if(status)
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
        }
        else
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        }
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
      Debug.Log("calling API: "); 
      while(!request.isDone)
      {
        yield return new WaitForSeconds(.1f);
      }
    }
    // Forgot pass API 
    public IEnumerator CallForgotPass(string Email)
    {
        string forgotpassdataJsonString = "{\"email\": \"" + Email + "\" }";
        Debug.Log(forgotpassdataJsonString); 
        
        var request = new UnityWebRequest(APIUrlConfig.ForgotPass, "POST"); 
        byte[] bodyRaw = Encoding.UTF8.GetBytes(forgotpassdataJsonString); 
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw); 
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer(); 
        request.SetRequestHeader("Content-Type", "application/json"); 

        StartCoroutine(WaitForAPIResponse(request));
        yield return request.SendWebRequest(); 
        waitingScreen.SetActive(false); 
        if (request.error != null)
        {
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            UserDetail userDetail = JsonUtility.FromJson<UserDetail>(response);
            Debug.Log("Error: " + request.error);
            if (request.responseCode == 400)
            {
                // Gmail not found 
                pop.PopUp(userDetail.message);
            }
            else if (request.responseCode == 500){
                // Debug.Log("Server error !!");
                pop.PopUp(userDetail.message); 
            }
        }
        else
        {   
            Debug.Log("Status Code: " + request.responseCode);
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            UserDetail userDetail = JsonUtility.FromJson<UserDetail>(response);
            if( userDetail.code == 200)
            {
                PlayerPrefs.SetString("user_email_for_reset", Email);
                Debug.Log("secret code: "); 
                Debug.Log(userDetail.secret);
                // string token = PlayerPrefs.GetString("user_token");
                SceneManager.LoadScene(SceneConfig.resetpass);
            }
        }
    }
}   
