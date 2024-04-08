using UnityEngine;

namespace Struct
{
    public static class StaticString//多个地方用到的字符串放这里
    {
        public const string NullStr = "null";
        
        public const string TrueOrFalseQuestionImage = "TrueOrFalseQuestionImage";
        public const string TwoAnswerQuestionImage= "TwoAnswerQuestionImage";
        public const string ThreeAnswerQuestionImage= "ThreeAnswerQuestionImage";
            
        public static readonly string CsvDataPath= Application.dataPath+"/CSVData/";
    }
}