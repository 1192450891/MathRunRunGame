using System.Data;

namespace Struct
{
    public class CsvStaticData
    {
        public static float ReadCsvDataTime = 2f;
        
        public static DataTable ConfigTable=new DataTable();
        
        public static DataTable TureOrFalseQuestionTable=new DataTable();
        
        public static DataTable TwoAnswerQuestionTable=new DataTable();
        
        public static DataTable ThreeAnswerQuestionTable=new DataTable();
        
        public static void SetCsvDataTable()//进入游戏时读一次存起来
        {
            LoadManager.Instance.LoadCsvAssetAsync("QuestionConfig",ConfigTable);
            LoadManager.Instance.LoadCsvAssetAsync("TureOrFalseQuestion",TureOrFalseQuestionTable);
            LoadManager.Instance.LoadCsvAssetAsync("TwoAnswerQuestion",TwoAnswerQuestionTable);
            LoadManager.Instance.LoadCsvAssetAsync("ThreeAnswerQuestion",ThreeAnswerQuestionTable);
        }
    }
}