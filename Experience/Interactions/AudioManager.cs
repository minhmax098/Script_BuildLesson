using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<AudioManager>();
            return instance;
        }
    }

    AudioSource audioData;
    GameObject selectedObject;
    public bool IsPlayingAudio { get; set; }
    public bool IsDisplayAudio { get; set; }

    // UI
    public Text timeCurrentAudio;
    public Text timeEndAudio;
    public GameObject sliderControlAudio;
    public Button btnControlAudio;
    public GameObject panelAudio;
    public GameObject btnAudio;
    void Start()
    {
        SetPropertyAudio(false, false);
    }
    
    void Update()
    {
        if (ObjectManager.Instance.CurrentObject == null)
        {
            return;
        }
        if (audioData != null)
        {
            timeCurrentAudio.text = Helper.FormatTime(audioData.time);
            sliderControlAudio.GetComponent<Slider>().value = audioData.time;
        }
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
            timeEndAudio.GetComponent<Text>().text = Helper.FormatTime(audioData.clip.length);
            sliderControlAudio.GetComponent<Slider>().maxValue = audioData.clip.length;
        }
    }
    
    public void ControlAudio(bool _IsPlayingAudio)
    {        
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

    public void DisplayAudioWithObject(bool _IsDisplayAudio)
    {
        IsDisplayAudio = _IsDisplayAudio;
        selectedObject = ObjectManager.Instance.CurrentObject;
        DisplayAudio(IsDisplayAudio);
    }
    public void DisplayAudio(bool _IsDisplayAudio)
    {
        IsDisplayAudio = _IsDisplayAudio;
        if (IsDisplayAudio)
        {
            btnAudio.SetActive(false);
            PopupManager.Instance.IsClickedMenu = false;
            PopupManager.Instance.ShowListMedia(PopupManager.Instance.IsClickedMenu);
            
            SetPropertyAudio(true, true);
            SetPropertyComponentAudio();
            panelAudio.SetActive(true);
            ControlAudio(IsDisplayAudio);
        }
        else
        {
            if (audioData != null)
            {
                audioData.Stop();
            }

            SetPropertyAudio(false, false);
            audioData = null;

            panelAudio.SetActive(false);
            btnAudio.SetActive(true);
        }
    }

    public void HandleEventClickItemAudio(GameObject itemAudio)
    {
        selectedObject = GameObject.Find(itemAudio.name);
        // GameObject selected = GameObject.Find(itemAudio.name);
        // Debug.Log(selected);
        // Display object selected
        // ChildNodeManager.Instance.DisplaySelectedObject(selected);
        // Change current object
        // GameObjectManager.Instance.SelectDoubleTouchObject(selected);
        // Append child node UI in tree
        // ChildNodeManager.Instance.CreateChildNodeUI(selected.name);
        // UIHandler.Instance.isClickedBtnAudio = true;
        // UIHandler.Instance.HandlerBtnAudio();

        MediaManager.Instance.UnClickItemMedia();

        // Set UI item clicked
        itemAudio.transform.GetChild(0).gameObject.SetActive(true);
        itemAudio.transform.GetChild(3).gameObject.SetActive(true);

        // Play Audio
        IsDisplayAudio = true;
        DisplayAudio(IsDisplayAudio);
    }
}
