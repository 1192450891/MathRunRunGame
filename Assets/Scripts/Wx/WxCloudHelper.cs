using GameBase;
using UnityEngine;
using WeChatWASM;

namespace Wx
{
    public class WxCloudHelper
    {
        public void SetUserData(PlayerGameInfo playerGameInfo)
        {
            WXBase.cloud.CallFunction(new CallFunctionParam()
                {
                    name = "setUserData",// 此处设置云函数名称，并非JS文件名
            
                    data = JsonUtility.ToJson(playerGameInfo),
            
                    success = (res) =>
                    {
                        Debug.Log("setUserDataSuccess");
                        Debug.Log(res.result);
                    },
                    fail = (res) =>
                    {
                        Debug.Log("setUserDataFail");
                        Debug.Log(res.errMsg);
                    },
                    complete = (res) =>
                    {
                        Debug.Log("setUserDataComplete");
                        Debug.Log(res.result);
                    }
                });
        }
    }
}