using Manager;
using UnityEngine.UI;

namespace Module.RunningPanelModule
{
    public class JourneyProgressBar
    {
        private Slider slider;
        public JourneyProgressBar(Slider slider1)
        {
            slider = slider1;
            slider.value = 0;
            SumJourneyLength = QuestionController.Instance.QuestionAmount * 60 *RunwayManager.RUNWAY_LENGTH_MAGNIFICATION+30;//最后加上终点的偏移
        }
        
        private float SumJourneyLength;

        public void SetBar()
        {
            float fixedValue = (Player.Instance.transform.position.z - 30) / SumJourneyLength;//减去起点的偏移
            slider.value = fixedValue<0?0:fixedValue;
        }
    }
}