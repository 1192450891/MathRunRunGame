using LitJson;
using UnityEngine;
using WeChatWASM;

namespace Wx
{
    public class WxGetData
    {
        public void GetData()
        {
            // WX.InitSDK((int code)=> {
            //     WX.cloud.Init(new CallFunctionInitParam()
            //     {
            //         env = "mathrunruncsv-0gppeloj57600df4"
            //     });
            //     WX.cloud.CallFunction(new CallFunctionParam()
            //     {
            //         name = "levelDataHelper",// 此处设置云函数名称
            //
            //         data = JsonUtility.ToJson(""),// 此处代表上传的数据，必须要有，空数据或没有此行代码，均会报错，并且须经过此方法序列化才行。括号中的数据可根据实际需求修改。
            //
            //         success = (res) =>
            //         {
            //             // Debug.Log("res.result:"+res.result);
            //             var data = JsonMapper.ToObject(res.result);
            //             JsonData response = null;
            //             if (data.ContainsKey("data"))
            //             {
            //                 response = data["data"];
            //             }
            //             QuestionAmount = response.Count;
            //             // Debug.Log("QuestionAmount:"+QuestionAmount);
            //             head = GenerateRandomLinkedList(QuestionAmount-1);
            //             VariableInit();
            //             for (int i = 0; i < QuestionAmount; i++)
            //             {
            //                 levelData[i].question = response[head.val]["question"].ToString();
            //                 levelData[i].way = int.Parse(response[head.val]["way"].ToString());
            //                 levelData[i].correct_answer = response[head.val]["correct_answer"].ToString();
            //                 levelData[i].wrong_answer = response[head.val]["wrong_answer"].ToString();
            //                 // Debug.Log($"questions[{i}]:"+questions[i]);
            //                 head = head.next;
            //             }
            //             hasGetData = true;
            //             Debug.Log("levelDataHelperSuccess");
            //         },
            //         fail = (res) =>
            //         {
            //             Debug.Log(res.errMsg);
            //             Debug.Log("levelDataHelperFail");
            //         },
            //         complete = (res) =>
            //         {
            //             Debug.Log("levelDataHelperComplete");
            //         }
            //     });
            // });
        }
    }
}