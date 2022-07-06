using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class ExperienceManager : MonoBehaviour
{
    public Button btnSeparate;
    public Button btnLabel;
    public Button btnXray;
    public Button btnHold;
    public Button btnShowPopupExit;
    public Button btnClosePopupExit;
    public Button btnExitLesson;
    public Button btnContinueLesson;
    public Button btnShowGuideBoard;
    public Button btnExitGuideBoard;
    public Button btnMenu;
    public Button btnAudio;
    public Button btnAR;
    public Button btnSwitch;

    // Video
    public Button btnControlVideo;
    public Button btnControlVideoFull;
    public Button btnExitVideo;
    public Button btnFullScreen;
    public Button btnZoomScreen;

    // Audio
    public Button btnControlAudio;
    public Button btnExitAudio;

    // Mode AR
    public Button btnBacktoMode3D;

    void OnEnable()
    {
        TouchManager.onSelectChildObject += OnSelectChildObject;
        TreeNodeManager.onClickNodeTree += OnClickNodeTree;
        ObjectManager.onResetObject += OnResetObject;
        ARUIManager.OnARPlaceObject += OnARPlaceObject;
    }

    void OnDisable()
    {
        TouchManager.onSelectChildObject -= OnSelectChildObject;
        TreeNodeManager.onClickNodeTree -= OnClickNodeTree;
        ObjectManager.onResetObject -= OnResetObject;
        ARUIManager.OnARPlaceObject -= OnARPlaceObject;
    }

    void OnResetObject()
    {
        ObjectManager.Instance.DestroyOriginalObject();
        ObjectManager.Instance.InitOriginalExperience();
        TreeNodeManager.Instance.ClearAllNodeTree();
        XRayManager.Instance.HandleXRayView(XRayManager.Instance.IsMakingXRay);
        Helper.ResetStatusFeature();
    }

    void OnSelectChildObject(GameObject selectedObject)
    {
        OnResetStatusFeature();
        TreeNodeManager.Instance.DisplaySelectedObject(selectedObject, ObjectManager.Instance.CurrentObject);
        ObjectManager.Instance.ChangeCurrentObject(selectedObject);
        TreeNodeManager.Instance.CreateChildNodeUI(selectedObject.name);
    }

    void OnClickNodeTree(string nodeName)
    {
        OnResetStatusFeature();
        XRayManager.Instance.HandleXRayView(false);
        if (nodeName != ObjectManager.Instance.CurrentObject.name)
        {
            GameObject selectedObject = GameObject.Find(nodeName);
            TreeNodeManager.Instance.DisplayAllChildSelectedObject(selectedObject);
            // Conjoined
            ObjectManager.Instance.CurrentObject = selectedObject;
            SeparateManager.Instance.HandleSeparate(false);

            ObjectManager.Instance.ChangeCurrentObject(selectedObject);
            TreeNodeManager.Instance.RemoveItem(nodeName);
            // StartCoroutine(Helper.MoveObject(Camera.main.gameObject, Camera.main.transform.position));
        }
        ObjectManager.Instance.OriginObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Start()
    {
        ObjectManager.Instance.InitOriginalExperience();
        CheckMode();
        InitInteractions();
        InitEvents();
        InitLayoutScreen();
    }

    void Update()
    {
        TouchManager.Instance.HandleTouchInteraction();
        EnableFeature();
    }

    void InitLayoutScreen()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        StatusBarManager.statusBarState = StatusBarManager.States.Hidden;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }

    void EnableFeature()
    {
        if (ObjectManager.Instance.CurrentObject != null)
        {
            if (ObjectManager.Instance.CurrentObject.transform.childCount == 0)
            {
                btnHold.interactable = false;
                btnLabel.interactable = false;
                btnSeparate.interactable = false;
            }
            else
            {
                btnHold.interactable = true;
                btnLabel.interactable = true;
                btnSeparate.interactable = true;
            }

            if (ObjectManager.Instance.CurrentObject.GetComponent<AudioSource>() == null)
            {
                btnAudio.interactable = false;
            }
            else
            {
                btnAudio.interactable = true;
            }

            if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
            {
                btnAR.gameObject.SetActive(false);
                btnSwitch.gameObject.SetActive(true);
            }
            else if(ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_3D)
            {
                btnAR.gameObject.SetActive(true);
                btnSwitch.gameObject.SetActive(false);
            }   
        }
    }

    void InitInteractions()
    {
        XRayManager.Instance.IsMakingXRay = false;
        SeparateManager.Instance.IsSeparating = false;
        TouchManager.Instance.IsClickedHoldBtn = false;
        LabelManager.Instance.IsShowingLabel = false;
        PopupManager.Instance.InitPopupMangaer(false, false, false);
    }

    void CheckMode()
    {
        ModeManager.Instance.CheckModeInitToView(ModeManager.Instance.Mode);
    }
    void InitEvents()
    {
        btnSeparate.onClick.AddListener(HandleSeparation);
        btnXray.onClick.AddListener(HandleXRayView);
        btnLabel.onClick.AddListener(HandleLabelView);
        btnHold.onClick.AddListener(HandleHoldAction);
        btnShowPopupExit.onClick.AddListener(ToggleExitConfirmationPopup);
        btnClosePopupExit.onClick.AddListener(ToggleExitConfirmationPopup);
        btnExitLesson.onClick.AddListener(ExitLesson);
        btnContinueLesson.onClick.AddListener(ToggleExitConfirmationPopup);
        btnShowGuideBoard.onClick.AddListener(ToggleGuideBoard);
        btnExitGuideBoard.onClick.AddListener(ToggleGuideBoard);
        btnMenu.onClick.AddListener(ToggleMenuListMediaView);

        // Video
        btnControlVideo.onClick.AddListener(ToggleStatusPlayingVideo);
        btnControlVideoFull.onClick.AddListener(ToggleStatusPlayingVideo);
        btnExitVideo.onClick.AddListener(ToggleStatusDisPlayVideo);
        btnFullScreen.onClick.AddListener(ToggleModeShowingVideo);
        btnZoomScreen.onClick.AddListener(ToggleModeShowingVideo);

        // Audio
        btnAudio.onClick.AddListener(HanldeDisplayAudio);
        btnExitAudio.onClick.AddListener(HanldeDisplayAudio);
        btnControlAudio.onClick.AddListener(ToggleStatusPlayingAudio);

        // Switch mode
        btnAR.onClick.AddListener(ToggleStatusModeAR);
        btnBacktoMode3D.onClick.AddListener(ToggleStatusMode3D);
        btnSwitch.onClick.AddListener(ToggleStatusMode3D);
    }

    void ToggleGuideBoard()
    {
        PopupManager.Instance.IsClickedGuideBoard = !PopupManager.Instance.IsClickedGuideBoard;
        PopupManager.Instance.ShowGuideBoard(PopupManager.Instance.IsClickedGuideBoard);
    }

    void ExitLesson()
    {
        if (PlayerPrefs.GetString("user_email") != "")
        {
            SceneManager.LoadScene(SceneConfig.lesson);
        }
        else
        {
            SceneManager.LoadScene(SceneConfig.lesson_nosignin);
        }
    }

    void ToggleExitConfirmationPopup()
    {
        PopupManager.Instance.IsClickedExitLesson = !PopupManager.Instance.IsClickedExitLesson;
        PopupManager.Instance.ShowPopupExitLesson(PopupManager.Instance.IsClickedExitLesson);
    }


    void HandleSeparation()
    {
        SeparateManager.Instance.IsSeparating = !SeparateManager.Instance.IsSeparating;
        SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);
    }

    void HandleLabelView()
    {
        LabelManager.Instance.IsShowingLabel = !LabelManager.Instance.IsShowingLabel;
        LabelManager.Instance.HandleLabelView(LabelManager.Instance.IsShowingLabel);
    }

    void HandleXRayView()
    {
        XRayManager.Instance.IsMakingXRay = !XRayManager.Instance.IsMakingXRay;
        XRayManager.Instance.HandleXRayView(XRayManager.Instance.IsMakingXRay);
    }

    void HandleHoldAction()
    {
        TouchManager.Instance.IsClickedHoldBtn = !TouchManager.Instance.IsClickedHoldBtn;
    }

    void ToggleMenuListMediaView()
    {
        PopupManager.Instance.IsClickedMenu = !PopupManager.Instance.IsClickedMenu;
        PopupManager.Instance.ShowListMedia(PopupManager.Instance.IsClickedMenu);
    }
    void ToggleStatusPlayingVideo()
    {
        VideoManager.Instance.IsPlayingVideo = !VideoManager.Instance.IsPlayingVideo;
        VideoManager.Instance.ControlVideo(VideoManager.Instance.IsPlayingVideo);
    }

    void ToggleStatusDisPlayVideo()
    {
        VideoManager.Instance.IsDisplayVideo = !VideoManager.Instance.IsDisplayVideo;
        VideoManager.Instance.DisplayVideo(VideoManager.Instance.IsDisplayVideo);
    }

    void ToggleModeShowingVideo()
    {
        VideoManager.Instance.IsShowingFullScreen = !VideoManager.Instance.IsShowingFullScreen;
        VideoManager.Instance.ChangeVideoView(VideoManager.Instance.IsShowingFullScreen);
    }

    void HanldeDisplayAudio()
    {
        AudioManager.Instance.IsDisplayAudio = !AudioManager.Instance.IsDisplayAudio;
        AudioManager.Instance.DisplayAudioWithObject(AudioManager.Instance.IsDisplayAudio);
    }

    void ToggleStatusPlayingAudio()
    {
        AudioManager.Instance.IsPlayingAudio = !AudioManager.Instance.IsPlayingAudio;
        AudioManager.Instance.ControlAudio(AudioManager.Instance.IsPlayingAudio);
    }

    void ToggleStatusModeAR()
    {
        ModeManager.Instance.Mode = ModeManager.MODE_EXPERIENCE.MODE_AR;
        ModeManager.Instance.ViewScreenWithMode(ModeManager.Instance.Mode);
    }

    void ToggleStatusMode3D()
    {
        ModeManager.Instance.Mode = ModeManager.MODE_EXPERIENCE.MODE_3D;
        ModeManager.Instance.ViewScreenWithMode(ModeManager.Instance.Mode);
    }

    void OnResetStatusFeature()
    {
        LabelManager.Instance.IsShowingLabel = false;
        LabelManager.Instance.HandleLabelView(LabelManager.Instance.IsShowingLabel);

        SeparateManager.Instance.IsSeparating = false;
        SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);

        TouchManager.Instance.IsClickedHoldBtn = false;

        MediaManager.Instance.StopMedia();
    }

    void OnARPlaceObject(Vector3 position, Quaternion rotation)
    {
        ARUIManager.Instance.PlaceARObject(position, rotation, true);
    }
}
