using Manager;
using Struct;

namespace GameBase
{
    public class GameOverClass : Singleton<GameOverClass>
    {
        public void GameOver()//点击返回主页或重新开始 一局游戏才真正结束了
        {
            UIManager.Instance.HideAllPanel();
            QuestionController.Instance.ReStart();//重置参数
            Player.Instance.ReStart();
            GameStaticData.GameHasStart = false;
            GameStaticData.GameHasEnd = false;
            ScoreManager.Instance.ReStart();
        
            RunwayManager.Instance.InitRunways();
            RunwayManager.Instance.DestoryFinishLine();
        }
    }
}