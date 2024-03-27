using Framework.Core;
using Module.RunningPanelModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunningPanel : PanelBase
{
    private GameObject ScoreNumText;
    private GameObject SpeedNumText;
    private JourneyProgressBar bar;
    private int barCountdownTimeIndex=-1;
    public override void Show()
    {
        ScoreNumText = GameObject.Find("ScoreSquare/Square/NumText");
        SpeedNumText = GameObject.Find("SpeedSquare/Square/NumText");
        bar = new JourneyProgressBar(GameObject.Find("JourneyProgressBar/Slider").GetComponent<Slider>());
        EventManager.Instance.AddEvent<int>(ClientEvent.RunningPanel_ScoreChange, SetScoreNumText);
        EventManager.Instance.AddEvent<float>(ClientEvent.RunningPanel_SpeedChange, SetSpeedNumText);
        
        CharacterLocomotion.Instance.moveJoystick=TransformUtil.Find(PanelObj.transform, "Joystick").transform.GetComponent<FixedJoystick>();
        CharacterLocomotion.Instance.canOperate = true;

        QuestionController.Instance.questionText = TransformUtil.Find(PanelObj.transform, "QuestionText").gameObject;

        SetScoreNumText(0);
        SetSpeedNumText(Player.Instance.startrationRate);

        barCountdownTimeIndex=TimeTool.Instance.Countdown(0.1f, () => SetJourneyProgressBar());
    }

    private void SetJourneyProgressBar()
    {
        bar.SetBar();
    }

    void SetScoreNumText(int score)
    {
        if(ScoreNumText==null)return;
        ScoreNumText.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }
    void SetSpeedNumText(float walkSpeed)
    {
        if(SpeedNumText==null)return;
        SpeedNumText.GetComponent<TextMeshProUGUI>().text = walkSpeed.ToString();
    }

    public override void Hide()
    {
        CharacterLocomotion.Instance.canOperate = false;
        EventManager.Instance.RemoveEvent<int>(ClientEvent.RunningPanel_ScoreChange, SetScoreNumText);
        EventManager.Instance.RemoveEvent<float>(ClientEvent.RunningPanel_SpeedChange, SetSpeedNumText);
        
        if(barCountdownTimeIndex!=-1)TimeTool.Instance.RemoveTimeEvent(barCountdownTimeIndex);
    }
}