using System.Collections.Generic;
using Module.Enum;
using Wx;

namespace Struct
{
    public struct QuestionConfig
    {
        public int TrueOrFalseQuestionCount;
        public int TwoAnswerQuestionCount;
        public int ThreeAnswerQuestionCount;
        // public int FourAnswerQuestionCount;
    }
    
    public struct LevelData
    {
        public QuestionTypeEnum QuestionType;//题目题型   1：判断题 2:2选1 3:3选1
        public string ID;
        public List<string> Answers; //显示在门上的答案
        public string Question; //显示在屏幕上的问题
        public string QuestionImagePath; //图片类型问题的资源路径 初始化时赋值 文字类型时这里是空字符串
        public int Way; //实际左右的答案数组 取值0/1;
        public int Score;//分数奖励
        public string QuestionKey;//问题题解
        public LevelData(QuestionTypeEnum questionType, string id, string question, int way, int score, string questionKey,List<string> answers) : this()
        {
            QuestionType = questionType;
            ID = id;
            Answers = new List<string>();
            Question = question;
            Way = way;
            QuestionKey = questionKey;
            Score = score;
            
            if (question==StaticString.NullStr)
            {
                QuestionImagePath = $"{id}";
            }
            else
            {
                QuestionImagePath = "";
            }
            Answers=answers;//第一个是正确答案 后面是错误答案
        }
    }
}