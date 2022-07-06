using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System; 
using System.Threading.Tasks; 
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace CreateLesson
{
    public class LoadData : MonoBehaviour
    {
        private string jsonResponse;
        private static LoadData instance;
        public static LoadData Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<LoadData>();
                }
                return instance; 
            }
        }
        public ListOrgans getListOrgans()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrlConfig.GetListOrgans); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("Json response: ");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<ListOrgans>(jsonResponse);
        }
        
        public IEnumerator buildLesson(PublicLesson requestBody)
        {   
            var webRequest = new UnityWebRequest(APIUrlConfig.CreateLessonInfo, "POST");
            Debug.Log(requestBody.modelId);
            Debug.Log(requestBody.lessonObjectives);
            // string requestBodyString = JsonConvert.SerializeObject(requestBody);
            string requestBodyString = JsonUtility.ToJson(requestBody);
            Debug.Log(requestBodyString);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestBodyString);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {   
                // Invoke error action
                // onDeleteRequestError?.Invoke(webRequest.error);
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received
                if (webRequest.isDone)
                {
                    // Invoke success action
                    // onDeleteRequestSuccess?.Invoke("Patch request completed");
                    Debug.Log("Created lesson: " + webRequest.downloadHandler.text);
                    CreateLessonInfoResponse rsp = JsonUtility.FromJson<CreateLessonInfoResponse>(webRequest.downloadHandler.text);
                    Debug.Log("Created lesson: " + rsp.data[0].lessonId);
                    SaveLesson.SaveLessonId(rsp.data[0].lessonId);
                    SceneManager.LoadScene(SceneConfig.buildLesson);
                }
            }

        }
        
    }
}
