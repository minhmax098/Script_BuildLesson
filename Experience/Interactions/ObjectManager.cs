using System.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Net;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TriLibCore;
using System.Linq;
using System.IO;
using Photon.Pun;
using UnityEngine.Video;

// using BuildLesson;

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager instance;
    public static ObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectManager>();
            }
            return instance;
        }
    }
    
    public const float X_SIZE_BOUND = 2.5f;
    public const float Y_SIZE_BOUND = 2.5f;
    public const float Z_SIZE_BOUND = 2.5f;
    public float FactorScaleInitial { get; set; }
    Bounds boundOriginObject;

    private const float TIME_SCALE_FOR_APPEARANCE = 0.04f;
    public static event Action onInitOrganSuccessfully;
    public static event Action<string> onChangeCurrentObject;
    public static event Action onResetObject;

    public OrganInfor OriginOrganData { get; set; }
    public Material OriginOrganMaterial { get; set; }
    public GameObject OriginObject { get; set; }
    public List<Vector3> ListchildrenOfOriginPosition { get; set; }
    public GameObject CurrentObject { get; set; }
    public Vector3 OriginPosition { get; set; }
    public Quaternion OriginRotation { get; set; }
    public Vector3 OriginScale { get; set; }

    // Add 
    private GameObject objectInstance;
    private string lessonId;
    private static string API_KEY;
    private int modelId;

    void Start() 
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    public void LoadDataOrgan()
    {
        // OriginOrganData = new OrganInfor("", "Brain_Demo", "");
    }

    public void InitOriginalExperience()
    {
        StartCoroutine(IEDownloadModel());
        // LabelManagerBuildLesson.Instance.updateCenterPosition();
    }

    public void ScaleObjectWithBound(GameObject objectInstance)
    {
        boundOriginObject = Helper.CalculateBounds(objectInstance);
        FactorScaleInitial = Mathf.Min(Mathf.Min(X_SIZE_BOUND / boundOriginObject.size.x, Y_SIZE_BOUND / boundOriginObject.size.y), Z_SIZE_BOUND / boundOriginObject.size.z);
        objectInstance.transform.localScale = objectInstance.transform.localScale * FactorScaleInitial;
    }
    
    public void SetCollider(GameObject objectInstance)
    {
        var transforms = objectInstance.GetComponentsInChildren<Transform>();
        if (transforms.Length <= 0)
        {
            return;
        }
        foreach (var item in transforms)
        {
            item.gameObject.AddComponent<MeshCollider>();
            // item.tag = TagConfig.;
        }
    }

    public IEnumerator GetModelId()
    {
        API_KEY = PlayerPrefs.GetString("user_token");
        lessonId = LessonManager.lessonId.ToString();
        Debug.Log(API_KEY);
        Debug.Log(lessonId);
        string BASE_URL = "https://api.xrcommunity.org/v1/xap/";
        using (var uwr = UnityWebRequest.Get(BASE_URL + $"lessons/getLessonDetail/{lessonId}"))
        {
            uwr.SetRequestHeader("Authorization",API_KEY);
            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
            {
                yield return null;
            }

            if (uwr.downloadHandler.text == "Unauthorized")
            {
                Debug.Log(uwr.downloadHandler.text);
                yield break;
            }
            string response = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
            AllGetLessonDetail all_data = JsonUtility.FromJson<AllGetLessonDetail>(response);
            GetLessonDetail data = all_data.data[0];
            Debug.Log(data.modelId);
            modelId = data.modelId;
        }
    }

    public void InitObject(GameObject newObject)
    {   
        Debug.Log("Init object: " + newObject.name);
        this.OriginObject = newObject;
        // OriginOrganMaterial = OriginObject.GetComponent<Renderer>().materials[0];
        this.OriginOrganMaterial = OriginObject.GetComponentInChildren<Renderer>().materials[0];
        ChangeCurrentObject(OriginObject);
        this.OriginPosition = OriginObject.transform.position;
        this.OriginRotation = OriginObject.transform.rotation;
        this.OriginScale = OriginObject.transform.localScale;

        ScaleObjectWithBound(OriginObject);
        SetCollider(OriginObject);

        onInitOrganSuccessfully?.Invoke();
    }

    public void ChangeCurrentObject(GameObject newGameObject)
    {
        this.CurrentObject = newGameObject;
        this.ListchildrenOfOriginPosition = Helper.GetListchildrenOfOriginPosition(CurrentObject);
        onChangeCurrentObject?.Invoke(CurrentObject.name);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object at specified position/rotation in AR mode
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void InstantiateARObject(Vector3 position, Quaternion rotation, bool isHost)
    {
        OriginObject.transform.position = position;
        OriginObject.transform.rotation = rotation;
        if (isHost && (!ARUIManager.Instance.IsStartAR))
        {
            OriginObject.transform.localScale *= ModelConfig.scaleFactorInARMode;
        }
        OriginObject.SetActive(true);
        AddARAnchorToObject();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show zoom up affect for object
    /// </summary>
    public void ShowZoomUpAffectForObject()
    {
        StartCoroutine(Helper.EffectScaleObject(OriginObject, TIME_SCALE_FOR_APPEARANCE, OriginObject.transform.localScale));
    }

    public void Instantiate3DObject()
    {
        OriginObject.transform.position = OriginPosition;
        OriginObject.transform.rotation = OriginRotation;
        OriginObject.transform.localScale = OriginScale;
        OriginObject.SetActive(true);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add ARAnchor component to object
    /// </summary>
    public void AddARAnchorToObject()
    {
        if (OriginObject != null)
        {
            if (OriginObject.GetComponent<ARAnchor>() == null)
            {
                ARAnchor localAnchor = OriginObject.AddComponent<ARAnchor>();
                localAnchor.destroyOnRemoval = false;
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get ARAnchor component from object
    /// </summary>
    /// <returns></returns>
    public ARAnchor GetARAnchorComponent()
    {
        if (OriginObject == null)
        {
            return null;
        }
        if (OriginObject.GetComponent<ARAnchor>() == null)
        {
            AddARAnchorToObject();
        }
        return OriginObject.GetComponent<ARAnchor>();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Destroy original object
    /// </summary>
    public void DestroyOriginalObject()
    {
        if (OriginObject != null)
        {
            Destroy(OriginObject);
        }
        if (CurrentObject != null)
        {
            Destroy(CurrentObject);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Destroy AR Anchor component
    /// </summary>
    public void DestroyARAnchorComponent()
    {
        if (OriginObject != null)
        {
            ARAnchor localAnchor = OriginObject.GetComponent<ARAnchor>();
            if (localAnchor != null)
            {
                Destroy(localAnchor);
            }
        }
    }

    // public void Init3DObject
    private IEnumerator IEDownloadModel()
    {
        API_KEY = PlayerPrefs.GetString("user_token");
        lessonId = LessonManager.lessonId.ToString();
        Debug.Log(API_KEY);
        Debug.Log(lessonId);
        string BASE_URL = "https://api.xrcommunity.org/v1/xap/";

        using (var uwr = UnityWebRequest.Get(BASE_URL + $"lessons/getLessonDetail/{lessonId}"))
        {
            uwr.SetRequestHeader("Authorization",API_KEY);
            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
            {
                yield return null;
            }

            if (uwr.downloadHandler.text == "Unauthorized")
            {
                Debug.Log(uwr.downloadHandler.text);
                yield break;
            }
            string response = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
            AllGetLessonDetail all_data = JsonUtility.FromJson<AllGetLessonDetail>(response);
            GetLessonDetail data = all_data.data[0];
            modelId = data.modelId;
        }

        Debug.Log(modelId);
        
        using (var uwr = UnityWebRequest.Get(BASE_URL + $"models/get3DModelDetail/{modelId}"))
        {
            uwr.SetRequestHeader("Authorization",API_KEY);

            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
            {
                yield return null;
            }

            if (uwr.downloadHandler.text == "Unauthorized")
            {
                Debug.Log(uwr.downloadHandler.text);
                yield break;
            }
            string str = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
            Debug.Log(str);
            Response response = JsonUtility.FromJson<Response>(str);

            ModelData data = response.data[0];
            Debug.Log(data.modelFile);
            DownloadFile(BASE_URL + data.modelFile);
        }
    }
    public void DownloadFile(string url)
    {
        using var client = new WebClient();
        var lastIndex = url.LastIndexOf("/", StringComparison.Ordinal);
        var modelName = url.Remove(0, lastIndex + 1);
        var loaded = false;
        Debug.Log(modelName);

        client.DownloadProgressChanged +=
            (sender, args) =>
            {
                Debug.Log($"Percent:{args.ProgressPercentage}");

                if (args.ProgressPercentage == 100 &&
                    !loaded)
                {
                    loaded = true;

                    Debug.Log($"path: {Application.persistentDataPath}/{modelName}");

                    AssetLoader.LoadModelFromFile($"{Application.persistentDataPath}/{modelName}",
                        x =>
                        {
                            // x = x.RootGameObject.transform.GetChild(0);
                            var cam = Camera.main;

                            if (cam != null)
                            {
                                x.RootGameObject.transform.SetParent(cam.transform);
                            }

                            var render = x.RootGameObject.GetComponentsInChildren<MeshRenderer>();

                            foreach (var mrc in x.MaterialRenderers.Values.SelectMany(y => y))
                            {
                                foreach (var r in render)
                                {
                                    if (r.name != mrc.Renderer.name)
                                    {
                                        continue;
                                    }

                                    r.materials = mrc.Renderer.materials;
                                    break;
                                }
                            }

                            x.RootGameObject.transform.localPosition = Vector3.zero;
                            x.RootGameObject.transform.localRotation = Quaternion.Euler(Vector3.up * 180f);

                            x.RootGameObject.AddComponent<Rotate>();

                            if (x.RootGameObject.transform.parent != null)
                            {
                                x.RootGameObject.transform.SetParent(null);
                            }

                            DontDestroyOnLoad(x.RootGameObject);
                            GameObject prefabMainOrgan = x.RootGameObject.transform.GetChild(0).gameObject as GameObject;
                            Debug.Log("PREFABMAINORGAN: " + prefabMainOrgan.name);
                            var audioSource = prefabMainOrgan.gameObject.AddComponent<AudioSource>();
                            audioSource.clip = Resources.Load<AudioClip>("Media/Audios/Brain");
                            audioSource.priority = 128;
                            audioSource.volume = 1;
                            audioSource.pitch = 1;
                            audioSource.panStereo = 0;
                            audioSource.spatialBlend = 0;
                            audioSource.reverbZoneMix = 1;

                            var videoPlayer = prefabMainOrgan.gameObject.AddComponent<VideoPlayer>();
                            videoPlayer.source = VideoSource.VideoClip;
                            videoPlayer.clip = null;
                            videoPlayer.playOnAwake = false;
                            videoPlayer.waitForFirstFrame = false;
                            videoPlayer.isLooping = false;
                            videoPlayer.skipOnDrop = false;
                            videoPlayer.playbackSpeed = 1;
                            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                            videoPlayer.targetTexture = Resources.Load<RenderTexture>("Textures/VideoZoom");
                            videoPlayer.aspectRatio = VideoAspectRatio.Stretch;
                            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

                            var photonView = prefabMainOrgan.gameObject.AddComponent<PhotonView>();
                            photonView.OwnershipTransfer = OwnershipOption.Request;
                            photonView.Synchronization = ViewSynchronization.UnreliableOnChange;
                            photonView.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

                            var dataSyncManager = prefabMainOrgan.gameObject.AddComponent<DataSyncManager>();
                            dataSyncManager.isSynchronizedLocalPosition = true;
                            dataSyncManager.isSynchronizedLocalRotation = true;
                            dataSyncManager.isSynchronizedLocalScale = true;
                            dataSyncManager.isSynchronizedDisplay = true;

                            var photonTransformView = prefabMainOrgan.gameObject.AddComponent<PhotonTransformView>();
                            photonTransformView.m_SynchronizePosition = false;
                            photonTransformView.m_SynchronizeRotation = true;
                            photonTransformView.m_SynchronizeScale = true;
                            photonTransformView.m_UseLocal = true;

                            prefabMainOrgan.gameObject.AddComponent<MeetingObjectInstantiateObserve>();
                            objectInstance = Instantiate(prefabMainOrgan, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
                            InitObject(objectInstance);
                        },
                        x => { },
                        (x, y) => { },
                        x => { },
                        null,
                        ScriptableObject.CreateInstance<AssetLoaderOptions>(),
                        false);
                }
            };

        client.DownloadFileAsync(new Uri(url), $"{Application.persistentDataPath}/{modelName}");
    }
}
    [System.Serializable]
    class Response 
    {
        public long code;
        public string message;
        public ModelData[] data;
    }

    [System.Serializable]
    class ModelData 
    {
        public long modelId;
        public string ModelName;
        public long modelFileId;
        public long thumbnailFileId;
        public string modelThumbnail;
        public string modelFile;
        public int type;
    }
