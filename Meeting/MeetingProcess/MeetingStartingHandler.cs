using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using EasyUI.Toast;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Voice.PUN;

[RequireComponent(typeof(LoadingManager))]
public class MeetingStartingHandler : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public Button backBtn;
    public Button copyMeetingCodeBtn;
    public Button launchMeetingBtn;
    public Text meetingCodeTxt;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare flags for error connection handler
    /// </summary>
    private int numberOfConnectionAttempt = 0;
    private int numberOfStartingAttempt = 0;
    private bool isForcedDisconnected = false;

    // Start is called before the first frame update
    void Start()
    {
        InitScreen();
        InitEvents();
        GenerateMeetingCode();
        ConnectToMultiplayerServer();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: setup screen orientation and visibility of status bar, navigation bar
    /// </summary>
    void InitScreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add event triggers for UI
    /// </summary>
    void InitEvents()
    {
        backBtn.onClick.AddListener(BackScreenHandler);
        copyMeetingCodeBtn.onClick.AddListener(CopyMeetingCodeHandler);
        launchMeetingBtn.onClick.AddListener(StartMeetingRoom);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Back to previous scene (LESSON scene) from MEETING_STARTING scene
    /// </summary>
    void BackScreenHandler()
    {
        isForcedDisconnected = true;
        StartCoroutine(DisconnectPhotonServer());
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Disconnect from photon server
    /// </summary>
    /// <returns></returns>
    public IEnumerator DisconnectPhotonServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                // repeat if the status is still connected
                yield return null;
            }
        }
        SceneManager.LoadScene(SceneConfig.lesson);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Copy meeting code to clipboard to share with other users
    /// </summary>
    void CopyMeetingCodeHandler()
    {
        Helper.CopyToClipboard(meetingCodeTxt.text);
        Toast.Show(MeetingConfig.successCopyMeetingCodeMessage, 2f);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show meeting code to share with other users
    /// </summary>
    void GenerateMeetingCode()
    {
        meetingCodeTxt.text = GetMeetingCodeFromServer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Generate meeting code 
    /// Note: This is for test only. Need call API to get meeting code
    /// </summary>
    /// <returns></returns>
    string GetMeetingCodeFromServer()
    {
        // Neet call API
        return (Random.Range(100000000, 999999999)).ToString();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Connect to photon server
    /// </summary>
    void ConnectToMultiplayerServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set up infos for photon session and create meeting room
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NickName = PlayerPrefs.GetString("user_name"); // Add user_name key to config
        CreateMeetingRoom();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Create meeing room
    /// </summary>
    void CreateMeetingRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PlayerTtl = MeetingConfig.meetingPlayerTTL;
        roomOptions.EmptyRoomTtl = MeetingConfig.meetingRoomEmptyTTL;
        PhotonNetwork.JoinOrCreateRoom(meetingCodeTxt.text, roomOptions, TypedLobby.Default);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init meeting room properties, photon local player (is organizer, voice) and UI for starting meeting
    /// </summary>
    public override void OnJoinedRoom()
    {
        InitVoices();
        InitRoomProperties();
        AllowStartMeeting();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init voice component
    /// </summary>
    void InitVoices()
    {
        PhotonNetwork.LocalPlayer.SetOrganizer();
        PhotonNetwork.LocalPlayer.MuteByHost(MeetingConfig.isMutedByHostDefault);
        PhotonNetwork.LocalPlayer.Mute();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init default values for meeting room properties
    /// </summary>
    void InitRoomProperties()
    {
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add(MeetingConfig.IS_MAKING_XRAY_KEY, false);
        roomProperties.Add(MeetingConfig.IS_SEPARATING_KEY, false);
        roomProperties.Add(MeetingConfig.IS_SHOWING_LABEL_KEY, false);
        roomProperties.Add(MeetingConfig.IS_CLICKED_HOLD_KEY, false);
        roomProperties.Add(MeetingConfig.IS_CLICKED_GUIDE_BOARD_KEY, false);
        roomProperties.Add(MeetingConfig.HOST_ACTOR_NUMBER_KEY, PhotonNetwork.LocalPlayer.ActorNumber);
        roomProperties.Add(MeetingConfig.IS_MUTING_ALL_KEY, true);
        roomProperties.Add(MeetingConfig.CURRENT_OBJECT_NAME_KEY, "");
        roomProperties.Add(MeetingConfig.EXPERIENCE_MODE_KEY, ModeManager.MODE_EXPERIENCE.MODE_3D);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties, null, null);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Enable start meeting
    /// </summary>
    void AllowStartMeeting()
    {
        launchMeetingBtn.interactable = true;
        LoadingManager.Instance.HideLoading();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Disable start meeting
    /// </summary>
    void DisallowStartMeeting()
    {
        launchMeetingBtn.interactable = false;
        LoadingManager.Instance.ShowLoading(MeetingConfig.connecting);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Start meeting
    /// </summary>
    void StartMeetingRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        LoadingManager.Instance.ShowLoading(MeetingConfig.startingMeeting);
        PhotonNetwork.LoadLevel(SceneConfig.meetingExperience);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Reconnect to photon server after disconnection
    /// </summary>
    /// <param name="cause">Reason of disconnection</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (isForcedDisconnected)
        {
            // if isForcedDisconnected == true (user wants to disconnect) -> do not reconnect
            return;
        }
        DisallowStartMeeting();
        numberOfConnectionAttempt++;
        if (numberOfConnectionAttempt >= MeetingConfig.maxNumberOfConnecetionAttempt)
        {
            numberOfConnectionAttempt = 0;
            Toast.Show(MeetingConfig.errorConnectionMessage, MeetingConfig.toastDuration);
        }
        ConnectToMultiplayerServer();
    }
}

