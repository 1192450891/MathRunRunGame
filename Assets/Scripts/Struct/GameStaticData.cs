using Manager;

namespace Struct
{
    public struct GameStaticData
    {
        public static bool GameHasStart=false;
        
        public static bool GameHasEnd=false;
        
        public static bool PlayerIsPlaying()//检测玩家是否正在跑道上闯关
        {
            return GameHasStart&&!GameHasEnd;
        }
        
        private static float sumJourneyLength;
        public static float SumJourneyLength
        {
            get
            {
                if (sumJourneyLength == 0)
                {
                    sumJourneyLength=QuestionController.Instance.QuestionAmount * 60 *RunwayManager.RUNWAY_LENGTH_MAGNIFICATION+30;//最后加上终点的偏移
                }
                return sumJourneyLength;
            }
        }
        
        public static bool CanOperate;//玩家当前是否能进行操作
        
        public static float InitSpeedNum=20;//玩家开局初始速度
        
        public static float MinWalkSpeed=20;
        public static float MaxWalkSpeed=100;

    }
}