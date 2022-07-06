using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using EasyUI.Toast;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Voice.PUN;

[RequireComponent(typeof(LoadingManager))]
public class MeetingJoiningHandler : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public Button backBtn;
    public Button joinMeetingBtn;
    public Button resetMeetingCodeBtn;
    public InputField meetingCodeInputField;
    public GameObject inputMeetingCodeComponent;
    public GameObject meetingLobbyComponent;
    public Text lobbyMeetingCodeTxt;
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare flags for error connection handler
    /// </summary>
    private int numberOfConnectionAttempt = 0;
    private int numberOfJoiningAttempt = 0;
    private bool isForcedDisconnected = false;

    // Start is called before the first frame update
    void Start()
    {
        InitScreen();
        InitEvents();
        ConnectToMultiplayerServer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup screen orientation and visibility of status bar, navigation bar
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
        resetMeetingCodeBtn.onClick.AddListener(ResetMeetingCodeHandler);
        meetingCodeInputField.onValueChanged.AddListener(OnMeetingCodeChangedHandler);
        joinMeetingBtn.onClick.AddListener(JoinMeetingHandler);
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
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = PlayerPrefs.GetString("user_name"); // Add user_name key to config
        AllowJoinMeeting();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Enable join meeting
    /// </summary>
    void AllowJoinMeeting()
    {
        LoadingManager.Instance.HideLoading();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Disable join meeting
    /// </summary>
    void DisallowJoinMeeting()
    {
        LoadingManager.Instance.ShowLoading(MeetingConfig.connecting);
    }

    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update visibility of reset value UI according to input value length
    /// </summary>
    void OnMeetingCodeChangedHandler(string meetingCodeValue)
    {
        joinMeetingBtn.interactable = meetingCodeValue.Length == MeetingConfig.meetingCodeLength;
        resetMeetingCodeBtn.gameObject.SetActive(meetingCodeValue.Length > 0);
    }

    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Back to previous scene (HOME scene) from MEETING_JOINING scene
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
        SceneManager.LoadScene(SceneConfig.home_user);
    }


    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Reset meeting code input value
    /// </summary>
    void ResetMeetingCodeHandler()
    {
        meetingCodeInputField.text = "";
    }
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Join meeting by code
    /// </summary>
    void JoinMeetingHandler()
    {
        PhotonNetwork.JoinRoom(meetingCodeInputField.text);
        LoadingManager.Instance.ShowLoading(MeetingConfig.joinMeeting);
    }
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup photon local player (voice) and enter lobby when member joined room successfully
    /// </summary>
    public override void OnJoinedRoom()
    {
        InitVoices();
        EnterLobbyMeeting();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init voice component
    /// </summary>
    void InitVoices()
    {
        PhotonNetwork.LocalPlayer.MuteByHost(MeetingConfig.isMutedByHostDefault);
        PhotonNetwork.LocalPlayer.Mute();
    }
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Rejoin meeting room after falied joining
    /// </summary>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        numberOfJoiningAttempt++;
        if (numberOfJoiningAttempt >= MeetingConfig.maxNumberOfJoiningAttempt)
        {
            // Show notification and reset joining after many times of rejoining
            numberOfJoiningAttempt = 0;
            LoadingManager.Instance.HideLoading();
            Toast.Show(MeetingConfig.invalidMeetingCodeMessage, MeetingConfig.longToastDuration);
        }
        else
        {
            JoinMeetingHandler();
        }
    }

    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Enter lobby for meeting
    /// </summary>
    void EnterLobbyMeeting()
    {
        inputMeetingCodeComponent.SetActive(false);
        meetingLobbyComponent.SetActive(true);
        lobbyMeetingCodeTxt.text = lobbyMeetingCodeTxt.text + " " + meetingCodeInputField.text;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set up UI for joining meeting
    /// </summary>
    void InitMeetingJoining()
    {
        inputMeetingCodeComponent.SetActive(true);
        meetingLobbyComponent.SetActive(false);
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
        InitMeetingJoining();
        DisallowJoinMeeting();
        numberOfConnectionAttempt++;
        if (numberOfConnectionAttempt >= MeetingConfig.maxNumberOfConnecetionAttempt)
        {
            numberOfConnectionAttempt = 0;
            Toast.Show(MeetingConfig.errorConnectionMessage, MeetingConfig.toastDuration);
        }
        ConnectToMultiplayerServer();
    }
}