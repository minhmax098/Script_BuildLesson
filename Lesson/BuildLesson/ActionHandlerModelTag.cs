using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; 

namespace BuildLesson
{
    public class ActionHandlerModelTag : MonoBehaviour
    {
        private static ActionHandlerModelTag instance; 
        public static ActionHandlerModelTag Instance
        {
            get
            {
                if (instance = null)
                {
                    instance = FindObjectOfType<ActionHandlerModelTag>(); 
                }
                return instance;
            }
        }

        public GameObject btnSave; 
        public GameObject btnCancel;

        void Update()
        {
            SetActions();
        }
        void SetActions()
        {
            btnSave.GetComponent<Button>().onClick.AddListener(SaveBtnHandler);
            btnCancel.GetComponent<Button>().onClick.AddListener(CancelBtnHandler);
        }
        void SaveBtnHandler()
        {
            Debug.Log("Save tag: ");
        }
        void CancelBtnHandler()
        {
            // Debug.Log("gameObject name: " + gameObject.name);
            Destroy(gameObject); 
        }
    }
}
