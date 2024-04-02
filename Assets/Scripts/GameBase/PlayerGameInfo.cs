using System.Collections.Generic;

namespace GameBase
{
    public class PlayerGameInfo
    {
        public string PlayerID { get; set; }
        public int Score { get; set; }
        public float MaxSpeed{ get; set; }
        
        public int CorrectNum{ get; set; }
        public string Difficulty { get; set; }
        
        public List<string> CorrectQuestionIdList;//本局答对的题目ID列表
        public List<string> WrongQuestionIdList;//本局答错的题目ID列表
    }
}