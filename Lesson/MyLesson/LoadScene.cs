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

namespace MyLesson
{
    public class LoadScene : MonoBehaviour, IEndDragHandler
    {
       public GameObject contentForOrgansListLessons; 
       public InputField searchBox; 
       public Lesson[] organLesson; 
       public GameObject sumLesson; 
       public GameObject xBtn; 
       public GameObject searchBtn; 
       private char[] charsToTrim = { '*', '.', ' '};
       public ScrollRect content; 
       // Variable control content 
       private int offset = 0; // this is curent page value, offset = 0..pageSize - 1 
       private int limit = 21; // this is pageSize in API 
       private int totalPage; 
       private GridLayoutGroup layoutGroup;
       private int calculatedSize = 30; 
       AllOrgans listLessons; 
       
       async void Start()
       {
           Screen.orientation = ScreenOrientation.Portrait; 
           StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
           StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
           
           searchBtn.SetActive(true); 
           xBtn.SetActive(false);
           xBtn.transform.GetComponent<Button>().onClick.AddListener(ClearInput);

           AllOrgans listLessons; 
           listLessons = LoadData.Instance.GetListMyLesson("", offset, limit); 
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

       public async void OnEndDrag(PointerEventData eventData)
       {
           AllOrgans listLessons; 
           Debug.Log("Stop dragging: " + this.name); 

           if (content.verticalNormalizedPosition <= 0.05f)
           {
               // Update current page
               Debug.Log("Currrent page: " + offset + "Total page: " + totalPage);
               if (offset < totalPage -1)
               {
                   Debug.Log("Go to the next page"); 
                   offset += 1; 
                   // Get current text inside search box then pass.
                   listLessons = LoadData.Instance.GetListMyLesson(searchBox.GetComponent<InputField>().text, offset, limit);
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
            AllOrgans listLessons = LoadData.Instance.GetListMyLesson(processedString, offset, limit);

            totalPage = listLessons.meta.totalPage; 
            StopAllCoroutines(); 
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true)); 
        }

        async void ClearInput()
        {
            searchBox.GetComponent<InputField>().SetTextWithoutNotify(""); 
            xBtn.SetActive(false); 
            searchBtn.SetActive(true); 

            offset = 0; 
            listLessons = LoadData.Instance.GetListMyLesson("", offset, limit);
            StopAllCoroutines();
            StartCoroutine(loadLessons(listLessons.data, listLessons.meta.totalElements, true));
            // AllOrgans listLessons; 
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
                    organLessonList[i].transform.GetChild(4).gameObject.SetActive(false);
                }
            }
        }
        // get texture for each URL 
        void GetTexture(string url, Action<string> onError, Action<Texture2D> onSucess)
        {
            StartCoroutine(GetTextureCoroutine(url, onError, onSucess)); 
        }

        IEnumerator GetTextureCoroutine(string url, Action<string> onError, Action<Texture2D> onSucess)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return unityWebRequest.SendWebRequest(); 
                if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
                {
                    onError(unityWebRequest.error);
                } 
                else 
                {
                    DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                    Debug.Log("DownloadHandlerTexture: " + downloadHandlerTexture);
                    onSucess(downloadHandlerTexture.texture);
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
