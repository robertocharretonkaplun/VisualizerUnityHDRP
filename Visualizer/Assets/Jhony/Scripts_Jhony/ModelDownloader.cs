/*using UnityEngine.Networking;
using UnityEngine;

ususing System.Collections; 
using System.IO; 
using UnityEngine; 
using UnityEngine.Networking; 
using TriLibCore; 
using System.Collections.Generic; 
using TriLibCore.SFB;

public class ModelDownloader : MonoBehaviour
{
    public string url; private IList _items;

    private void Start()
    {
        StartCoroutine("DownloadZipFile");
    }

    private IEnumerator DownloadZipFile()
    {
        if (File.Exists(Application.persistentDataPath + "/zip_file_name.zip"))
        {
            print("Target Zip file exists");
            LoadModel();
        }

        if (!File.Exists(Application.persistentDataPath + "/zip_file_name.zip"))
        {
            print("Target Zip file doesn't exists");

            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                print("Start downloading zip file");
                yield return www.Send();
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string savePath = string.Format("{0}/{1}.zip", Application.persistentDataPath, "zip_file_name");
                    File.WriteAllBytes(savePath, www.downloadHandler.data);
                    print("Target Zip file downloaded");
                    LoadModel();
                }
            }
        }
    }


    void LoadModel()
    {
        print("Start loading zip file");
        var hasFiles = _items != null && _items.Count > 0 && _items[0].HasData;
        var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        AssetLoaderZip.LoadModelFromZipFile(Application.persistentDataPath + "/zip_file_name.zip", OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions, _items, null);
    }

    private void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }

    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Materials loaded. Model fully loaded.");
    }
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model loaded. Loading materials.");
    }
}*/