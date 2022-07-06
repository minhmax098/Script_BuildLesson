using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeftPanelHandler : MonoBehaviour
{
    [SerializeField]
    Animator statusAnimator;
    private GameObject meetingManager;
    void Start()
    {
        InitUI();
    }
    void Update()
    {
    
    }
    void InitUI() 
    {
        meetingManager = GameObject.Find("MeetingManager");
    }
    public void ShowLeftPanel() 
    {
        statusAnimator.SetBool(AnimatorConfig.showLeftPanel, true);
    }
    public void HideLeftPanel() 
    {
        statusAnimator.SetBool(AnimatorConfig.showLeftPanel, false);
    }
    public void JoinMeeting()
    {
        SceneManager.LoadScene(SceneConfig.meetingJoining);
    } 
}
