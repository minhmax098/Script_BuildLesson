using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;

namespace Home
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject waitingScreen;
        public GameObject contentItemCategoryWithLesson;
        public GameObject contentItemCategory;
        public GameObject searchBox;
        // add 3 record: searchBtn, xBtn, sumLesson
        public GameObject searchBtn;
        public GameObject xBtn;
        private ListXRLibrary x;
        private int numberCategories;
        public ScrollRect scroll; 
        private List<UnityWebRequestAsyncOperation> requests = new List<UnityWebRequestAsyncOperation>(); 
        private List<GameObject> organLessonList = new List<GameObject>(); 
        private int calculatedSize = 30; 

        void Start()
        {
            waitingScreen.SetActive(false);
            Screen.orientation = ScreenOrientation.Portrait;
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            // search record
            searchBtn.SetActive(true);
            xBtn.SetActive(false);
            xBtn.transform.GetComponent<Button>().onClick.AddListener(ClearInput); //
            // searchBox.GetComponent<InputField>().onValueChanged.AddListener(UpdateList); 
            // LoadCategories();
            StartCoroutine(LessonByCategory());
        }
        
        void Update()
        {
            if (searchBox.GetComponent<InputField>().isFocused == true)
            {
                Debug.Log("Search box is focused: ");
                PlayerPrefs.SetString("user_input", "");

                StartCoroutine(LoadAsynchronously(SceneConfig.xrLibrary_edit));
            }
        }
        private string formatString(string inputString, int maxSize)
        {
            if (inputString.Length > maxSize)
            {
                return inputString.Remove(maxSize) + "..."; 
            }
            return inputString;
        }
       
        void scrollContent(string id)
        {
            GameObject currentBtnObj = scroll.transform.Find("Viewport/Content/" + id).gameObject;
            Debug.Log("Scroll content active: ");
            Debug.Log("Value: " + (float)(currentBtnObj.transform.GetSiblingIndex() + 1) / numberCategories);
            scroll.verticalNormalizedPosition = 1f;
            scroll.verticalNormalizedPosition = 1f - (float)(currentBtnObj.transform.GetSiblingIndex() + 1) / numberCategories + 0.05f;
        }

        void ClearInput()
        {
            searchBox.GetComponent<InputField>().SetTextWithoutNotify(""); 
            xBtn.SetActive(false);
            searchBtn.SetActive(true);
        }

        IEnumerator LessonByCategory()
        {
            x = LoadData.Instance.GetCategoryWithLesson();
            numberCategories = x.data.Length;
            // load from file json
            foreach (OrganForHome organ in x.data)
            {
                // check if u have a lesson, load image and if u don't have a lesson, not load 
                if (organ.listLesson.Length > 0)
                {
                    GameObject itemCategoryObject = Instantiate(Resources.Load(DemoConfig.demoItemCategoryWithLessonPath) as GameObject);
                    itemCategoryObject.name = organ.organsId.ToString();
                    itemCategoryObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = organ.organsName;
                    itemCategoryObject.transform.parent = contentItemCategoryWithLesson.transform;
                    itemCategoryObject.transform.localScale = Vector3.one;
                    Button moreLessonBtn = itemCategoryObject.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>();
                    moreLessonBtn.onClick.AddListener(() => updateOrganManager(organ.organsId, organ.organsName));
                    GameObject subContent = itemCategoryObject.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                    foreach (LessonForHome lesson in organ.listLesson)
                    {
                        string imageUri = String.Format(APIUrlConfig.LoadLesson, lesson.lessonThumbnail);
                        var www = UnityWebRequestTexture.GetTexture(imageUri); 
                        requests.Add(www.SendWebRequest()); 
                        GameObject lessonObject = Instantiate(Resources.Load(DemoConfig.demoLessonObjectPath) as GameObject);
                        Debug.Log("lesson.lessonId.ToString() = "+ lesson.lessonId.ToString());
                        organLessonList.Add(lessonObject);
                        lessonObject.name = lesson.lessonId.ToString();
                        lessonObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = formatString(lesson.lessonTitle, calculatedSize);
                        lessonObject.transform.parent = subContent.transform;
                        Button lessonBtn = lessonObject.GetComponent<Button>();
                        lessonBtn.onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(lesson.lessonId));
                    }
                }
            }
            yield return new WaitUntil(() => AllRequestDone(requests)); 
            HandleAllRequestsWhenFinished(requests, organLessonList); 
        }

        void LateUpdate()
        {
            if (organLessonList.Count > 0)
            {
                for (var i = 0; i < organLessonList.Count; i++)
                {
                    organLessonList[i].GetComponent<RectTransform>().localScale = Vector3.one;
                }
            }
        }

        private bool AllRequestDone(List<UnityWebRequestAsyncOperation> requests)
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
                    // Change the image with the specific item
                    organLessonList[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = sprite;  
                    organLessonList[i].transform.GetChild(2).gameObject.SetActive(false);
                }
            }
        }

        void updateOrganManager(int id, string name)
        {
            OrganManager.InitOrgan(id, name);
            Debug.Log(id);
            StartCoroutine(LoadAsynchronously(SceneConfig.listOrgan_edit));
        }

        public IEnumerator LoadAsynchronously(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            waitingScreen.SetActive(true);
            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}