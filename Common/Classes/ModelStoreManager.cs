using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class ModelStoreManager
{
    public static int modelId; 
    public static string filePath;
    public static void InitModelStore(int _modelId, string _filePath = null)
    {
        modelId = _modelId;
        if (_filePath != null)
        {
            filePath = _filePath;
        }
    }
}
