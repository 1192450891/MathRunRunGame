using Manager;

namespace Struct
{
    public struct GameStaticData
    {
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
    }
}