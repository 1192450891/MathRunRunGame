using System;
using System.Collections.Generic;
using System.Data;
using Framework.Core;
using Manager;
using Struct;
using TMPro;
using UnityEngine;
using Wx;

public class QuestionController : Singleton<QuestionController>
{
    public int QuestionAmount;//各类型问题数量总和 即玩家过关需要通过的问题数量
    
    [HideInInspector] public int currentPanelQuestionIndex; //用于切换画面上显示的问题

    private ListNodeUtil.ListNode head;

    // [Tooltip("栅栏透明材质球")] [SerializeField] public Material FenceFade;
    private bool firstTime; //第一次不更新跑道

    private bool hasEndLine;

    private bool hasGetData;
    
    private QuestionConfig questionConfig;

    public LevelData[] levelData;

    public void GetData()
    {
        //======================================UNITY_EDITOR==========================================
#if UNITY_EDITOR
        //**********读取问题配置****************//
        GetQuestionConfig();
        //**********读取所有问题表的数据****************//
        GetALlQuestionData();
        hasGetData = true;
#endif
        //======================================WX===================================================
#if !UNITY_EDITOR
        WX.InitSDK((int code)=> {
        WX.cloud.Init(new CallFunctionInitParam()
        {
            env = "mathrunruncsv-0gppeloj57600df4"
        });
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "levelDataHelper",// 此处设置云函数名称
            
            data = JsonUtility.ToJson(""),// 此处代表上传的数据，必须要有，空数据或没有此行代码，均会报错，并且须经过此方法序列化才行。括号中的数据可根据实际需求修改。
            
            success = (res) =>
            {
                // Debug.Log("res.result:"+res.result);
                var data = JsonMapper.ToObject(res.result);
                JsonData response = null;
                if (data.ContainsKey("data"))
                {
                    response = data["data"];
                }
                QuestionAmount = response.Count;
                // Debug.Log("QuestionAmount:"+QuestionAmount);
                head = GenerateRandomLinkedList(QuestionAmount-1);
                VariableInit();
                for (int i = 0; i < QuestionAmount; i++)
                {
                    levelData[i].question = response[head.val]["question"].ToString();
                    levelData[i].way = int.Parse(response[head.val]["way"].ToString());
                    levelData[i].correct_answer = response[head.val]["correct_answer"].ToString();
                    levelData[i].wrong_answer = response[head.val]["wrong_answer"].ToString();
                    // Debug.Log($"questions[{i}]:"+questions[i]);
                    head = head.next;
                }
                hasGetData = true;
                Debug.Log("levelDataHelperSuccess");
            },
            fail = (res) =>
            {
                Debug.Log(res.errMsg);
                Debug.Log("levelDataHelperFail");
            },
            complete = (res) =>
            {
                Debug.Log("levelDataHelperComplete");
            }
        });
        });
#endif
    }

    private void GetQuestionConfig()
    {
        string configpath = Application.dataPath+"/CSVData/QuestionConfig.csv";
        DataTable configdt = CSVController.CSVHelper.ReadCSV(configpath);
        int configLen = configdt.Rows.Count;
        QuestionAmount = 0;
        for (int i = 0; i < configLen; i++)
        {
            QuestionAmount += Convert.ToInt32(configdt.Rows[i][1]);
        }
        questionConfig.TrueOrFalseQuestionCount = Convert.ToInt32(configdt.Rows[0][1]);
        questionConfig.TwoAnswerQuestionCount = Convert.ToInt32(configdt.Rows[1][1]);
        questionConfig.ThreeAnswerQuestionCount = Convert.ToInt32(configdt.Rows[2][1]);
    }

    private void GetALlQuestionData()
    {
        levelData = new LevelData[QuestionAmount];
        head = ListNodeUtil.Instance.GenerateRandomLinkedList(QuestionAmount - 1);
        GetQuestionData(Application.dataPath+"/CSVData/TrueOrFalseQuestionData.csv",questionConfig.TrueOrFalseQuestionCount,QuestionTypeEnum.TrueOrFalse);//判断题
        GetQuestionData(Application.dataPath+"/CSVData/TwoAnswerQuestionData.csv",questionConfig.TwoAnswerQuestionCount,QuestionTypeEnum.TwoAnswerQuestion);//2选1题
        GetQuestionData(Application.dataPath+"/CSVData/ThreeAnswerQuestionData.csv",questionConfig.ThreeAnswerQuestionCount,QuestionTypeEnum.ThreeAnswerQuestion);//3选1题
    }

    private void GetQuestionData(string path,int questionCount,QuestionTypeEnum questionType)
    {
        DataTable dt = CSVController.CSVHelper.ReadCSV(path);
        QuestionAmount = questionCount>dt.Rows.Count?dt.Rows.Count:QuestionAmount;
        int i = 0;
        switch (questionType)
        {
            case QuestionTypeEnum.TrueOrFalse: i = 0;break;
            case QuestionTypeEnum.TwoAnswerQuestion: i = questionConfig.TrueOrFalseQuestionCount;break;
            case QuestionTypeEnum.ThreeAnswerQuestion: i = questionConfig.TrueOrFalseQuestionCount+questionConfig.TwoAnswerQuestionCount;break;
        }
        int m = i;
        for (; i < m+questionCount; i++)
        {
            int j = 0;//列数下标
            levelData[i].questionType = questionType;
            levelData[i].id = dt.Rows[head.val][j++].ToString();//题目Id
            levelData[i].question = dt.Rows[head.val][j++].ToString();//题干
            levelData[i].way = int.Parse(dt.Rows[head.val][j++].ToString());//正确答案位置
            levelData[i].score=int.Parse(dt.Rows[head.val][j++].ToString());//分数
            levelData[i].questionKey = dt.Rows[head.val][j++].ToString();//题解内容
            levelData[i].answers = new List<string>();
            
            while (j<dt.Rows[head.val].ItemArray.Length)
            {
                levelData[i].answers.Add(dt.Rows[head.val][j].ToString());//第一个是正确答案 后面是错误答案
                j++;
            }
            head = head.next;
        }
    }
    void Start()
    {

    }

    public void ManualStart()
    {
        if (hasGetData)
        {
            TimeTool.Instance.Delay(0.2f, NextQuestion );
            hasGetData = false;
        }
    }

    void Update()
    {
    }

    public void NextQuestion()
    {
        if (currentPanelQuestionIndex >= QuestionAmount)return;
        EventManager.Instance.TriggerEvent<LevelData>(ClientEvent.QuestionController_NextQuestion,levelData[currentPanelQuestionIndex++]);
    }

    public void ClearQuestion()
    {
        EventManager.Instance.TriggerEvent(ClientEvent.QuestionController_AllQuestionDone);
    }


    public bool IsAllQuestionDone()
    {
        if (RunwayManager.Instance.LevelDataIndex < QuestionAmount)
        {
            return false;
        }

        if (hasEndLine == false)
        {
            RunwayManager.Instance.CreateFinishLine();
            hasEndLine = true;
        }

        return true;
    }

    public void ReStart()
    {
        GetData();//重新读表获取问题
        currentPanelQuestionIndex = 0;
        hasEndLine = false;
        
    }





}