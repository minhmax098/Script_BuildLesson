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

namespace XRLibrary
{
    public class LoadScene : MonoBehaviour, IEndDragHandler
    {
        public GameObject contentForOrgansListLessons; 
        public InputField searchBox; 
        public Lesson[] xrLibraryLesson; 
        // insert into 3 records
        public GameObject sumLesson; 
        public GameObject xBtn; 
        public GameObject searchBtn; 
        private string processedString; 
        private string userInput;
        private char[] charsToTrim = { '*', '.', ' '};
        public ScrollRect SR;
        // variable control content 
        private int offset = 0; // This is current page value (offset = 0.. pagesize -1)
        private int limit = 21; // this is pagesize in API, control number of maximum item in each page, this is constant 
        // Total page this is also a critical variable, will use it to control our API call this is equivalent to totalPage in API 
        private int totalPage;
        public AllXRLibrary listLessons;
        public GameObject spinner;
        // private UnityWebRequest request;
        private int caretPos = 0;
        private int selectionPos = 0;
        public int calculatedSize = 35; 
        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            
            searchBtn.SetActive(true); 
            xBtn.SetActive(false);
            xBtn.transform.GetComponent<Button>().onClick.AddListener(ClearInput); 

            if(SceneNameManager.prevScene != SceneConfig.xrLibrary_edit)
            {
                searchBox.Select();
                searchBox.ActivateInputField();
                userInput = PlayerPrefs.GetString("user_input");
                searchBox.text = userInput;
            }
            searchBox.onValueChanged.AddListener(UpdateList); 
            listLessons = LoadData.Instance.GetListLessons(userInput, offset, limit); 
            totalPage = listLessons.meta.totalPage;
            
            spinner.SetActive(false);
        }
        void Update()
        {
            
        }
        private string formatString(string inputString, int maxSize)
        {
            if (inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "...";
            }
            return inputString;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            // if (SR.verticalNormalizedPosition >= 0.95f) {
            //     Debug.Log("Current page: " + offset + "Total page: " + totalPage);
            //     if (offset > 0)
            //     {
            //         Debug.Log("Go to previous page");
            //         offset -= 1;
            //         listLessons = LoadData.Instance.GetListLessons(searchBox.text, offset, limit); 
            //         // We not use this later
            //         StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true));
            //     }
            // }
            if (SR.verticalNormalizedPosition <= 0.05f){
                // Update current page 
                Debug.Log("Current page: " + offset + "Total page: " + totalPage);
                if (offset < totalPage - 1){
                    Debug.Log("Go to next page");
                    offset += 1;
                    // Get current text inside Search box then pass ...
                    listLessons = LoadData.Instance.GetListLessons(searchBox.text, offset, limit); 
                    StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, false));
                }     
            }
        }

        void LateUpdate()
        {
            searchBox.MoveTextEnd(true);
        }

        void UpdateList(string data)
        {
            processedString = Regex.Replace(data, @"\s+", " ").ToLower().Trim(charsToTrim); 
            // processedString = data.Trim(charsToTrim).ToLower(); 
            if (processedString == "")
            {
                // When input is empty
                xBtn.SetActive(false); 
                searchBtn.SetActive(true); 
            }
            else 
            {
                // When input is not empty
                xBtn.SetActive(true); 
                searchBtn.SetActive(false); 
            }
            offset = 0;
            listLessons = LoadData.Instance.GetListLessons(processedString, offset, limit); 
            // Update totalPage too when user change the search input
            // When change input, we must renew lessons 
            totalPage = listLessons.meta.totalPage;
            StopAllCoroutines();
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true));
        }
        
        void ClearInput()
        {
            searchBox.GetComponent<InputField>().SetTextWithoutNotify(""); 
            xBtn.SetActive(false); 
            searchBtn.SetActive(true); 
            
            offset = 0; 
            listLessons = LoadData.Instance.GetListLessons("", offset, limit);
            // xrLibraryLesson = LoadData.Instance.GetListLessons("", offset, limit).data;

            // When clear input, we need renew lesssons
            StopAllCoroutines();
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true));
        }
        IEnumerator loadLessons(Lesson[] lessons, int totalLessons, bool isRenewLessons)
        {
            if (isRenewLessons){
                Debug.Log("destroy old object");
                SR.verticalNormalizedPosition = 1f;
                // When need renew then we delete the old component FIRST
                 foreach(Transform child in contentForOrgansListLessons.transform){
                    GameObject.Destroy(child.gameObject);
                }
            }
            if (totalLessons == 0)
            {
                sumLesson.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"No results found."; 
            }
            else
            {
                sumLesson.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{totalLessons} lessons"; 
            }

            var requests = new List<UnityWebRequestAsyncOperation>(lessons.Length);
            var organLessonList = new List<GameObject>(lessons.Length);

            spinner.SetActive(true);
            // Start request and init the UI 
            foreach(Lesson lesson in lessons)
            {
                string imageUri = String.Format(APIUrlConfig.LoadLesson, lesson.lessonThumbnail);
                var www = UnityWebRequestTexture.GetTexture(imageUri);
                requests.Add(www.SendWebRequest());
                
                GameObject xrLibraryLesson = Instantiate(Resources.Load(ItemConfig.totalLesson2) as GameObject, contentForOrgansListLessons.transform); 
                xrLibraryLesson.GetComponent<RectTransform>().localScale = Vector3.one;
                
                organLessonList.Add(xrLibraryLesson);

                xrLibraryLesson.name = lesson.lessonId.ToString(); 
                xrLibraryLesson.transform.GetChild(1).gameObject.GetComponent<Text>().text = formatString(lesson.lessonTitle, calculatedSize);
                xrLibraryLesson.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = lesson.viewed.ToString();            
                xrLibraryLesson.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(lesson.lessonId));
            }
            // Wait for all requests parallel
            yield return new WaitUntil(() => AllRequestsDone(requests));
            // Use this result to update the UI 
            HandleAllRequestsWhenFinished(requests, organLessonList);
        }

        private bool AllRequestsDone(List<UnityWebRequestAsyncOperation> requests)
        {
            return requests.All(r => r.isDone);
        }

        private void HandleAllRequestsWhenFinished(List<UnityWebRequestAsyncOperation> requests, List<GameObject>organLessonList)
        {
            for(var i = 0; i < requests.Count; i++)
            {
                var www = requests[i].webRequest;
                if(www.isNetworkError || www.isHttpError) 
                {
                    // Don't modify any thing
                }
                else 
                {
                    Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                    organLessonList[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = sprite;  
                    organLessonList[i].transform.GetChild(4).gameObject.SetActive(false);
                }
            }
            spinner.SetActive(false);
        }
    }
}
