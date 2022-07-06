using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using UnityEngine.UI;

namespace BuildLesson 
{
    public class LoadDataListItemPanel : MonoBehaviour
    {
        private static LoadDataListItemPanel instance;
        public static LoadDataListItemPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoadDataListItemPanel>();
                }
                return instance; 
            }
        }
        public GameObject lessonInfoPanel;
        private int calculatedSize = 30;
        public Button btnDeleteAudio;
        public Button btnDeleteVideo;
        public GameObject showListItem;

        // panel PopUpDeleteActions
        public GameObject panelPopUpDeleteActions;
        public Button btnExitPopupDeleteActions;
        public Button btnDeleteActions;
        public Button btnCancelDeleteActions; 

        // panel PopUpDeleteActionsVideo
        public GameObject panelPopUpDeleteActionsVideo;
        public Button btnExitPopupDeleteActionsVideo;
        public Button btnDeleteActionsVideo;
        public Button btnCancelDeleteActionsVideo; 

        void Update()
        {
            btnExitPopupDeleteActions.onClick.AddListener(HandlerExitPopupDeleteActions);
            btnCancelDeleteActions.onClick.AddListener(HandlerCancelDeleteActions);
            btnExitPopupDeleteActionsVideo.onClick.AddListener(HandlerExitPopupDeleteActionsVideo);
            btnCancelDeleteActionsVideo.onClick.AddListener(HandlerCancelDeleteActionsVideo);
        }
        void HandlerExitPopupDeleteActions()
        {
            panelPopUpDeleteActions.SetActive(false);
        }
        void HandlerCancelDeleteActions()
        {
            panelPopUpDeleteActions.SetActive(false);
        }
        void HandlerExitPopupDeleteActionsVideo()
        {
            panelPopUpDeleteActionsVideo.SetActive(false);
        }
        void HandlerCancelDeleteActionsVideo()
        {
            panelPopUpDeleteActionsVideo.SetActive(false);
        }

        // Update the Pannel 
        public IEnumerator UpdateLessonInforPannel(int lessonId)
        {
            Debug.Log("Get lesson info: " + String.Format(APIUrlConfig.GetLessonsByID, lessonId));
            UnityWebRequest webRequest = UnityWebRequest.Get(String.Format(APIUrlConfig.GetLessonsByID, lessonId));
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
                    string rsp = webRequest.downloadHandler.text;
                    LessonDetail lesson = JsonUtility.FromJson<AllLessonDetails>(rsp).data[0];
                    // Invoke success action
                    Debug.Log("Get lesson info: " + rsp);
                    Debug.Log("Get lesson info: " + lesson.lessonTitle);
                    Debug.Log("Get lesson info: " + lesson.audio);
                    // Get all info from lesson and view into the page
                    foreach (Transform child in lessonInfoPanel.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    // Load the info
                    if (lesson.audio != "")
                    {
                        StartCoroutine(loadAudio(lesson.audio, lessonId));
                    }
                    if (lesson.video != "")
                    {
                        StartCoroutine(loadVideo(lesson.video, lessonId));
                    } 
                    // else (totalLesson == 0)
                    // {
                    //     showListItem.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"No item";
                    // }
                }
            }
        }

        private IEnumerator loadAudio(string audioUrl, int lessonId)
        {
            GameObject audioComp = Instantiate(Resources.Load(PathConfig.ADD_AUDIO) as GameObject);
            audioComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteAudio(lessonId));
            Debug.Log("Load audio url: " + String.Format(APIUrlConfig.LoadLesson, audioUrl));
            UnityWebRequest webRequest = UnityWebRequest.Get(String.Format(APIUrlConfig.LoadLesson, audioUrl));
            audioComp.transform.parent = lessonInfoPanel.transform;     
            audioComp.transform.localScale = new Vector3(1f, 1f, 1f);                   
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {   
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received
                if (webRequest.isDone)
                {
                    byte[] audio = webRequest.downloadHandler.data;
                    // Convert to AudioClip 
                    AudioClip audioData = Helper.ToAudioClip(audio);
                    // Convert 
                    audioComp.GetComponent<AudioSource>().clip = audioData;
                    AudioManager1.Instance.DisplayAudio(true);
                    // audioComp.GetComponent<AudioSource>().Play();
                }
            }
        }
        
        private IEnumerator loadVideo(string videoUrl, int lessonId)
        {
            GameObject videoComp = Instantiate(Resources.Load(PathConfig.ADD_VIDEO) as GameObject); 
            videoComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteVideo(lessonId));
            Debug.Log("Load video url: " + String.Format(APIUrlConfig.LoadLesson, videoUrl));
            UnityWebRequest webRequest = UnityWebRequest.Get(String.Format(APIUrlConfig.GetLinkVideo, videoUrl));
            videoComp.transform.parent = lessonInfoPanel.transform;
            videoComp.transform.localScale = new Vector3(1f, 1f, 1f);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received 
                if (webRequest.isDone)
                {
                    Debug.Log("Update video: " + webRequest.downloadHandler.text);
                    InfoLinkVideo dataResp = JsonUtility.FromJson<InfoLinkVideo>(webRequest.downloadHandler.text); 
                    videoComp.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = formatString(dataResp.title.ToLower(), calculatedSize);
                    StartCoroutine(LoadVideoThumbnail(videoComp, dataResp.thumbnail_url, videoUrl));
                }
            }
        }

        IEnumerator LoadVideoThumbnail(GameObject videoObj, string imageUri, string videoUri)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {

            }
            if (request.isDone)
            {
                Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                
                videoObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
                videoObj.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => NavigageToVideo(videoUri));
            }
        }

        private string formatString(string inputString, int maxSize)
        {
            if (inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "...";
            }
            return inputString;
        }

        void NavigageToVideo(string videoUri)
        {
            Application.OpenURL(videoUri);
        }

        void HandlerDeleteAudio(int lessonId)
        {
            panelPopUpDeleteActions.SetActive(true);
            btnDeleteActions = GameObject.Find("BtnDeleteActions").GetComponent<Button>();
            btnDeleteActions.onClick.AddListener(() => StartCoroutine(DeleteAudioLesson(lessonId)));
        }
        void HandlerDeleteVideo(int lessonId)
        {
            panelPopUpDeleteActionsVideo.SetActive(true);
            btnDeleteActionsVideo = GameObject.Find("BtnDeleteActionsVideo").GetComponent<Button>();
            btnDeleteActionsVideo.onClick.AddListener(() => StartCoroutine(DeleteVideoLesson(lessonId)));
        }
        IEnumerator DeleteAudioLesson(int lessonId)
        {
            panelPopUpDeleteActions.SetActive(false);
            Debug.Log("Trigger delete audio: ");
            UnityWebRequest webRequest = UnityWebRequest.Delete(String.Format(APIUrlConfig.DeleteAudioLesson, lessonId));
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.isDone)
                {
                    Debug.Log("Delete audio: ");
                    StartCoroutine(UpdateLessonInforPannel(lessonId));
                }
            }
        }
        IEnumerator DeleteVideoLesson(int lessonId)
        {
            Debug.Log("Trigger delete video: ");
            UnityWebRequest webRequest = UnityWebRequest.Delete(String.Format(APIUrlConfig.DeleteVideoLesson, lessonId));
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.isDone)
                {
                    Debug.Log("Delete video: ");
                    StartCoroutine(UpdateLessonInforPannel(lessonId));
                    panelPopUpDeleteActionsVideo.SetActive(false);
                }
            }
        }
    }
}

