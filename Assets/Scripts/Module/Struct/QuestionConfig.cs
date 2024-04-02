using System.Collections.Generic;
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
        public QuestionTypeEnum questionType;//题目题型   1：判断题 2:2选1 3:3选1
        public string id;
        public List<string> answers; //显示在门上的答案
        public string question; //显示在屏幕上的问题
        public int way; //实际左右的答案数组 取值0/1;
        public string questionKey;//问题题解
        public int score;//分数奖励
    }
}