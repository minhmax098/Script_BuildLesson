using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;

// Cai nay la global Class
public class LabelManager : MonoBehaviour
{
    private static LabelManager instance; 
    public static LabelManager Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LabelManager>(); 
            }
            return instance; 
        }
    }
    
    private const float LONG_LINE_FACTOR = 1f; // Between 0 and 1
    private int childCount;

    private Vector3 centerPosition;
    
    public List<GameObject> listLabelObjects = new List<GameObject>(); 
    public List<Transform> listChildrenTransform = new List<Transform>();
    public Button btnLabel;
    private bool isShowingLabel;
    public bool IsShowingLabel 
    { 
        get
        {
            return isShowingLabel;
        }
        set
        {
            isShowingLabel = value;
            btnLabel.GetComponent<Image>().sprite = isShowingLabel ? Resources.Load<Sprite>(PathConfig.LABEL_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.LABEL_UNCLICK_IMAGE);
        }
    }

    public void HandleLabelView(bool currentLabelStatus)
    {
        IsShowingLabel = currentLabelStatus;
        if (IsShowingLabel)
        {
            CreateLabel();
        }
        else
        {
            DestroyLabel();
        }
    }

    public void CreateLabel()
    {
        // childCount = ObjectManager.Instance.CurrentObject.transform.childCount;
        childCount = ObjectManager.Instance.OriginObject.transform.childCount;
        if (childCount == 0)
        {
            return;
        }
        // centerPosition = CalculateCentroid(ObjectManager.Instance.CurrentObject);
        centerPosition = CalculateCentroid(ObjectManager.Instance.OriginObject);
        Debug.Log("Centerposition x: " + centerPosition.x + ", Centerposition y: " + centerPosition.y + ", Centerposition z: " + centerPosition.z);
        listChildrenTransform.Clear();
        // GetLastChildren(ObjectManager.Instance.CurrentObject.transform);
        GetLastChildren(ObjectManager.Instance.OriginObject.transform);
        foreach (Transform child in listChildrenTransform)
        {
            GameObject labelObject = Instantiate(Resources.Load(PathConfig.MODEL_TAG) as GameObject);
            labelObject.transform.localScale *=  ObjectManager.Instance.OriginScale.x / ObjectManager.Instance.OriginObject.transform.localScale.x ;
            
            TagHandler.Instance.AddTag(labelObject);
            labelObject.transform.SetParent(child.gameObject.transform, false); 
            
            labelObject.transform.localPosition = new Vector3(0, 0, 0);
            SetLabel(child.gameObject, ObjectManager.Instance.OriginObject, centerPosition, labelObject);
            listLabelObjects.Add(labelObject);
        }
    }

    public void SetLabel(GameObject currentObject, GameObject parentObject, Vector3 rootPosition, GameObject label)
    {
        GameObject line = label.transform.GetChild(0).gameObject; 
        GameObject labelName = label.transform.GetChild(1).gameObject;
            
        labelName.transform.GetChild(0).GetComponent<TextMeshPro>().text = currentObject.name;

        Bounds parentBounds = GetParentBound(parentObject, rootPosition);
        Bounds objectBounds = currentObject.GetComponent<Renderer>().bounds;
        Vector3 dir = currentObject.transform.localPosition - rootPosition; 
    
        Debug.Log("Magnitude: " + parentBounds.max.magnitude); 
        Debug.Log("Active scene: " + SceneManager.GetActiveScene().name);

        if (SceneManager.GetActiveScene().name == "Experience")
        {
            labelName.transform.localPosition = 1 / parentObject.transform.localScale.x * parentBounds.max.magnitude * dir.normalized;
        }
        else 
        {
            labelName.transform.localPosition = 2f * parentBounds.max.magnitude * dir.normalized;
        }
        // labelName.transform.localPosition = 5f * parentBounds.max.magnitude * dir.normalized;
        line.GetComponent<LineRenderer>().useWorldSpace = false;
        line.GetComponent<LineRenderer>().widthMultiplier = 0.25f * parentObject.transform.localScale.x;
        line.GetComponent<LineRenderer>().SetVertexCount(2);
        line.GetComponent<LineRenderer>().SetPosition(0, currentObject.transform.InverseTransformPoint(objectBounds.center));
        line.GetComponent<LineRenderer>().SetPosition(1, labelName.transform.localPosition);
        Debug.Log("Label name x: " + labelName.transform.localPosition.x + ", Label name y: " + labelName.transform.localPosition.y + ", Label name z: " + labelName.transform.localPosition.z);
        Debug.Log("Line x: " + line.transform.localPosition.x + ", Line y: " + line.transform.localPosition.y + ", Line z: " + labelName.transform.localPosition.z);
        line.GetComponent<LineRenderer>().SetColors(Color.black, Color.black);
    }

    private void GetLastChildren(Transform childTranform)
    {
        if (childTranform.parent && childTranform.childCount == 0)
        {
            listChildrenTransform.Add(childTranform);
        }
        else
        {
            foreach (Transform child in childTranform.transform)
            {
                GetLastChildren(child);
            }
        }
    }

    private Vector3 CalculateCentroid(GameObject obj)
    {
        Transform[] children;
        Vector3 centroid = new Vector3(0, 0, 0);
        children = obj.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if(child != obj.transform)
            {
                centroid += child.transform.position;
            }  
        }
        centroid /= (children.Length - 1);
        return centroid;
    }

    public Bounds GetParentBound(GameObject parentObject, Vector3 center)
    {
        foreach (Transform child in parentObject.transform)
        {
            center += child.gameObject.GetComponent<Renderer>().bounds.center;
        }

        center /= parentObject.transform.childCount;
        
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach(Transform child in parentObject.transform)
        {
            bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        return bounds;
    }

    public void AdjustLinePosition(){
        if (listLabelObjects.Count > 0)
        {
            for (int i = 0; i < listLabelObjects.Count; i++)
            {
                // Recalculate the bound 
                GameObject currChild = listChildrenTransform[i].gameObject;
                Bounds childBound = currChild.GetComponent<Renderer>().bounds;
                GameObject line = listLabelObjects[i].transform.GetChild(0).gameObject;
                line.GetComponent<LineRenderer>().SetPosition(0, currChild.transform.InverseTransformPoint(childBound.center));
            }
        }
    }

    public void DestroyLabel()
    {
        foreach(GameObject label in listLabelObjects)
        {
            Destroy(label);
        }
        listLabelObjects.Clear();
        TagHandler.Instance.DeleteTags();
    }
}
