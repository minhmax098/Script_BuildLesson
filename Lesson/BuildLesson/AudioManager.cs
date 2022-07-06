using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

namespace BuildLesson
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();
                }
                return instance; 
            }
        }
       
        public GameObject panelListCreateLesson;
        private AudioSource audioData;
        public GameObject selectedObject;
        public bool IsPlayingAudio { get; set; }
        public bool IsDisplayAudio { get; set; }
        // UI
        public Text timeCurrentAudio;
        public Text timeEndAudio;
        public GameObject sliderControlAudio;
        public Button btnControlAudio;
        public Button btnSaveRecord; 
        public Animator toggleListItemAnimator;
        
        void Start()
        {
            InitEvents();
            SetPropertyAudio(false, false);
        }

        void Update()
        {
           if (audioData != null)
            {
                timeCurrentAudio.text = FormatTime(audioData.time);
                sliderControlAudio.GetComponent<Slider>().value = audioData.time;
            }
        }

        void InitEvents()
        {
            // btnToogleListItem.onClick.AddListener(ToggleListItem);
            // btnControlAudio.onClick.AddListener(() => ControlAudio(IsPlayingAudio));
            btnSaveRecord.onClick.AddListener(HandlerSaveRecord);
            // btnCancelListItem.onClick.AddListener(CancelListItem);
        }

        public void SetPropertyAudio(bool _IsPlayingAudio, bool _IsDisplayAudio)
        {
            IsPlayingAudio = _IsPlayingAudio;
            IsDisplayAudio = _IsDisplayAudio;
        }

        public void SetPropertyComponentAudio()
        {
            audioData = selectedObject.GetComponent<AudioSource>();
            if (audioData != null)
            {
                timeEndAudio.GetComponent<Text>().text = FormatTime(audioData.clip.length);
                sliderControlAudio.GetComponent<Slider>().maxValue = audioData.clip.length;
            }
        }

        public void ControlAudio(bool _IsPlayingAudio)
        {
            Debug.Log("Control Audio Click");
            IsPlayingAudio = _IsPlayingAudio; 
            if (audioData != null)
            {
                if (IsPlayingAudio)
                {
                    audioData.Play();
                    btnControlAudio.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
                }
                else 
                {
                    audioData.Pause();
                    btnControlAudio.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
                }
            }
        }

        public void DisplayAudio(bool _IsDisplayAudio)
        {
            IsDisplayAudio = _IsDisplayAudio;
            if (IsDisplayAudio)
            {
                // btnAudio.SetActive(false);
                SetPropertyAudio(false, true);
                SetPropertyComponentAudio(); // ham reset thuoc tinh audio
            }
            else 
            {
                if (audioData != null)
                {
                    audioData.Stop();
                }
                SetPropertyAudio(false, false);
                audioData = null;
                // btnAudio.SetActive(true);
            }
        }

        public static string FormatTime(float time)
        {
            int minutes = (int)time / 60;
            int seconds = (int)time - 60 * minutes;
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        void HandlerSaveRecord()
        {
            ListItemsManager.startTime = 0f;
            Debug.Log("Enter save record: ");
            StartCoroutine(SaveRecordAudio());
        }

        IEnumerator SaveRecordAudio()
        {
            Debug.Log("Save video: ");
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            Debug.Log("Created lesson save audio: " + SaveLesson.lessonId);
            formData.Add(new MultipartFormDataSection("lessonId", Convert.ToString(SaveLesson.lessonId)));
            Debug.Log("Created lesson save done: ");
            formData.Add(new MultipartFormFileSection("audio", Helper.FromAudioClip(audioData.clip), "abc.mp3", "audio/mp3"));

            UnityWebRequest request = UnityWebRequest.Post(APIUrlConfig.AddAudioLesson, formData);
            request.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Add audio complete: ");
                selectedObject.SetActive(false);
                panelListCreateLesson.SetActive(false);
                toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
                StartCoroutine(LoadDataListItemPanel.Instance.UpdateLessonInforPannel(SaveLesson.lessonId));
            }
        }
    }
}
