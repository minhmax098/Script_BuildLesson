using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagHandler : MonoBehaviour
{
    private static TagHandler instance;
    public static TagHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TagHandler>();
            }
            return instance;
        }
    }

    public List<GameObject> addedTags = new List<GameObject>();

    void Update()
    {
        if (LabelManager.Instance.IsShowingLabel)
        {
            OnMove();
        }
    }

    public void AddTag(GameObject tag)
    {
        addedTags.Add(tag);
    }

    public void DeleteTags()
    {
        addedTags.Clear();
    }

    public void OnMove()
    {
        foreach (GameObject addedTag in addedTags)
        {
            if (addedTag != null)
            {
                DenoteTag(addedTag);
                MoveTag(addedTag);
            }
        }
    }

    public void DenoteTag(GameObject addedTag)
    {
        if (addedTag.transform.GetChild(1).transform.position.z > 1f)
        {
            addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
            addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = false;
            addedTag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = true;
            addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = true;
            addedTag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void MoveTag(GameObject addedTag)
    {
        addedTag.transform.GetChild(1).transform.LookAt(addedTag.transform.GetChild(1).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        addedTag.transform.GetChild(1).GetChild(0).transform.LookAt(addedTag.transform.GetChild(1).GetChild(0).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
