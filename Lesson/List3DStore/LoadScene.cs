using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.Networking; 
using System.Threading.Tasks;
using System.Linq; 

namespace List3DStore
{
    public class LoadScene : MonoBehaviour, IEndDragHandler
    {
        public GameObject contentForOrgansListLessons; 
        public InputField searchBox; 
        public Lesson[] organLesson; 
        public GameObject xBtn; 
        public GameObject searchBtn; 
        private char[] charsToTrim = { '*', '.', ' '};
        public ScrollRect content; 
        private int offset = 0; // this is curent page value, offset = 0...pageSize - 1 
        private int limit = 21; // this is pageSize in API 
        private int totalPage; 
        private GridLayoutGroup layoutGroup;
        private int type = -1;  // Initial value
        private int calculatedSize = 30;

        private Dictionary<string, int> lookUpTable = new Dictionary<string, int>()
        {
            {"AllBtn", -1 },
            {"MystoreBtn", 1},
            {"TopModelBtn", 2}
        };
        void Start()
        {
            if (TabManager.selectedTab != null)
            {
                Debug.Log("Tab active name: " + TabManager.selectedTab.name);
                type = lookUpTable[TabManager.selectedTab.name]; 
                Debug.Log("Type has changed to: " + type);
            }
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            
            searchBtn.SetActive(true); 
            xBtn.SetActive(false);
            xBtn.transform.GetComponent<Button>().onClick.AddListener(ClearInput);

            All3DModel listLessons; 
            listLessons = LoadData.Instance.GetList3DModel(type, "", offset, limit); 
            totalPage = listLessons.meta.totalPage; 
            
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true));

            searchBox.GetComponent<InputField>().onValueChanged.AddListener(UpdateList); 
            layoutGroup = contentForOrgansListLessons.GetComponent<GridLayoutGroup>(); 
        }
        private string formatString(string inputString, int maxSize)
        {
            if(inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "..."; 
            }
            return inputString;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            All3DModel listLessons; 
            Debug.Log("Stop dragging: " + this.name); 
            if (content.verticalNormalizedPosition <= 0.05f)
            {
                // Update current page
                Debug.Log("Currrent page: " + offset + "Total page: " + totalPage);
                if (offset < totalPage -1)
                {
                    Debug.Log("Go to the next page"); 
                    offset += 1; 
                    // Get current text inside search box then pass ...
                    listLessons = LoadData.Instance.GetList3DModel(type, searchBox.GetComponent<InputField>().text, offset, limit);
                    StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, false)); 
                }
            }
        }
        void LateUpdate()
        {
            searchBox.MoveTextEnd(true); 
        }
        void Update() 
        {
            if (TabManager.selectedTab != null)
            {
                Debug.Log("Tab active name: " + TabManager.selectedTab.name);
            }
        }

        void UpdateList(string data)
        {
            string processedString = Regex.Replace(data, @"\s+", " ").ToLower().Trim(charsToTrim);
            if (processedString == "")
            {
                // when the input is empty
                xBtn.SetActive(false);
                searchBtn.SetActive(true); 
            }
            else
            {
                // when the input is not empty
                xBtn.SetActive(true); 
                searchBtn.SetActive(false);
            }
            offset = 0;
            All3DModel listLessons = LoadData.Instance.GetList3DModel(type, processedString, offset, limit);

            totalPage = listLessons.meta.totalPage; 
            StopAllCoroutines(); 
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true)); 
        }

        void ClearInput()
        {
            All3DModel listLessons; 
        }
        IEnumerator loadLessons(List3DModel[] models, int totalLessons, bool isRenewLesssons)
        {
            if (isRenewLesssons)
            {
                content.verticalNormalizedPosition = 1f; 
                foreach(Transform child in contentForOrgansListLessons.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
                var requests = new List<UnityWebRequestAsyncOperation>(models.Length);
                var organLessonList = new List<GameObject>(models.Length); 
                foreach (List3DModel model in models)
                {
                    string imageUri = String.Format(APIUrlConfig.LoadLesson, model.modelThumbnail);
                    var www = UnityWebRequestTexture.GetTexture(imageUri); 
                    requests.Add(www.SendWebRequest()); 

                    GameObject organLesson = Instantiate(Resources.Load(ItemConfig.ListModel) as GameObject, contentForOrgansListLessons.transform); 
                    organLessonList.Add(organLesson);

                    organLesson.name = model.modelId.ToString(); 
                    organLesson.transform.GetChild(1).gameObject.GetComponent<Text>().text = formatString(model.modelName, calculatedSize); 
                    // organLesson.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = model.viewed.ToString(); 
                    organLesson.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(model.modelId)); 
                }
                // wait for all request parallel
                yield return new WaitUntil(() => AllRequestsDone(requests)); 
                // Evaluate all result
                HandleAllRequestsWhenFinished(requests, organLessonList); 
            }

        private bool AllRequestsDone(List<UnityWebRequestAsyncOperation> requests)
        {
            return requests.All(r => r.isDone); 
        }
        private void HandleAllRequestsWhenFinished(List<UnityWebRequestAsyncOperation> requests, List<GameObject> organLessonList)
        {
            for (var i=0; i < requests.Count; i++)
            {
                var www = requests[i].webRequest; 
                if (www.isNetworkError || www.isHttpError)
                {

                }
                else 
                {
                    Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture; 
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height /2)); 
                    organLessonList[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = sprite; 
                    organLessonList[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }   
}
