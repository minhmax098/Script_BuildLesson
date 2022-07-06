using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;
using System;


public class VideoManager : MonoBehaviour
{
    private static VideoManager instance;
    public static VideoManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<VideoManager>();
            return instance;
        }
    }

    VideoPlayer videoPlayer;
    GameObject selectedObject;

    public bool IsPlayingVideo { get; set; }
    public bool IsShowingFullScreen { get; set; }
    public bool IsDisplayVideo { get; set; }

    // UI
    public Text txtNameOrgan;
    public GameObject panelVideo;
    public Button btnControlVideo;
    public Button btnControlVideoFull;
    public GameObject sliderControlVideo;
    public GameObject sliderControlVideoFull;
    public GameObject panelFullScreen;
    public GameObject ZoomScreen;

    void OnEnable()
    {
        ObjectManager.onInitOrganSuccessfully += OnInitOrganSuccessfully;
    }

    void OnDisable()
    {
        ObjectManager.onInitOrganSuccessfully -= OnInitOrganSuccessfully;
    }

    void OnInitOrganSuccessfully()
    {
        txtNameOrgan.text = ObjectManager.Instance.OriginObject.name;
    }

    void Start()
    {
        SetPropertyVideo(false, false, false);
    }

    void Update()
    {
        if (ObjectManager.Instance.CurrentObject == null)
        {
            return;
        }

        if (videoPlayer != null)
        {
            sliderControlVideo.GetComponent<Slider>().value = (float)videoPlayer.frame;
            sliderControlVideoFull.GetComponent<Slider>().value = (float)videoPlayer.frame;
        }
    }
    public void SetPropertyVideo(bool _IsPlayingVideo, bool _IsShowingFullScreen, bool _IsDisplayVideo)
    {
        IsPlayingVideo = _IsPlayingVideo;
        IsShowingFullScreen = _IsShowingFullScreen;
        IsDisplayVideo = _IsDisplayVideo;
    }

    public void ControlVideo(bool _IsPlayingVideo)
    {
        IsPlayingVideo = _IsPlayingVideo;
        if (videoPlayer != null)
        {
            if (IsPlayingVideo)
            {
                videoPlayer.Play();
                btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
                btnControlVideoFull.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
            }
            else
            {
                videoPlayer.Pause();
                btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
                btnControlVideoFull.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
            }
        }
    }

    public void ChangeVideoView(bool _IsShowingFullScreen)
    {
        IsShowingFullScreen = _IsShowingFullScreen;
        if (videoPlayer != null)
        {
            if (IsShowingFullScreen)
            {
                if (!videoPlayer.isPlaying)
                {
                    IsPlayingVideo = true;
                    ControlVideo(IsPlayingVideo);
                }
                panelFullScreen.SetActive(true);
                ZoomScreen.SetActive(false);
                videoPlayer.targetTexture = Resources.Load<RenderTexture>("Textures/FullScreen");
            }
            else
            {
                panelFullScreen.SetActive(false);
                ZoomScreen.SetActive(true);
                videoPlayer.targetTexture = Resources.Load<RenderTexture>("Textures/VideoZoom");
            }
        }

    }

    public void DisplayVideo(bool _IsDisplayVideo)
    {
        IsDisplayVideo = _IsDisplayVideo;
        if (IsDisplayVideo)
        {
            PopupManager.Instance.IsClickedMenu = false;
            PopupManager.Instance.ShowListMedia(PopupManager.Instance.IsClickedMenu);

            SetPropertyVideo(true, false, true);
            SetPropertyComponentVideo();
            panelVideo.SetActive(true);
            ControlVideo(IsPlayingVideo);
        }
        else
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            SetPropertyVideo(false, false, false);
            videoPlayer = null;

            panelVideo.SetActive(false);
        }
    }

    public void HandleEventClickItemVideo(GameObject itemVideo)
    {
        selectedObject = GameObject.Find(itemVideo.name);

        // if (selectedObject == null || selectedObject == GameObjectManager.Instance.CurrentObject)
        // {
        //     return;
        // }
        // // Display object selected
        // ChildNodeManager.Instance.DisplaySelectedObject(selectedObject);

        // // Change current object
        // GameObjectManager.Instance.SelectDoubleTouchObject(selectedObject);

        // // Append child node UI in tree
        // ChildNodeManager.Instance.CreateChildNodeUI(selectedObject.name);
        MediaManager.Instance.UnClickItemMedia();

        itemVideo.transform.GetChild(0).gameObject.SetActive(true);
        itemVideo.transform.GetChild(3).gameObject.SetActive(true);

        IsDisplayVideo = true;
        DisplayVideo(IsDisplayVideo);

    }

    void SetPropertyComponentVideo()
    {
        // if (selectedObject.GetComponent<VideoPlayer>() == null)
        // {
        //     selectedObject.AddComponent<VideoPlayer>();
        // }
        videoPlayer = selectedObject.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;

        // videoPlayer.url = PathConfig.VIDEO_PATH  + nameVideo + ".mp4";
        sliderControlVideo.GetComponent<Slider>().maxValue = videoPlayer.frameCount;
    }
}
