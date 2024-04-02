using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


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
}
