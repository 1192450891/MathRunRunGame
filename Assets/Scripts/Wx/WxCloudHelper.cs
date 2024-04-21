using System.Collections.Generic;
using GameBase;
using Struct;
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
                        // Debug.Log("setUserDataSuccess");
                        // Debug.Log(res.result);
                    },
                    fail = (res) =>
                    {
                        // Debug.Log("setUserDataFail");
                        // Debug.Log(res.errMsg);
                    },
                    complete = (res) =>
                    {
                        // Debug.Log("setUserDataComplete");
                        // Debug.Log(res.result);
                    }
                });
        }
        public void SetQuestionData(QuestionIdData questionIdData)//传入正确答案+错误答案的序列 使用correctLen作为界限
        {
            WXBase.cloud.CallFunction(new CallFunctionParam()
            {
                name = "setQuestionData",// 此处设置云函数名称，并非JS文件名
            
                data = JsonUtility.ToJson(questionIdData),
            
                success = (res) =>
                {
                    Debug.Log("setQuestionDataSuccess");
                    Debug.Log(res.result);
                },
                fail = (res) =>
                {
                    Debug.Log("setQuestionDataFail");
                    Debug.Log(res.errMsg);
                },
                complete = (res) =>
                {
                    Debug.Log("setQuestionDataComplete");
                    Debug.Log(res.result);
                }
            });
        }
    }
}