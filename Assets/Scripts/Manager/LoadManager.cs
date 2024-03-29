using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class LoadManager:MonoSingleton<LoadManager>
{
    public void LoadAndShowPrefabAsync(string ObjName, string ObjPath, Transform ObjParentTransform = null, Action<GameObject> callback = null)  
    {  
        Addressables.LoadAssetsAsync<GameObject>(ObjPath, obj =>  
        {  
            obj = Instantiate(obj, Vector3.zero, Quaternion.identity);  
            obj.name = ObjName;  
            obj.transform.SetParent(ObjParentTransform,false);
            callback?.Invoke(obj);  
        });  
    }
}
