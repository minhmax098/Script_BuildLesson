using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 
using System.Text;

public class SigninScript : MonoBehaviour
{
    public GameObject loadingScreen; 
    public GameObject waitingScreen;
    public Slider slider; 
    public Button signInBtn; 
    public InputField userNameInput; 
    public InputField passwordInput; 
    public GameObject EmailWarning; 
    public GameObject PassWarning; 
    public GameObject headerCapcha; 
    public GameObject capcha; 
    public GameObject CapchaWarning; 
    public GameObject incorrectCapcha;
    private int invalidCount; 
    public GameObject notificationBox; 
    private PopupSystem pop; 
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        
        signInBtn = transform.GetComponent<Button>();
        signInBtn.onClick.AddListener(ValidateSignin);

        EmailWarning.SetActive(false); 
        PassWarning.SetActive(false); 
        capcha.SetActive(false);  

        loadingScreen.SetActive(false); 
        waitingScreen.SetActive(false); 

        userNameInput.keyboardType = TouchScreenKeyboardType.EmailAddress;
        userNameInput.onValueChanged.AddListener(checkUserNameValid);
        
        passwordInput.contentType = InputField.ContentType.Password;
        passwordInput.onValueChanged.AddListener(checkPassValid);

        headerCapcha.SetActive(false);
        CapchaWarning.SetActive(false); 
        incorrectCapcha.SetActive(false); 

        notificationBox.SetActive(false); 
        pop = notificationBox.GetComponent<PopupSystem>();
    }
    
    void checkUserNameValid(string data)
    {
        if(data != "")
        {
            changeUIStatus(userNameInput, EmailWarning, false);
        }
        // else
        // {
        //     changeUIStatus(userNameInput, EmailWarning, true);
        // }
    }

    void checkPassValid(string data)
    {
        if(data != "")
        { 
            changeUIStatus(passwordInput, PassWarning, false);
        }
    }

    private void ValidateSignin()
    {
        Debug.Log("Number of invalid count: " + invalidCount);
        string email = userNameInput.text; 
        string pass = passwordInput.text; 
        bool check = false;
        if (email == "")
        {
            changeUIStatus(userNameInput, EmailWarning, true);
            check = true;
        }
        if (pass == "")
        {
            changeUIStatus(passwordInput, PassWarning, true);
            check = true;
        }
        if (invalidCount >= 5)
        {
            string capchaString = capcha.transform.GetChild(0).GetComponent<InputField>().text;
            Debug.Log(capchaString);
            if (capchaString == "")
            {
                changeUIStatus(capcha.transform.GetChild(0).gameObject.GetComponent<InputField>(), CapchaWarning, true);
                incorrectCapcha.SetActive(false);
                check = true;
            }
            if (capchaString != "")
            {
                changeUIStatus(capcha.transform.GetChild(0).gameObject.GetComponent<InputField>(), CapchaWarning, false); 
                // CapchaWarning.SetActive(false);
                if (!check)
                {
                    Debug.Log("test capcha comparison: ");
                    Debug.Log(capchaString);
                    Debug.Log(PlayerPrefs.GetString("capcha"));
                    CapchaWarning.SetActive(false); 
                    if (capchaString.Equals(PlayerPrefs.GetString("capcha")))
                    {                    
                        // incorrectCapcha.SetActive(false);
                        // capcha.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
                        changeUIStatus(capcha.transform.GetChild(0).gameObject.GetComponent<InputField>(), incorrectCapcha, false);
                        StartCoroutine(CallSignin(email, pass));
                    }
                    else
                    {
                        Debug.Log("capcha");
                        incorrectCapcha.SetActive(true);
                        GenerateCapcha.genCapchaCode(6);
                        CapchaWarning.SetActive(false);
                    }
                }
            }
        }
        if (!check && invalidCount < 5) 
        {
            StartCoroutine(CallSignin(email, pass));
        }
    }
    private void changeUIStatus(InputField input, GameObject warning, bool status)
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
        while(!operation.isDone)
        {
            yield return new WaitForSeconds(.0f);
        }
    }
    public IEnumerator WaitForAPIResponse(UnityWebRequest request)
    {
        waitingScreen.SetActive(true);
        Debug.Log("calling API");
        while (!request.isDone)
        {
            yield return new WaitForSeconds(.0f);
        }
    }
    // Signin API
    public IEnumerator CallSignin(string Email, string Password)
    {
        string logindataJsonString = "{\"email\": \"" + Email + "\", \"password\": \"" + Password + "\"}";
        Debug.Log(logindataJsonString);
        var request = new UnityWebRequest(APIUrlConfig.SignIn, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(logindataJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        StartCoroutine(WaitForAPIResponse(request));
        yield return request.SendWebRequest();
        waitingScreen.SetActive(false); 
        if (request.error != null)
        {
            Debug.Log("Error: " + request.error);
            if (request.responseCode == 400)
            {
                // no found user, show message 
                pop.PopUp("Username or password incorrect");
                invalidCount += 1; 
                if (invalidCount == 5)
                {
                    headerCapcha.SetActive(true);
                    capcha.SetActive(true); 
                }
            }
        }
        else
        {   
            Debug.Log("Status Code: " + request.responseCode);
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("Login response: " + response);
            LoginData userDetail = JsonUtility.FromJson<LoginData>(response);
            if( request.responseCode == 200)
            {
                Debug.Log("Set email: ");
                PlayerPrefs.SetString("user_email", userDetail.data[0].email); 
                PlayerPrefs.SetString("user_name", "Nguyen Thi Huong Giang"); 
                PlayerPrefs.SetString("user_token", userDetail.data[0].token);
                // string token = PlayerPrefs.GetString("user_token");
                StartCoroutine(LoadAsynchronously(SceneConfig.home_user));
            }
        }
    }
}
