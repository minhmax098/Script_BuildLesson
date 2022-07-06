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

namespace ListOrgan 
{
    public class LoadScene : MonoBehaviour, IEndDragHandler
    {
        public GameObject contentForOrgansListLessons; 
        public InputField searchBox; 
        public GameObject organTitle; 
        public Lesson[] organLesson; 
        // add 3 record
        public GameObject sumLesson; 
        public GameObject xBtn; 
        public GameObject searchBtn;
        private char[] charsToTrim = { '*', '.', ' '};
        public ScrollRect content;
        // Variable control content 
        private int offset = 0;  // This is curent page value (offset = 0.. pagesize-1)
        private int limit = 21;  // this is pageSize in API, control number of maximum item in each page, this is constant
        // Total page this is also a critical variable, will use it to conrol our API call this is equivalent to totalPage in API 
        private int totalPage;
        // public AllOrgans listLessons;
        private Vector2 baseSize = new Vector2(1080, 2340); 
        private Vector2 baseCellSize; 
        private Vector2 baseCellSpacing; 
        private GridLayoutGroup layoutGroup; 
        private int calculatedSize = 35;

        async void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            
            searchBtn.SetActive(true);
            xBtn.SetActive(false); 
            xBtn.transform.GetComponent<Button>().onClick.AddListener(ClearInput); 

            AllOrgans listLessons;
            listLessons = await LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, "", offset, limit);
            totalPage = listLessons.meta.totalPage;
            // organLesson = listLessons.data;
            organTitle.gameObject.GetComponent<Text>().text = OrganManager.organName;

            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true)); 
            
            searchBox.GetComponent<InputField>().onValueChanged.AddListener(UpdateList);

            layoutGroup = contentForOrgansListLessons.GetComponent<GridLayoutGroup>();
        }
        private string formatString (string stringInput, int maxSize)
        {
            if (stringInput.Length > maxSize)
            {
                return stringInput.Remove(maxSize) + "..."; 
            }
            return stringInput;
        }
        public async void OnEndDrag(PointerEventData eventData)
        {
            AllOrgans listLessons;
            Debug.Log("Stopped draging: " + this.name);
            // if (content.verticalNormalizedPosition >= 0.95f) 
            // {
            //     Debug.Log("Current page: " + offset + "Total page; " + totalPage);
            //     if (offset > 0)
            //     {
            //         Debug.Log("Go to previous page");
            //         offset -= 1;
            //         organLesson = LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, "", offset, limit).data;
            //         loadLessons(organLesson);
            //     }
            // }
            if (content.verticalNormalizedPosition <= 0.05f){
                // Update current page 
                Debug.Log("Current page: " + offset + "Total page; " + totalPage);
                if (offset < totalPage - 1){
                    Debug.Log("Go to next page");
                    offset += 1;
                    // Get current text inside Search box then pass ...
                    listLessons = await LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, searchBox.GetComponent<InputField>().text, offset, limit);
                    StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, false));
                }     
            }
        }
        void LateUpdate()
        {
            searchBox.MoveTextEnd(true);
        }

        async void UpdateList(string data)
        {
            Debug.Log("Update string !!!");
            string processedString = Regex.Replace(data, @"\s+", " ").ToLower().Trim(charsToTrim);
            Debug.Log(processedString);
            // processedString = data.Trim(charsToTrim).ToLower();
            if (processedString == "")
            {
                // when input is empty
                xBtn.SetActive(false); 
                searchBtn.SetActive(true);
            }
            else
            {
                // when input is not empty 
                xBtn.SetActive(true);
                searchBtn.SetActive(false); 
            }
            offset = 0;
            // organLesson = LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, processedString, offset, limit).data;
            AllOrgans listLessons = await LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, processedString, offset, limit); 
            Debug.Log("listLesson: " + listLessons.ToString());
            // Update totalPage too when user change the search input 
            // when change input, we must renew lessons
            // totalPage = organLesson.meta.totalPage;
            // loadLessons(organLesson.data, organLesson.meta.totalElements, true);
            totalPage = listLessons.meta.totalPage;
            StopAllCoroutines();
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true));
        }

        async void ClearInput()
        {
            AllOrgans listLessons;
            searchBox.GetComponent<InputField>().SetTextWithoutNotify("");
            xBtn.SetActive(false); 
            searchBtn.SetActive(true); 

            offset = 0;
            listLessons = await LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, "", offset, limit);
            // organLesson = LoadData.Instance.GetListLessonsByOrgan(OrganManager.organId, "", offset, limit).data;
            StopAllCoroutines();
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true)); 
        }

        private void ChangeUIStatus(InputField input, GameObject warning, bool status)
        {
            warning.SetActive(status);
            if (status)
            {
                input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldBold); 
            }
            else 
            {
                input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
            }
        }

        IEnumerator loadLessons(Lesson[] lessons, int totalLessons, bool isRenewLesssons)
        {
            if (isRenewLesssons)
            {
                content.verticalNormalizedPosition = 1f;
                foreach(Transform child in contentForOrgansListLessons.transform)
                {
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
            // Start all request and init the UI
            foreach (Lesson lesson in lessons)
            {
                string imageUri = String.Format(APIUrlConfig.LoadLesson, lesson.lessonThumbnail);
                var www = UnityWebRequestTexture.GetTexture(imageUri);
                requests.Add(www.SendWebRequest());

                GameObject organLesson = Instantiate(Resources.Load(ItemConfig.totalLesson2) as GameObject, contentForOrgansListLessons.transform); 
                organLessonList.Add(organLesson);

                organLesson.name = lesson.lessonId.ToString(); 
                organLesson.transform.GetChild(1).gameObject.GetComponent<Text>().text = formatString(lesson.lessonTitle, calculatedSize);
                organLesson.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = lesson.viewed.ToString(); 
                organLesson.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(lesson.lessonId)); 
            }
            // Wait for all request parallel 
            yield return new WaitUntil(() => AllRequestsDone(requests));
            // Evaluate all result
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
                    // Dont modify any thing
                }
                else 
                {
                    Texture2D tex = ((DownloadHandlerTexture) www.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                    organLessonList[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = sprite;  
                    organLessonList[i].transform.GetChild(4).gameObject.SetActive(false);
                }
            }
        }
        // Get Texture for each URL 
        void GetTexture(string url, Action<string> onError, Action<Texture2D> onSuccess)
        {
            StartCoroutine(GetTextureCoroutine(url, onError, onSuccess));
        }

        IEnumerator GetTextureCoroutine(string url, Action<string> onError, Action<Texture2D> onSuccess)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url)){
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    onError(unityWebRequest.error);
                } 
                else 
                {
                    DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                    Debug.Log("DownloadHandlerTexture: " + downloadHandlerTexture);
                    onSuccess(downloadHandlerTexture.texture);
                }
            }
        }
        IEnumerator GetTextureRequest(string url, GameObject currentLesson, Action<Sprite> callback)
        {
            using(UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    Debug.Log(unityWebRequest.error);
                }
                else
                {
                    if (unityWebRequest.isDone)
                    {
                        Texture2D tex = ((DownloadHandlerTexture) unityWebRequest.downloadHandler).texture;
                        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                        callback(sprite);
                    }
                }
            }
        }
    }
}
