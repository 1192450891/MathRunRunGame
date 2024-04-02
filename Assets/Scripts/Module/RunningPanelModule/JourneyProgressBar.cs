using GameBase.Player;
using Manager;
using Struct;
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
        }

        public void SetBar()
        {
            float fixedValue = (Player.Instance.transform.position.z - 30) / GameStaticData.SumJourneyLength;//需要减去起点的偏移
            slider.value = fixedValue<0?0:fixedValue;
        }
    }
}