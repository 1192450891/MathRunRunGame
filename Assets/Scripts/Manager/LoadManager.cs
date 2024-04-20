using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Struct;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = System.Object;


public class LoadManager:MonoSingleton<LoadManager>
{
    public void LoadAndShowPrefabAsync(string objName, string objPath, Transform objParentTransform = null, Action<GameObject> callback = null)  
    {  
        Addressables.LoadAssetsAsync<GameObject>(objPath, obj =>  
        {  
            obj = Instantiate(obj, Vector3.zero, Quaternion.identity);  
            obj.name = objName;  
            obj.transform.SetParent(objParentTransform,false);
            callback?.Invoke(obj);  
        });  
    }

    public void GameStartPreload(Action callback = null)
    {
        LoadArtAsset();
        
        LoadQuestionTextureAssetAsync();
        CsvStaticData.SetCsvDataTable((() =>
        {
            callback?.Invoke();
        }));
    }
    
    public void LoadCsvAssetAsync(string csvLabel,DataTable dt, Action callback = null)  
    {  
        Addressables.LoadAssetsAsync<TextAsset>(csvLabel, csvFile =>  
        {  
            // 加载成功，获取CSV文件内容
            string csvContent = csvFile.text;
            CSVController.CSVHelper.SetDataTable(csvContent, dt);
            callback?.Invoke();  
        });  
    }

    private void LoadQuestionTextureAssetAsync(Action callback = null)
    {
        var textureLabel = "QuestionTexture";
        Addressables.LoadAssetsAsync<Texture2D>(textureLabel, (t) =>
        {
            CsvStaticData.Texture2DDic.Add(t.name,t);
        });
    }
    private class LoadNode
    {
        public LoadNode(LoadAssetType type,string str)
        {
            loadType = type;
            loadLabel = str;
        }
        public LoadAssetType loadType;
        public string loadLabel;
    }

    private void LoadArtAsset()
    {
        var loadList = new List<LoadNode>
        {
            new LoadNode(LoadAssetType.GameObject,"ArtAni"),
            new LoadNode(LoadAssetType.Material, "ArtMaterial"),
            new LoadNode(LoadAssetType.GameObject,"ArtModel"),
            new LoadNode(LoadAssetType.Shader,"ArtShader"),
            new LoadNode(LoadAssetType.GameObject,"ArtPreb"),
            new LoadNode(LoadAssetType.Texture2D, "ArtTexture")
        };
        foreach (var node in loadList)
        {
            LoadArtAssetAsync(node);
        }
    }

    enum LoadAssetType
    {
        GameObject,
        Material,
        Shader,
        Texture2D
    }
    private void LoadArtAssetAsync(LoadNode node)
    {
        switch (node.loadType)
        {
            case LoadAssetType.GameObject:
                Addressables.LoadAssetsAsync<GameObject>(node.loadLabel,null);
                break;
            case LoadAssetType.Material:
                Addressables.LoadAssetsAsync<Material>(node.loadLabel,null);
                break;
            case LoadAssetType.Shader:
                Addressables.LoadAssetsAsync<Shader>(node.loadLabel,null);
                break;
            case LoadAssetType.Texture2D:
                Addressables.LoadAssetsAsync<Texture2D>(node.loadLabel,null);
                break;
            default:
                Debug.Log("not find loadType");
                break;
        }
    }
    
}
