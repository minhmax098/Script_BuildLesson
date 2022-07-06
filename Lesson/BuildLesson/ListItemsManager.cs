using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using System;
using System.Net;
using UnityEngine.Networking; 
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace BuildLesson
{
    public class ListItemsManager : MonoBehaviour
    {
        private static ListItemsManager instance;
        public static ListItemsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ListItemsManager>();
                }
                return instance;
            }
        }

        public Button btnToggleListItem;
        public Animator toggleListItemAnimator;
        public Image groupIcon;
        // public RectTransform rectComponent;

        // Panel List Create Lesson 
        public GameObject panelListCreateLesson;
        public Button btnAddAudio; 
        public Button btnAddVideo; 
        public Button btnCancelListItem; 
        public GameObject panelAddAudio;
        public Button btnCancelAddAudio;
        public Button btnCancelAudio;

        // Panel Record 
        public GameObject panelRecord; 
        public Button btnCancelAddRecord;
        public Button btnCancelRecord;
        public Button btnRecord; 
        public Button btnUpload;
        public Button btnRedRecord;
        public GameObject timeIndicator;

        // Panel Save Record 
        public GameObject panelSaveRecord;
        public Button btnPlayBack;
        public Button btnCancelSaveRecordTL;
        public Button btnCancelSaveRecordBL;
        public Button btnRecording;

        // Panel Add Video 
        public GameObject panelAddVideo;   
        public Button btnCancelAddVideo;  // X button 
        public Button btnCancelPasteVideo;  // Cancel Button
        public Button btnSaveAddVideo;
        
        public InputField pasteVideo;
        private Regex ytbRegex = new Regex(@"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube(-nocookie)?\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$"); 
        private string jsonResponse;
        public Text title;
        private int calculatedSize = 25;
        public GameObject videoObj;

        // Panel Upload
        public GameObject panelUploadAudio;
        public Button btnCancelUploadAudio;
        public Button btnCancelUpload;
        public Button btnUploadAudio;

        private bool isRecordAudio = false;

        public static float startTime = 0f;
        private AudioSource audioSource;
        private bool isPlayingAudio = false;
        // add sampling rate 
        private static int samplingRate;

        void Awake()
        {
            // base.Awake();
            // Query the maximum frequency of the default microphone.
            int minSamplingRate; 
            Microphone.GetDeviceCaps(null, out minSamplingRate, out samplingRate);
        }

        void Start()
        {
            Debug.Log("Record Pannel Start: ");
            foreach (var device in Microphone.devices)
            {
                Debug.Log("Record Pannel Name: " + device);
            }
            InitEvents();
            // btnSaveAddVideo.onClick.AddListener(ValidateSaveAddVideo);
        }
        
        void Update()
        {
            // Update time here ! 
            if (panelRecord.activeSelf && isRecordAudio)
            {
                startTime += Time.deltaTime;
                DisplayTime(startTime, timeIndicator);
            }
            if (panelRecord.activeSelf && !isRecordAudio){
                timeIndicator.GetComponent<Text>().text = "00:00";
            }
        }
    
        void InitEvents()
        {
            btnToggleListItem.onClick.AddListener(ToggleListItem);
            btnAddAudio.onClick.AddListener(HandlerAddAudio);
            btnCancelListItem.onClick.AddListener(CancelListItem); 
            // rectComponent = GetComponent<RectTransform>();
            // groupIcon = rectComponent.GetComponent<Image>();
            groupIcon = GetComponent<Image>();
            btnCancelAddAudio.onClick.AddListener(CancelAddAudio);
            btnCancelAudio.onClick.AddListener(CancelAudio);
            
            // Record
            btnCancelAddRecord.onClick.AddListener(CancelAddRecord);
            btnCancelRecord.onClick.AddListener(CancelRecord);
            btnRecord.onClick.AddListener(RecordLesson);
            btnUpload.onClick.AddListener(UploadLesson);
            btnRedRecord.onClick.AddListener(HandlerRedRecord);

            // Playback Audio, Save Record
            btnPlayBack.onClick.AddListener(PlayStopAudio);
            btnCancelSaveRecordTL.onClick.AddListener(HandlerCancelSaveRecordTL);
            btnCancelSaveRecordBL.onClick.AddListener(HandlerCancelSaveRecordBL);
            btnRecording.onClick.AddListener(HandlerRecording);

            // Upload
            btnCancelUploadAudio.onClick.AddListener(CancelUploadAudio);
            btnCancelUpload.onClick.AddListener(CancelUpload);
            btnUploadAudio.onClick.AddListener(HandlerUploadAudio);
            
            // Add Video
            btnAddVideo.onClick.AddListener(HandlerAddVideo);
            btnCancelAddVideo.onClick.AddListener(HandlerCancelAddVideo);
            btnCancelPasteVideo.onClick.AddListener(HandlerCancelPasteVideo);
            btnSaveAddVideo.onClick.AddListener(HandlerSaveAddVideo);

            pasteVideo.onEndEdit.AddListener(delegate {LockInput(pasteVideo);});
        }

        private string formatString(string inputString, int maxSize)
        {
            if (inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "...";
            }
            return inputString;
        }
        
        // Checks if there is anything entered into the input field.
        void LockInput(InputField input)
        {
            if (input.text.Length > 0)
            {
                if (ytbRegex.IsMatch(input.text.Trim()))
                {
                    Debug.Log("Link is valid");
                    Debug.Log(input.text);
                    // StartCoroutine(GetVideoInfo(input.text.Trim()));
                    InfoLinkVideo info = GetVideoInfo(input.text.Trim());
                    Debug.Log("info: " + info.title);
                    title.gameObject.GetComponent<Text>().text = formatString(info.title.ToLower(), calculatedSize);
                    // title.text = info.title;
                    StartCoroutine(LoadVideoThumbnail(videoObj, info.thumbnail_url, input.text.Trim()));
                }
                else
                {
                    Debug.Log("Pop up notify! Invalid link");
                }
            }
            else if (input.text.Length == 0)
            {
                Debug.Log("Pop up notity! Enter empty string");
            }
        }

        void HandlerSaveAddVideo()
        {
            Debug.Log("Enter save record: ");
            StartCoroutine(SaveAddVideo(SaveLesson.lessonId, pasteVideo.text));
        }

        void ToggleListItem()
        {
            Debug.Log("log animation: " +!toggleListItemAnimator.GetBool(AnimatorConfig.isShowMeetingMemberList));
            toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, !toggleListItemAnimator.GetBool(AnimatorConfig.isShowMeetingMemberList)); 
            // groupIcon.transform.Rotate(0f, 180f, 0f);
        }
        void CancelListItem()
        {
           toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, false); 
        }
        void HandlerAddAudio()
        {
            Debug.Log("Add audio: ");

            panelAddAudio.SetActive(true);
        }
        void HandlerAddVideo()
        {
            panelAddVideo.SetActive(true);
            panelListCreateLesson.SetActive(false);
        }
       
        void CancelAddAudio()
        {
            panelAddAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelAudio()
        {
            panelAddAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelAddRecord()
        {
            panelRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelRecord()
        {
            panelRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void RecordLesson()
        {
            Debug.Log("Recorded: ");
            panelAddAudio.SetActive(false);
            panelRecord.SetActive(true);
        }
        void UploadLesson()
        {
            Debug.Log("Uploaded:");
            panelAddAudio.SetActive(false);
            panelUploadAudio.SetActive(true);
        }
        void CancelUploadAudio()
        {
            panelUploadAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelUpload()
        {
            panelUploadAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerCancelAddVideo()
        {
            panelAddVideo.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerCancelPasteVideo()
        {
            pasteVideo.text = "";
            title.gameObject.GetComponent<Text>().text = "";
            videoObj.GetComponent<Image>().sprite = null;
            videoObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            panelAddVideo.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerRecording()
        {
            Debug.Log("Recording again: ");
            panelSaveRecord.SetActive(false);
            panelRecord.SetActive(true);
            HandlerRedRecord();
        }

        // Enter save call API AddVideoLesson
        IEnumerator SaveAddVideo(int lessonId, string video)
        {
            var webRequest = new UnityWebRequest(APIUrlConfig.AddVideoLesson, "POST");
            string requestBody = "{\"lessonId\": \"" + lessonId + "\", \"video\": \"" + video + "\"}";
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestBody);
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
                    panelAddVideo.SetActive(false);
                    panelListCreateLesson.SetActive(false);
                    // UpdateLessonInforPannel(lessonId);
                    toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
                    StartCoroutine(LoadDataListItemPanel.Instance.UpdateLessonInforPannel(lessonId));
                }
            }
        }

        public InfoLinkVideo GetVideoInfo(string link)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetLinkVideo, link)); 
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("jsonResponse");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<InfoLinkVideo>(jsonResponse); 
        }

        public string DecodeFromUtf8(string utf8String)
        {
            // copy the string as UTF-8 bytes.
            byte[] utf8Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
                utf8Bytes[i] = (byte)utf8String[i];
            }
            return Encoding.UTF8.GetString(utf8Bytes,0,utf8Bytes.Length);
        }

        void HandlerCancelSaveRecordTL()
        {
            panelSaveRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerCancelSaveRecordBL()
        {
            startTime = 0f;
            timeIndicator.GetComponent<Text>().text = "00:00";
            btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageRecordingIcon);
            panelSaveRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }

        void HandlerRedRecord()
        {
            isRecordAudio = !isRecordAudio;
            // Change UI 
            if (isRecordAudio)
            {
                btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageStopRecordingIcon);
                // Start recording
                audioSource = panelSaveRecord.GetComponent<AudioSource>();
                audioSource.clip = Microphone.Start(null, true, 3599, samplingRate); // Maximum record 1 hour
            }
            else 
            {
                startTime = 0f;
                panelRecord.SetActive(false);
                panelSaveRecord.SetActive(true);
                // Stop recording
                audioSource = EndRecording(audioSource);
                AudioManager.Instance.DisplayAudio(true);
                // Play it back to see the result
                // audioSource.Play();
            }
        }

        void DisplayTime(float timeToDisplay, GameObject time)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            time.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        AudioSource EndRecording (AudioSource audS) 
        {
            // Reference: https://answers.unity.com/questions/544264/record-dynamic-length-from-microphone.html
            // Capture the current clip data
            AudioClip recordedClip = audS.clip;
            var position = Microphone.GetPosition(null);  // Use default microphone 
            var soundData = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData (soundData, 0);
            // Create shortened array for the data that was used for recording
            var newData = new float[position * recordedClip.channels];
            // anonymous Microphone.End (null);
            // Copy the used samples to a new array
            for (int i = 0; i < newData.Length; i++) 
            {
                newData[i] = soundData[i];
            }
            // One does not simply shorten an AudioClip,
            // so we make a new one with the appropriate length
            var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
            newClip.SetData(newData, 0); //Give it the data from the old clip
            // Replace the old clip
            AudioClip.Destroy(recordedClip);
            audS.clip = newClip;   
            return audS;
        }

        void PlayStopAudio()
        {
            isPlayingAudio = !isPlayingAudio;
            AudioManager.Instance.ControlAudio(isPlayingAudio);
        }

        public void HandlerUploadAudio()
        {
            Debug.Log("Upload audio from local: ");
            // StartCoroutine(HandleUploadModel3D(File.ReadAllBytes(path), path));
        }
        // public IEnumerator HandleUploadModel3D(byte[] fileData, string fileName)
        // {
        //     var form = new WWWForm(); 
        //     form.AddBinaryData("file: ", fileData, fileName);
        // }

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
                videoObj.GetComponent<Image>().sprite = sprite;
                videoObj.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => NavigageToVideo(videoUri));
            }
        }

        void NavigageToVideo(string videoUri)
        {
            Application.OpenURL(videoUri);
        }
    }
}
