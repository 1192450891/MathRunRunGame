using Manager;
using Struct;

namespace GameBase
{
    public class GameOverClass : Singleton<GameOverClass>
    {
        public void GameOver()//点击返回主页或重新开始 一局游戏才真正结束了
        {
            InitGameInfo();//创建玩家本局游戏信息记录
            UIManager.Instance.HideAllPanel();
            QuestionController.Instance.ReStart();//重置参数
            Player.Instance.ReStart();
            GameStaticData.ReStart();
            ScoreManager.Instance.ReStart();
        
            RunwayManager.Instance.InitRunways();
            RunwayManager.Instance.DestoryFinishLine();
        }

        private void InitGameInfo()
        {
            var info =new PlayerGameInfo
            {
                PlayerID = null,
                Score = ScoreManager.Instance.Score,
                MaxSpeed = GameStaticData.MaxWalkSpeed,
                CorrectNum=GameStaticData.HasCorrectNum,
                Difficulty = null,
                CorrectQuestionIdList=GameStaticData.CorrectQuestionIdList,
                WrongQuestionIdList= GameStaticData.WrongQuestionIdList,
            };
        }
    }
}