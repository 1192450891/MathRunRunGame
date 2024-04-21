using System.Collections.Generic;

namespace GameBase
{
    public class PlayerGameInfo
    {
        // public string PlayerID;
        public int Score;
        public float MaxSpeed;

        public int CorrectNum;
        
        public List<string> CorrectQuestionIdList;//本局答对的题目ID列表
        public List<string> WrongQuestionIdList;//本局答错的题目ID列表
    }
}