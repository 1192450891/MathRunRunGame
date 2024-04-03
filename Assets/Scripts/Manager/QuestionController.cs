using System;
using System.Collections.Generic;
using System.Data;
using Framework.Core;
using Manager;
using Module.Enum;
using Struct;

public class QuestionController : Singleton<QuestionController>
{
    public int QuestionAmount;//各类型问题数量总和 即玩家过关需要通过的问题数量
    
    public ListNodeUtil.ListNode CurrentPanelQuestionIndexHead;//用于乱序填入题目数据 后面使用时按顺序访问即可

    private int currentPanelQuestionIndex;//用于切换画面上显示的问题 每次穿过板子时更新 显示的和存储的是一致的

    private bool hasGetData;
    
    private QuestionConfig questionConfig;

    public LevelData[] LevelData;
    
    public LevelData CurLevelData
    {
        get { return LevelData[currentPanelQuestionIndex]; }
    }

    public void GetData()
    {
        //======================================UNITY_EDITOR==========================================
#if UNITY_EDITOR
        currentPanelQuestionIndex = -1;
        //**********读取问题配置****************//
        GetQuestionConfig();
        //**********读取所有问题表的数据****************//
        GetALlQuestionData();
#endif
        //======================================WX===================================================
#if !UNITY_EDITOR
        WxGetData wxGetData = new WxGetData();
        wxGetData.GetData();
#endif
        hasGetData = true;
    }

    private void GetQuestionConfig()
    {
        string configPath = StaticString.CsvDataPath+"QuestionConfig.csv";
        DataTable configDt = CSVController.CSVHelper.ReadCSV(configPath);
        int configLen = configDt.Rows.Count;
        QuestionAmount = 0;
        for (int i = 0; i < configLen; i++)
        {
            QuestionAmount += Convert.ToInt32(configDt.Rows[i][1]);
        }
        questionConfig.TrueOrFalseQuestionCount = Convert.ToInt32(configDt.Rows[0][1]);
        questionConfig.TwoAnswerQuestionCount = Convert.ToInt32(configDt.Rows[1][1]);
        questionConfig.ThreeAnswerQuestionCount = Convert.ToInt32(configDt.Rows[2][1]);
        
        CurrentPanelQuestionIndexHead=ListNodeUtil.Instance.GenerateRandomLinkedList(QuestionAmount - 1);
    }

    private void GetALlQuestionData()
    {
        LevelData = new LevelData[QuestionAmount];
        GetQuestionData(StaticString.CsvDataPath+"TrueOrFalseQuestionData.csv",questionConfig.TrueOrFalseQuestionCount,QuestionTypeEnum.TrueOrFalse);//判断题
        GetQuestionData(StaticString.CsvDataPath+"TwoAnswerQuestionData.csv",questionConfig.TwoAnswerQuestionCount,QuestionTypeEnum.TwoAnswerQuestion);//2选1题
        GetQuestionData(StaticString.CsvDataPath+"ThreeAnswerQuestionData.csv",questionConfig.ThreeAnswerQuestionCount,QuestionTypeEnum.ThreeAnswerQuestion);//3选1题
    }

    private void GetQuestionData(string path,int questionCount,QuestionTypeEnum questionType)
    {
        ListNodeUtil.ListNode head = ListNodeUtil.Instance.GenerateRandomLinkedList(questionCount - 1);
        DataTable dt = CSVController.CSVHelper.ReadCSV(path);
        while (head!=null)//已经把当前题型所需数量遍历了一遍
        {
            int val = CurrentPanelQuestionIndexHead.val;
            int j = 0;//列数下标
            LevelData[val].questionType = questionType;
            LevelData[val].id = dt.Rows[head.val][j++].ToString();//题目Id
            LevelData[val].question = dt.Rows[head.val][j++].ToString();//题干
            LevelData[val].way = int.Parse(dt.Rows[head.val][j++].ToString());//正确答案位置
            LevelData[val].score=int.Parse(dt.Rows[head.val][j++].ToString());//分数
            LevelData[val].questionKey = dt.Rows[head.val][j++].ToString();//题解内容
            LevelData[val].answers = new List<string>();
            
            while (j<dt.Rows[head.val].ItemArray.Length)
            {
                LevelData[val].answers.Add(dt.Rows[head.val][j].ToString());//第一个是正确答案 后面是错误答案
                j++;
            }

            CurrentPanelQuestionIndexHead = CurrentPanelQuestionIndexHead.next;
            head = head.next;
        }
        
    }

    public void ManualStart()
    {
        if (hasGetData)
        {
            TimeTool.Instance.Delay(0.2f, NextQuestion );
            hasGetData = false;
        }
    }

    public void NextQuestion()
    {
        if (currentPanelQuestionIndex>=QuestionAmount-1) //穿过最后一个板子时不用再显示问题
        {
            ClearQuestion();
            return;
        }
        EventManager.Instance.TriggerEvent<LevelData>(ClientEvent.QuestionController_NextQuestion,LevelData[++currentPanelQuestionIndex]);
    }

    private void ClearQuestion()
    {
        EventManager.Instance.TriggerEvent(ClientEvent.QuestionController_AllQuestionDone);
    }

    public void ReStart()
    {
        GetData();//重新读表获取问题

    }
}