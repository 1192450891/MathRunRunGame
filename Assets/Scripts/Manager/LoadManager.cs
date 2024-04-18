using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


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
}
