using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking; 
using System.Text;
using System.Text.RegularExpressions;

public class SignupScript : MonoBehaviour
{
   public GameObject waitingScreen; 
   public Slider slider; 
   public Button signUpBtn;
   public InputField emailInput; 
   public InputField fullNameInput;
   public InputField passwordInput;
   public InputField confirmPasswordInput;
   public GameObject EmailWarning; 
   public GameObject validateEmail; 
   public GameObject FullnameWarning; 
   public GameObject PasswordWarning; 
   public GameObject PasswordWarning2; 
   public GameObject ConfirmPasswordWarning;
   public GameObject ConfirmPasswordWarning2;
   public GameObject notificationBox; 
   private PopupSystem pop; 
   private Regex emailRgx = new Regex(@"^(([^<>()[\]\\.,;:\s@']+(\.[^<>()[\]\\.,;:\s@']+)*)|('.+'))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
   private Regex passRgx = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$");

   void Start()
   {
      Screen.orientation = ScreenOrientation.Portrait; 
      StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
      StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
      
      signUpBtn = transform.GetComponent<Button>();
      signUpBtn.onClick.AddListener(ValidateSignup);

      EmailWarning.SetActive(false); 
      FullnameWarning.SetActive(false); 
      PasswordWarning.SetActive(false); 
      ConfirmPasswordWarning.SetActive(false);  
      PasswordWarning2.SetActive(false); 
      ConfirmPasswordWarning2.SetActive(false);  
      validateEmail.SetActive(false);

      waitingScreen.SetActive(false);

      emailInput.keyboardType = TouchScreenKeyboardType.EmailAddress;
      
      emailInput.onValueChanged.AddListener(CheckEmailFormat);
   
      fullNameInput.onValueChanged.AddListener(CheckFullnameFormat);
   
      notificationBox.SetActive(false); 
      pop = notificationBox.GetComponent<PopupSystem>();

      passwordInput.onValueChanged.AddListener(CheckPasswordStrength);
      passwordInput.contentType = InputField.ContentType.Password;
      passwordInput.characterLimit = 20;
      
      confirmPasswordInput.onValueChanged.AddListener(CheckPasswordMatch);
      confirmPasswordInput.contentType = InputField.ContentType.Password; 
      confirmPasswordInput.characterLimit = 20; 
   } 
   private void CheckFullnameFormat(string data)
   {
      if (data == "")
      {
         changeUIStatus(fullNameInput, FullnameWarning, true);
      }
      else 
      {
         changeUIStatus(fullNameInput, FullnameWarning, false);
      }
   }
   private void CheckPasswordMatch(string data)
   {
      Debug.Log(passwordInput.text);
      if (data == "")
      {
         changeUIStatus(confirmPasswordInput, ConfirmPasswordWarning, false);
         ConfirmPasswordWarning2.SetActive(false);
      }
      else if (!data.Equals(passwordInput.text))
      {
         changeUIStatus(confirmPasswordInput, ConfirmPasswordWarning2, true);
         ConfirmPasswordWarning.SetActive(false);  
      }
      else 
      {
         changeUIStatus(confirmPasswordInput, ConfirmPasswordWarning2, false);
         ConfirmPasswordWarning.SetActive(false);
      }
   }
   private void CheckPasswordStrength(string data)
   {
      if (data == "")
      {
         changeUIStatus(passwordInput, PasswordWarning, false);
         changeUIStatus(passwordInput, PasswordWarning2, false); 
      }
      else if (!passRgx.IsMatch(data)){
         PasswordWarning.SetActive(false);
         PasswordWarning2.SetActive(true);
         changeUIStatus(passwordInput, PasswordWarning, false);
         changeUIStatus(passwordInput, PasswordWarning2, true); 
      }
      else 
      {
         PasswordWarning.SetActive(false);
         PasswordWarning2.SetActive(false);
         changeUIStatus(passwordInput, PasswordWarning, false);
         changeUIStatus(passwordInput, PasswordWarning2, false); 
      }
   }
   private void ValidateSignup()
   {
      string email = emailInput.text; 
      string fullName = fullNameInput.text; 
      string password = passwordInput.text; 
      string confirmPassword = confirmPasswordInput.text; 
      bool check = false; 
      if (email == "")
      {
         validateEmail.SetActive(false);
         changeUIStatus(emailInput, EmailWarning, true); 
         check = true; 
      }
      if (fullName == "")
      {
         changeUIStatus(fullNameInput, FullnameWarning, true);
         check = true;  
      }
      if (password == "")
      {
         PasswordWarning2.SetActive(false);
         changeUIStatus(passwordInput, PasswordWarning, true);
         check = true; 
      }
      if(confirmPassword == "")
      {
         ConfirmPasswordWarning2.SetActive(false);
         changeUIStatus(confirmPasswordInput, ConfirmPasswordWarning, true);
         check = true; 
      }
      
      if (email != "")
      {
         EmailWarning.SetActive(false);
      }
      if (fullName != "")
      {
         FullnameWarning.SetActive(false);
      }
      if (password != "")
      {
         PasswordWarning.SetActive(false);
      }
      if (confirmPassword != "")
      {
         ConfirmPasswordWarning.SetActive(false);
      }

      if (!check && !validateEmail.activeSelf && !PasswordWarning2.activeSelf && !ConfirmPasswordWarning2.activeSelf) 
      {
         StartCoroutine(CallSignup(email, fullName, password, confirmPassword));
      }
   }
   private void CheckEmailFormat(string data)
   {
      if (data == "")
      {
         changeUIStatus(emailInput, validateEmail, false);
         EmailWarning.SetActive(false);
      }
      else if (!emailRgx.IsMatch(data))
      {
         changeUIStatus(emailInput, validateEmail, true);
         EmailWarning.SetActive(false);
      }
      else
      {
         changeUIStatus(emailInput, validateEmail, false);
         EmailWarning.SetActive(false);
      }
   }
   private void changeUIStatus(InputField input, GameObject warning, bool status)
   {
      warning.SetActive(status);
      if (status)
      {
         input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning); 
      }
      else
      {
         input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
      }
   }
   public IEnumerator LoadAsynchronously (string sceneName)
   {
      AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
      waitingScreen.SetActive(true);
      while (!operation.isDone)
      {
         yield return new WaitForSeconds(.2f); 
      }
   }
   public IEnumerator WaitForAPIResponse(UnityWebRequest request)
   {
      waitingScreen.SetActive(true); 
      Debug.Log("calling API: "); 
      while(!request.isDone)
      {
         yield return new WaitForSeconds(.2f);
      }
   }
   // Signup API
   public IEnumerator CallSignup(string Email, string Fullname, string Password, string ConfirmPassword)
   {
        string signupdataJsonString = "{\"email\": \"" + Email + "\", \"fullName\": \"" + Fullname + "\",\"password\": \"" + Password + "\",\"confirmPassword\": \"" + ConfirmPassword + "\" }";
        Debug.Log(signupdataJsonString);
        var request = new UnityWebRequest(APIUrlConfig.SignUp, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(signupdataJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        StartCoroutine(WaitForAPIResponse(request));
        yield return request.SendWebRequest(); 
        waitingScreen.SetActive(false); 
        if (request.error != null)
        {
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            UserDetail userDetail = JsonUtility.FromJson<UserDetail>(response);
            // != 200
            Debug.Log("Error: " + request.error);
            if (request.responseCode == 400)
            {
               pop.PopUp(userDetail.message);
            }
        }
        else
        {
           // code 200
            Debug.Log("All OK");
            Debug.Log("Status Code: " + request.responseCode);
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data); 
            UserDetail userDetail = JsonUtility.FromJson<UserDetail>(response); 
            SceneManager.LoadScene(SceneConfig.signIn); 
        }
   }
}