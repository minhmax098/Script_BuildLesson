using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Video;


public class MediaManager : MonoBehaviour
{
    private static MediaManager instance;
    public static MediaManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MediaManager>();
            return instance;
        }
    }

    // UI
    public GameObject contentListMedia;
    public Text txtNameOrgan;


    void OnEnable()
    {
        ObjectManager.onInitOrganSuccessfully += OnInitOrganSuccessfully;
    }

    void OnDisable()
    {
        ObjectManager.onInitOrganSuccessfully -= OnInitOrganSuccessfully;
    }
    void Start()
    {

    }

    void OnInitOrganSuccessfully()
    {
        DestroyItemMedia();
        GetAllMediaOfOrgan(ObjectManager.Instance.OriginObject);
    }

    public void GetAllMediaOfOrgan(GameObject obj)
    {
        txtNameOrgan.text = ObjectManager.Instance.OriginOrganData.Name;

        int childCount = obj.transform.childCount;
        if (obj.GetComponent<AudioSource>() != null)
        {            
            GameObject itemAudio = Instantiate(Resources.Load(PathConfig.MODEL_ITEM_AUDIO) as GameObject);
            itemAudio.transform.SetParent(contentListMedia.transform, false);
            itemAudio.transform.GetChild(2).GetComponent<Text>().text = "Audio: " + obj.name;
            itemAudio.name = obj.name;
            itemAudio.transform.GetComponent<Button>().onClick.AddListener(delegate { AudioManager.Instance.HandleEventClickItemAudio(itemAudio);});
        }

        if (obj.GetComponent<VideoPlayer>() != null)
        {
            GameObject itemVideo = Instantiate(Resources.Load(PathConfig.MODEL_ITEM_AUDIO) as GameObject);
            itemVideo.transform.SetParent(contentListMedia.transform, false);
            itemVideo.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.VIDEO_UNCLICK_IMAGE);
            itemVideo.transform.GetChild(2).GetComponent<Text>().text = "Video: " + obj.name;
            itemVideo.name = obj.name;
            itemVideo.transform.GetComponent<Button>().onClick.AddListener(delegate { VideoManager.Instance.HandleEventClickItemVideo(itemVideo);});
        }

        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {                
                GetAllMediaOfOrgan(obj.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            return;
        }
    }

    public void UnClickItemMedia()
    {
        if (contentListMedia.transform.childCount > 0)
        {
            foreach(Transform itemMedia in contentListMedia.transform)
            {
                itemMedia.transform.GetChild(0).gameObject.SetActive(false);
                itemMedia.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    public void DestroyItemMedia()
    {
        if (contentListMedia.transform.childCount > 0)
        {
            foreach(Transform itemMedia in contentListMedia.transform)
            {
                Destroy(itemMedia.transform.gameObject);
            }
        }
    }

    public void StopMedia()
    {
        PopupManager.Instance.IsClickedMenu = false;
        PopupManager.Instance.ShowListMedia(PopupManager.Instance.IsClickedMenu);
        VideoManager.Instance.IsDisplayVideo = false;
        VideoManager.Instance.DisplayVideo(VideoManager.Instance.IsDisplayVideo);
        AudioManager.Instance.IsDisplayAudio = false;
        AudioManager.Instance.DisplayAudio(AudioManager.Instance.IsDisplayAudio);
    }
}
