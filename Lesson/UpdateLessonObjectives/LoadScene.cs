using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace UpdateLessonObjectives
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject spinner;
        public LessonDetail[] myData;
        public LessonDetail currentLesson; 
        public GameObject bodyObject; 
        public Button updateBtn;
        private ListOrgans listOrgans;
        public GameObject dropdownObj; 
        private Dropdown dropdown;
        private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            
            updateBtn.onClick.AddListener(UpdateLessonObjective);
            myData = LoadData.Instance.GetLessonByID(LessonManager.lessonId.ToString()).data;
            // currentLesson = Array.Find(myData, lesson => lesson.lessonId == LessonManager.lessonId);
            currentLesson = myData[0];
            StartCoroutine(LoadCurrentLesson(currentLesson));
            spinner.SetActive(false);
            dropdown = dropdownObj.GetComponent<Dropdown>();
            updateDropDown();
        }

        IEnumerator LoadCurrentLesson(LessonDetail currentLesson)
        {
            string imageUri = String.Format(APIUrlConfig.LoadLesson, currentLesson.lessonThumbnail);
            bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonTitle; 
            bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonObjectives; 
            ModelStoreManager.InitModelStore(currentLesson.modelId);
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
            yield return request.SendWebRequest(); 
            if (request.isNetworkError || request.isHttpError)
            {

            }
            if (request.isDone)
            {
                Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                // bodyObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
            }
        }

        void updateDropDown()
        {
            listOrgans = LoadData.Instance.getListOrgans();
            foreach (ListOrganLesson organ in listOrgans.data)
            {
                options.Add(new Dropdown.OptionData(organ.organsName));
            }
            dropdown.AddOptions(options);
        }

        public IEnumerator WaitForAPIResponse(UnityWebRequest webRequest)
        {
            spinner.SetActive(true);
            Debug.Log("Calling API");
            while(!webRequest.isDone)
            {
                yield return new WaitForSeconds(.1f);
            }
        }

        void UpdateLessonObjective()
        {
            // Check form valid
            Debug.Log("form submit: ");
            Debug.Log("Index choose: " + dropdown.value);
            // Reference the Real index by the API 
            Debug.Log("Real index: " + listOrgans.data[dropdown.value].organsId); 
            Debug.Log("Lesson name modified: " + bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text);
            Debug.Log("Lesson obj mod: " + bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text);
            // Debug.Log($"organName {bodyObject.transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text}");
            Debug.Log("Organ name: " + bodyObject.transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text);
            
            Dictionary<string, string> requestBody = new Dictionary<string, string>()
            {
                {"modelId", ModelStoreManager.modelId.ToString()},
                {"lessonTitle", bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text},
                {"organId", listOrgans.data[dropdown.value].organsId.ToString()},
                {"organName", bodyObject.transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text},
                {"lessonObjectives", bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text},
                {"publicLesson", bodyObject.transform.GetChild(3).GetChild(0).GetComponent<Toggle>().isOn ? "1" : "0" } 
            };
            StartCoroutine(Submit(LessonManager.lessonId, requestBody));
        }

        public IEnumerator Submit(int lessonId, Dictionary<string, string> requestBody)
        {
            string url = String.Format(APIUrlConfig.UpdateLessonInfo, lessonId);
            // Serialize body as a Json string
            string requestBodyString = JsonConvert.SerializeObject(requestBody);
            // Convert Json body string into a byte array
            byte[] requestBodyData = System.Text.Encoding.UTF8.GetBytes(requestBodyString);
            // Create new UnityWebRequest, pass on our url and body as a byte array
            UnityWebRequest webRequest = UnityWebRequest.Put(url, requestBodyData);
            // Specify that our method is of type 'patch'
            webRequest.method = "PATCH";
            // Set request headers i.e. conent type, authorization etc
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            webRequest.SetRequestHeader("Content-Length", (requestBodyData.Length.ToString()));
            // Set the default download buffer
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // Send the request itself
            StartCoroutine(WaitForAPIResponse(webRequest));
            yield return webRequest.SendWebRequest();
            spinner.SetActive(false);
            // Check for errors
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {   
                // Invoke error action
                // onDeleteRequestError?.Invoke(webRequest.error);
                Debug.Log("an error has occur" + webRequest.error);
            }
            else
            {
                // Check when response is received
                if (webRequest.isDone)
                {
                    // Invoke success action
                    // onDeleteRequestSuccess?.Invoke("Patch Request Completed");
                    SceneManager.LoadScene(SceneConfig.lesson_edit);
                }
            }
        }
    }
}
