using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        currentPanelQuestionIndex = -1;
        //**********读取问题配置****************//
        GetQuestionConfig(CsvStaticData.ConfigTable);
        //**********读取所有问题表的数据****************//
        GetALlQuestionData();
        hasGetData = true;
    }
    
    private void GetQuestionConfig(DataTable configDt)
    {
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
        GetQuestionData(CsvStaticData.TureOrFalseQuestionTable,questionConfig.TrueOrFalseQuestionCount,QuestionTypeEnum.TrueOrFalse);//判断题
        GetQuestionData(CsvStaticData.TwoAnswerQuestionTable,questionConfig.TwoAnswerQuestionCount,QuestionTypeEnum.TwoAnswerQuestion);//2选1题
        GetQuestionData(CsvStaticData.ThreeAnswerQuestionTable,questionConfig.ThreeAnswerQuestionCount,QuestionTypeEnum.ThreeAnswerQuestion);//3选1题
    }
    
    private void GetQuestionData(DataTable dataTable,int questionCount,QuestionTypeEnum questionType)
    {
        ListNodeUtil.ListNode head = ListNodeUtil.Instance.GenerateRandomLinkedList(dataTable.Rows.Count - 1);
        DataTable dt = dataTable;
        for (int i = 0; i < questionCount; i++)
        {
            int val = CurrentPanelQuestionIndexHead.val;
            int j = 0;//列数下标
            LevelData[val] = new LevelData(
                questionType,
                dt.Rows[head.val][j++].ToString(),
                dt.Rows[head.val][j++].ToString(),
                int.Parse(dt.Rows[head.val][j++].ToString()),
                int.Parse(dt.Rows[head.val][j++].ToString()),
                dt.Rows[head.val][j++].ToString(),
                dt.Rows[head.val].ItemArray.ToArray().Skip(j).Cast<string>().ToList()
            );
            CurrentPanelQuestionIndexHead = CurrentPanelQuestionIndexHead.next;
            head = head.next;
        }
        // while (head!=null)//已经把当前题型所需数量遍历了一遍
        // {
        //     int val = currentPanelQuestionIndexHead.val;
        //     int j = 0;//列数下标
        //     LevelData[val] = new LevelData(
        //         questionType,
        //         dt.Rows[head.val][j++].ToString(),
        //         dt.Rows[head.val][j++].ToString(),
        //         int.Parse(dt.Rows[head.val][j++].ToString()),
        //         int.Parse(dt.Rows[head.val][j++].ToString()),
        //         dt.Rows[head.val][j++].ToString(),
        //         dt.Rows[head.val].ItemArray.ToArray().Skip(j).Cast<string>().ToList()
        //     );
        //     currentPanelQuestionIndexHead = currentPanelQuestionIndexHead.next;
        //     head = head.next;
        // }
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