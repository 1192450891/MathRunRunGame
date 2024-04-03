namespace Manager
{
    public class ScoreManager : MonoSingleton<ScoreManager>
    {
        private int score;//本局游戏累积的积分
        public int Score
        {
            get => score;
            private set
            {
                score = value;
                EventManager.Instance.TriggerEvent<int>(ClientEvent.RunningPanel_ScoreChange,score);
            }
        } 


        public void AddScore()
        {
            Score += QuestionController.Instance.CurLevelData.score;
        }

        public void ReStart()
        {
            Score = 0;
        }
    }
}