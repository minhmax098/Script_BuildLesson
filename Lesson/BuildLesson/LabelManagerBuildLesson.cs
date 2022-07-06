using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

namespace BuildLesson 
{
    public class LabelManagerBuildLesson : MonoBehaviour
    {
        private static LabelManagerBuildLesson instance; 
        public static LabelManagerBuildLesson Instance
        {
            get 
            {
                if (instance == null)
                {
                    // centerPosition = CalculateCentroid(ObjectManager.Instance.OriginObject);
                    instance = FindObjectOfType<LabelManagerBuildLesson>(); 
                }
                return instance; 
            }
        }
        private int calculatedSize = 20;
        public GameObject btnLabel;
        
        private List<Vector3> pointPositions = new List<Vector3>();
        public List<GameObject> listLabelObjects = new List<GameObject>(); 
        public List<GameObject> listLabelObjectsOnEditMode = new List<GameObject>(); 

        private bool isLabelOnEdit = false;
        private bool isShowingLabel = true;

        public bool IsLabelOnEdit { get; set;}
        
        public bool IsShowingLabel 
        {    
            get
            {
                return isShowingLabel;
            }
            set
            {
                Debug.Log("LabelManagerBuildLesson IsShowingLabel call"); 
                isShowingLabel = value;
                btnLabel.GetComponent<Image>().sprite = isShowingLabel ? Resources.Load<Sprite>(PathConfig.LABEL_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.LABEL_UNCLICK_IMAGE);
            }
        }

        void Start()
        {
            InitUI();
        }

        void InitUI()
        {
            btnLabel = GameObject.Find("BtnLabel");
        }

        public void Update()
        {
            pointPositions.Add(transform.position);
        }
        
        private static Vector3 centerPosition;

        public void updateCenterPosition()
        {
            Debug.Log("After init object: " + ObjectManager.Instance.OriginObject);
            centerPosition = CalculateCentroid(ObjectManager.Instance.OriginObject);
        }

        private string formatString(string inputString, int maxSize)
        {
            if (inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "...";
            }
            return inputString;
        }

        public void SetLabel(GameObject currentObject, Vector3 tapPoint, GameObject parentObject, Vector3 rootPosition, GameObject label)
        {
            GameObject line = label.transform.GetChild(0).gameObject; 
            GameObject labelName = label.transform.GetChild(1).gameObject;
            // labelName.gameObject.GetComponent<InputField>().text = formatString(labelName, calculatedSize);
            // labelName.transform.GetChild(1).GetComponent<TextMeshPro>().text = currentObject.name;
            Bounds parentBounds = GetParentBound(parentObject, rootPosition);
            // GameObject s;
            // s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // s.transform.position = rootPosition;
            // Bounds objectBounds = currentObject.GetComponent<Renderer>().bounds;
            // New get bounds in local (in currentObject coordinate system)
            // Bounds objectBounds = currentObject.GetComponent<Renderer>().localBounds;
            // dir is the world vector3 present the direction between center of the game object and the center of currentObject
            // Vector3 dir = currentObject.transform.position - rootPosition; // Old 
            Vector3 dir = currentObject.transform.InverseTransformDirection(tapPoint - rootPosition) ; // New
            // Vector3 dir = tapPoint - rootPosition;
            Debug.Log("Magnitude: " + parentBounds.max.magnitude); // Magnitude not a constant
            // Dong ni day
            // labelName.transform.localPosition = 1 / parentObject.transform.localScale.x * parentBounds.max.magnitude * dir.normalized;
            labelName.transform.localPosition = 1 / parentObject.transform.localScale.x * parentBounds.max.magnitude * dir.normalized;
            line.GetComponent<LineRenderer>().useWorldSpace = false;
            line.GetComponent<LineRenderer>().widthMultiplier = 0.25f * parentObject.transform.localScale.x;  // 0.2 -> 0.05 then 0.02 -> 0.005
            line.GetComponent<LineRenderer>().SetVertexCount(2);
            line.GetComponent<LineRenderer>().SetPosition(0, currentObject.transform.InverseTransformPoint(tapPoint));
            line.GetComponent<LineRenderer>().SetPosition(1, labelName.transform.localPosition);
            line.GetComponent<LineRenderer>().SetColors(Color.black, Color.black);
            // Debug.Log("Label name x: " + labelName.transform.localPosition.x + ", Label name y: " + labelName.transform.localPosition.y + ", Label name z: " + labelName.transform.localPosition.z);
            // Debug.Log("Line x: " + line.transform.localPosition.x + ", Line y: " + line.transform.localPosition.y + ", Line z: " + labelName.transform.localPosition.z);
        }

        private static Vector3 CalculateCentroid(GameObject obj)
        {
            Debug.Log("object name: " + obj.name);
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

        public IEnumerator SaveCoordinate(int lessonId, int modelId, string labelName, Vector3 coordinate, string level)
        {
            var webRequest = new UnityWebRequest(APIUrlConfig.CreateModelLabel, "POST");
            string requestBody = "{\"lessonId\": \"" + lessonId + "\", \"modelId\": \"" + modelId + "\" , \"labelName\": \"" + labelName + "\", \"coordinates\" :{\"x\": "+ coordinate.x + ",\"y\": " + coordinate.y + ",\"z\": "+ coordinate.z +"}, \"level\":  \"" + level + "\"}";
            Debug.Log("Test create label Test data: " + requestBody);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(requestBody);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {   
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received
                if (webRequest.isDone)
                {
                    Debug.Log("Test create Label done: ");
                    // Use JsonTool to convert from JSONG String to Object
                    string jsonResponse = webRequest.downloadHandler.text;
                    Debug.Log("Test create Label Json response: " + jsonResponse);
                    //  IEnumerator not CoordinateJson
                    CoordinateJson rs =  JsonUtility.FromJson<CoordinateJson>(jsonResponse);
                    Debug.Log("Test create Label: " + rs.data.labelId);
                    // Add coresponding labelId with the addedTags 
                    TagHandler.Instance.AddLabelId(rs.data.labelId);
                }
            }
        }

        public string getIndexGivenGameObject(GameObject rootObject, GameObject targetObject)
        {
            var result = new System.Text.StringBuilder();
            while(targetObject != rootObject)
            {
                result.Insert(0, targetObject.transform.GetSiblingIndex().ToString());
                result.Insert(0, "-");
                targetObject = targetObject.transform.parent.gameObject;
            }
            result.Insert(0, "0");
            return result.ToString();
        }

        public void HandleLabelView(bool currentLabelStatus) 
        {
            IsShowingLabel = currentLabelStatus;
            ShowHideLabels(IsShowingLabel);
        }

        private void ShowHideLabels(bool isShowing)
        {
            foreach(GameObject label in listLabelObjects)
            {
                label.SetActive(isShowingLabel);
            }    
        }
    }
}
