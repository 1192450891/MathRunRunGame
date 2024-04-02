using System;
using System.IO;
using Framework.Core;
using Module.RunningPanelModule;
using Struct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx;

public class RunningPanel : PanelBase
{
    private GameObject QuestionText;
    private GameObject QuestionImage;
    private GameObject ScoreNumText;
    private GameObject SpeedNumText;
    private JourneyProgressBar bar;
    private int barCountdownTimeIndex=-1;
    public override void Show()
    {
        Bind();

        SetScoreNumText(0);
        SetSpeedNumText(GameStaticData.InitSpeedNum);

        barCountdownTimeIndex=TimeTool.Instance.Countdown(0.1f, () => SetJourneyProgressBar());
    }
    
    public override void Bind()
    {
        QuestionText = TransformUtil.Find(PanelObj.transform, "QuestionText").gameObject;
        QuestionImage =TransformUtil.Find(PanelObj.transform, "QuestionImage").gameObject;
        ScoreNumText = GameObject.Find("ScoreSquare/Square/NumText");
        SpeedNumText = GameObject.Find("SpeedSquare/Square/NumText");
        bar = new JourneyProgressBar(GameObject.Find("JourneyProgressBar/Slider").GetComponent<Slider>());
        
        EventManager.Instance.AddEvent<LevelData>(ClientEvent.QuestionController_NextQuestion, NextQuestionRefresh);
        EventManager.Instance.AddEvent(ClientEvent.QuestionController_AllQuestionDone, ClearQuestion);
        EventManager.Instance.AddEvent<int>(ClientEvent.RunningPanel_ScoreChange, SetScoreNumText);
        EventManager.Instance.AddEvent<float>(ClientEvent.RunningPanel_SpeedChange, SetSpeedNumText);
        
        Player.Instance.characterLocomotion.moveJoystick=TransformUtil.Find(PanelObj.transform, "Joystick").transform.GetComponent<Joystick>();
        GameStaticData.CanOperate = true;
    }

    private void SetJourneyProgressBar()
    {
        bar.SetBar();
    }

    void NextQuestionRefresh(LevelData levelData)
    {
        string questionStr = levelData.question;
        if (questionStr =="null")
        {
            QuestionText.SetActive(false);
            QuestionImage.SetActive(true);
            string str = null;
            switch (levelData.questionType)
            {
                case QuestionTypeEnum.TrueOrFalse:
                    str = "TrueOrFalseQuestionImage";
                    break;
                case QuestionTypeEnum.TwoAnswerQuestion:
                    str = "TwoAnswerQuestionImage";
                    break;
                case QuestionTypeEnum.ThreeAnswerQuestion:
                    str = "ThreeAnswerQuestionImage";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SetQuestionImage(Application.dataPath+"/CSVData/"+str+$"/{levelData.id}.jpeg");
        }
        else//文字类型
        {
            QuestionText.SetActive(true);
            QuestionImage.SetActive(false);
            SetQuestionText(questionStr);
        }
    }
    void SetQuestionText(string str)
    {
        if(QuestionText==null)return;
        QuestionText.GetComponent<TextMeshProUGUI>().text = str;
    }
    void SetQuestionImage(string imagePath)
    {
        if(QuestionImage==null)return;
        var texture=Util.Instance.LoadPNG(imagePath);
        var rawImageComponent = QuestionImage.GetComponent<RawImage>();
        rawImageComponent.texture = texture;
        rawImageComponent.SetNativeSize();
    }
    private void ClearQuestion()
    {
        QuestionText.SetActive(false);
        QuestionImage.SetActive(false);
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

    public override void BeforeHide()
    {
        GameStaticData.CanOperate = false;
        EventManager.Instance.RemoveEvent<LevelData>(ClientEvent.QuestionController_NextQuestion, NextQuestionRefresh);
        EventManager.Instance.RemoveEvent(ClientEvent.QuestionController_AllQuestionDone);
        EventManager.Instance.RemoveEvent<int>(ClientEvent.RunningPanel_ScoreChange, SetScoreNumText);
        EventManager.Instance.RemoveEvent<float>(ClientEvent.RunningPanel_SpeedChange, SetSpeedNumText);
        
        if(barCountdownTimeIndex!=-1)TimeTool.Instance.RemoveTimeEvent(barCountdownTimeIndex);
    }
}