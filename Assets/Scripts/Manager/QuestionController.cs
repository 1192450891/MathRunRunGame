using System;
using System.Collections.Generic;
using System.Data;
using Manager;
using TMPro;
using UnityEngine;
using Wx;

public class QuestionController : MonoSingleton<QuestionController>
{
    public GameObject questionText;
    
    public int QuestionAmount;//各类型问题数量总和 即玩家过关需要通过的问题数量
    
    [HideInInspector] public int currentPanelQuestionIndex; //用于切换画面上显示的问题

    private ListNode head;

    // [Tooltip("栅栏透明材质球")] [SerializeField] public Material FenceFade;
    private bool firstTime; //第一次不更新跑道

    private bool hasEndLine;

    private bool hasGetData;
    
    private QuestionConfig questionConfig;


    
    public LevelData[] levelData;

    public struct LevelData
    {
        public QuestionTypeEnum questionType;//题目题型   1：判断题 2:2选1 3:3选1
        public string _id;
        public List<string> answers; //显示在门上的答案
        public string question; //显示在屏幕上的问题
        public int way; //实际左右的答案数组 取值0/1;
        public string questionKey;//问题题解
        public int score;//分数奖励
    }

    void Awake()
    {
        GetData();
    }

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
        head = GenerateRandomLinkedList(QuestionAmount - 1);
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
            levelData[i].questionType = questionType;
            levelData[i].question = dt.Rows[head.val][0].ToString();//第一列题干
            levelData[i].way = int.Parse(dt.Rows[head.val][1].ToString());//第二列正确答案位置
            levelData[i].score=int.Parse(dt.Rows[head.val][2].ToString());//第三列分数
            levelData[i].questionKey = dt.Rows[head.val][3].ToString();//第四列题解内容
            levelData[i].answers = new List<string>();
            int start = 4;
            while (start<dt.Rows[head.val].ItemArray.Length)
            {
                levelData[i].answers.Add(dt.Rows[head.val][start].ToString());//start下标是正确答案 后面是错误答案
                start++;
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
            TimeTool.Instance.Delay(0.5f, NextQuestion );
            hasGetData = false;
        }
    }

    void Update()
    {
    }

    public void NextQuestion()
    {
        if (currentPanelQuestionIndex >= QuestionAmount)
        {
            questionText.GetComponent<TextMeshProUGUI>().text = "";
            return;
        }
        
        questionText.GetComponent<TextMeshProUGUI>().text = levelData[currentPanelQuestionIndex++].question;
    }

    public void ClearQuestion()
    {
        questionText.GetComponent<TextMeshProUGUI>().text = "";
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
    
    public struct QuestionConfig
    {
        public int TrueOrFalseQuestionCount;
        public int TwoAnswerQuestionCount;
        public int ThreeAnswerQuestionCount;
        // public int FourAnswerQuestionCount;
    }

    public class ListNode
    {
        public int val;
        public ListNode next;

        public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;
        }
    }

    public ListNode GenerateRandomLinkedList(int upperLimit)
    {
        // 创建链表并填充数字
        ListNode head = null;
        ListNode current = null;
        List<int> numbers = new List<int>();

        for (int i = 0; i <= upperLimit; i++)
        {
            numbers.Add(i);
        }

        // 使用 Fisher-Yates 洗牌算法打乱数字顺序
        System.Random random = new System.Random();
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }

        // 将打乱后的数字填充到链表中
        foreach (int number in numbers)
        {
            if (head == null)
            {
                head = new ListNode(number);
                current = head;
            }
            else
            {
                current.next = new ListNode(number);
                current = current.next;
            }
        }

        return head;
    }
}