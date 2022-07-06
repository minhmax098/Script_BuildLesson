using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TriLibCore;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

public class UploadModel : MonoBehaviour
{
    public Image imgLoadingFill;
    public GameObject uiBFill;
    public Text txtPercent;
    public GameObject uiUploadModelInfo;
    public Button btnUploadModel3D;
    public Button btnBack;
    public GameObject uiCoat;
    public static int idModel;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        SetEventUI();
    }

    public void SetEventUI()
    {
        btnUploadModel3D.onClick.AddListener(HandlerUploadModel);
        btnBack.onClick.AddListener(() => SceneManager.LoadScene("CreateLesson_Main"));
    }

    public void HandlerUploadModel()
    {
        imgLoadingFill.fillAmount = 0f;

        AssetLoaderFilePicker.Create()
            .LoadModelFromFilePickerAsync("load model",
                x =>
                {
                    uiCoat.SetActive(true);
                    uiBFill.SetActive(true);
                    txtPercent.text="0%";
                    
                    var path = $"{x.Filename}";
                    var cam = Camera.main;

                    if (cam != null)
                    {
                        x.RootGameObject.transform.SetParent(cam.transform);
                    }

                    var render = x.RootGameObject.GetComponentsInChildren<MeshRenderer>();

                    foreach (var y in x.MaterialRenderers.Values)
                    {
                        foreach (var mrc in y)
                        {
                            foreach (var r in render)
                            {
                                if (r.name == mrc.Renderer.name)
                                {
                                    r.materials = mrc.Renderer.materials;
                                    break;
                                }
                            }
                        }
                    }

                    var sizeY = x.RootGameObject.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y;

                    while (sizeY<1)
                    {
                        sizeY *= 150f;
                    }

                    sizeY *= 300f;
                    x.RootGameObject.transform.localScale = new Vector3(sizeY, sizeY, sizeY);
                    x.RootGameObject.transform.localPosition = Vector3.zero + new Vector3(0,-50,0);
                    x.RootGameObject.transform.localRotation = Quaternion.Euler(Vector3.up * 180f);

                    x.RootGameObject.AddComponent<Rotate>();

                    if (x.RootGameObject.transform.parent != null)
                    {
                        x.RootGameObject.transform.SetParent(null);
                    }

                    StartCoroutine(HandleUploadModel3D(File.ReadAllBytes(path), path));
                    DontDestroyOnLoad(x.RootGameObject);
                },
                x => { },
                (x, y) => { },
                x => { },
                x => { },
                null,
                ScriptableObject.CreateInstance<AssetLoaderOptions>());
    }

    public IEnumerator HandleUploadModel3D(byte[] fileData, string fileName)
    {
        var form = new WWWForm();

        form.AddBinaryData("model", fileData, fileName);
        string API_KEY = PlayerPrefs.GetString("user_token");
        using var www = UnityWebRequest.Post(APIUrlConfig.Upload3DModel, form);
        www.SetRequestHeader("Authorization", API_KEY);
        var operation = www.SendWebRequest();   

        while (!operation.isDone)
        {

            imgLoadingFill.fillAmount = operation.progress * 2f;    
            txtPercent.text=$"{(imgLoadingFill.fillAmount*100f):N0} %";
            yield return null;
        }

        uiUploadModelInfo.SetActive(false);
        
        Debug.Log(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));
        string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
        

        if (www.downloadHandler.text == "Unauthorized" ||
            www.downloadHandler.text.StartsWith("<!DOCTYPE html>"))
        {
            SSTools.ShowMessage($"{www.downloadHandler.text}!",SSTools.Position.bottom,SSTools.Time.twoSecond);
            yield return new WaitForSeconds(1);
            ReStore();
            yield break;
        }
        else 
        {
            ResUpModel res = JsonUtility.FromJson<ResUpModel>(response);
            Debug.Log(res.ToString());

            //var postmodel = JsonConvert.DeserializeObject<PostModel>(www.downloadHandler.text);

            if (res != null)
            {
                switch (res.code)
                {
                    case "200":
                        // Set value to the global variable id_model
                        if(res.data[0].file_id != null)
                        {
                            idModel = res.data[0].file_id;
                            Debug.Log("REPONSE UPLOAD MODEL FILE: " + response);
                            Debug.Log("CREATED MODEL ID: " + idModel);

                            // tra ra response
                            // ModelManager.InitModelData(idModel);
                            ModelStoreManager.InitModelStore(idModel, res.data[0].file_path);
                        } 
                        yield return new WaitForSeconds(0f);
                        ReStore();
                        SceneManager.LoadScene(SceneConfig.interactiveModel);
                        break;
                    case "400" :
                        ReStore();
                        SSTools.ShowMessage($"Please choose correct format file!",SSTools.Position.bottom,SSTools.Time.twoSecond);
                        break;
                    default:
                        SSTools.ShowMessage($"Upload Failed : {www.downloadHandler.text}!",SSTools.Position.bottom,SSTools.Time.twoSecond);
                        ReStore();
                        break;
                }
            }         
        }
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
class ResUpModel 
{
    
    public string code;
    public string message;
    public ResData[] data;
}

[System.Serializable]
class ResData 
{
    public int type;
    public string extention;
    public double size;
    public string file_name;
    public string file_path;
    public int created_by;
    public string created_date;
    public int file_id;
}

