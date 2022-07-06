using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using BuildLesson;

public class InteractiveModel : MonoBehaviour
{
    public Button btnCreateThumbnail;
    public Button btnBack;
    public Button btnCompleteConversion;
    public int resWidth = 2550;
    public int resHeight = 3300;
    public Image imgScreenShot;
    public new Camera camera;
    public Transform parent2;
	public Vector3 offset;
    public InputField iModelName;
    public GameObject uiCoat;
    public GameObject NameModelWarning;
    public GameObject PictureWarning;
    public Image imgLoadingFill;
    public GameObject uiBFill;
    public Text txtPercent;

    private int idModel;
    public static string modelName;
    private byte[] thumbnail = null;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;

        var rotate = FindObjectOfType<Rotate>();

        GameObject currentBuildObject = Instantiate(rotate.gameObject);
        ObjectManagerBuildLesson.Instance.InitObject(currentBuildObject);  // rotate is null 

        NameModelWarning.SetActive(false);
        PictureWarning.SetActive(false);
        iModelName.characterLimit = 255;

        if (rotate != null)
        {
            var parent = rotate.transform;

            parent.SetParent(parent2);

            parent.localPosition = offset + new Vector3(0,-22,0);
            iModelName.text = parent.name;
            idModel = UploadModel.idModel;
        }

        InitEvents();       
    }

    void OnValidate()
    {
        btnBack = GameObject.Find("BtnBack").GetComponent<Button>();
        btnCreateThumbnail = GameObject.Find("BtnCreateThumbnail").GetComponent<Button>();
        camera = GameObject.Find("Render Camera").GetComponent<Camera>();
        parent2 = camera.transform;
        imgScreenShot = GameObject.Find("ImgScreenShot").GetComponent<Image>();
        iModelName=GameObject.Find("IModelName").GetComponent<InputField>();
        btnCompleteConversion = GameObject.Find("BtnComplete").GetComponent<Button>();
    }

    private void InitEvents()
    {
        btnBack.onClick.AddListener(() =>{ SceneManager.LoadScene("UploadModel");
        Destroy(FindObjectOfType<Rotate>().gameObject);});
        btnCreateThumbnail.onClick.AddListener(HandleCreateThumbnail);
        btnCompleteConversion.onClick.AddListener(HandleCompleteConversion);
        iModelName.onValueChanged.AddListener(checkNameModelValid);
    }

    private void checkNameModelValid(string data)
    {
        if(data != "")
        {
            changeUIStatus(iModelName, NameModelWarning, false);
        }
    }

    private void changeUIStatus(InputField input, GameObject warning, bool status)
    {
        warning.SetActive(status);
        if(status)
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
        }
        else
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        }
    }

    private void HandleCreateThumbnail()
    {
        LateUpdates();
    }

    private void HandleCompleteConversion()
    {
        StartCoroutine(HandleCompleteConversionAPI());
    }

    private IEnumerator HandleCompleteConversionAPI() 
    {
        bool check = true;
        string modelName = iModelName.text;
        uiCoat.SetActive(true);
        uiBFill.SetActive(true);
        txtPercent.text="0%";
        if(thumbnail == null)
        {
            check = false;
            PictureWarning.SetActive(true);
            uiCoat.SetActive(false);
        }
        else 
        {
            PictureWarning.SetActive(false);
        }

        if(modelName == "")
        {
            check = false;
            uiCoat.SetActive(false);
            changeUIStatus(iModelName, NameModelWarning, true);
        }

        if(check == false) 
        {
            yield return new WaitForSeconds(0f);
            yield break;
        }

        var form = new WWWForm();

        form.AddField("modelFileId", idModel);
        form.AddField("modelName", iModelName.text.ToString());
        form.AddBinaryData("thumbnail", thumbnail);
        
        string API_KEY = PlayerPrefs.GetString("user_token");
        using var www = UnityWebRequest.Post(APIUrlConfig.Import3DModel, form);
        www.SetRequestHeader("Authorization", API_KEY);

        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            imgLoadingFill.fillAmount = operation.progress * 2f;    
            txtPercent.text=$"{(imgLoadingFill.fillAmount*100f):N0} %";
            yield return null;
        }

        string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
        Debug.Log(response);
        ResImportModel res = JsonUtility.FromJson<ResImportModel>(response);

        if(res != null)
        {
            switch(res.code)
            {
                case "200":
                    Debug.Log("RESPONSE IMPORT 3D MODEL FILE: " + res.data[0].modelId);
                    Debug.Log("CREATE MODEL ID IMPORT 3D MODEL: " + res.data[0].modelId);

                    ModelStoreManager.InitModelStore(res.data[0].modelId);
                    yield return new WaitForSeconds(0f);
                    SceneManager.LoadScene(SceneConfig.createLesson);
                    ReStore();
                    break;

                case "400":
                    
                    SSTools.ShowMessage("Please fill full information!",SSTools.Position.bottom,SSTools.Time.twoSecond);
                    ReStore();
                    break;

                default:
                    SSTools.ShowMessage("Failed",SSTools.Position.bottom,SSTools.Time.twoSecond);
                    ReStore();
                    break;
            }
        }        
        yield return new WaitForSeconds(0);
    }

    public static string ScreenShotName(int width, int height)
    {
        return
            //$"Assets/ScreenShots/screen_{width}x{height}_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
            $"{Application.dataPath}/ScreenShots/screen_{width}x{height}_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
    }

    private void LateUpdates()
    {
        var rt = new RenderTexture(resWidth, resHeight, 24);

        camera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

        camera.Render();

        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors

        Destroy(rt);

        imgScreenShot.sprite = Sprite.Create(screenShot, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));

        thumbnail = screenShot.EncodeToPNG();
        //path = ScreenShotName(resWidth, resHeight);
        //File.WriteAllBytes(path, screenShot.EncodeToPNG());
    }

    private void ReStore() 
    {
        uiCoat.SetActive(false);
        uiBFill.SetActive(false);
        imgLoadingFill.fillAmount = 0; 
        txtPercent.text = "";
    }
}

[System.Serializable]
class ResImportModel 
{
    
    public string code;
    public string message;
    public ResDataImportModel[] data;
}

[System.Serializable]
class ResDataImportModel 
{
    public string modelName;
    public string modelFileId;
    public int thumnailFileId;
    public int createBy;
    public string createDate;
    public int modelId;
}