using System;
using System.IO;
using Framework.Core;
using GameBase.Player;
using Module.RunningPanelModule;
using Struct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx;

public class RunningPanel : PanelBase
{
    private GameObject questionText;
    private GameObject questionImage;
    private GameObject scoreNumText;
    private GameObject speedNumText;
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
        questionText = TransformUtil.Find(PanelObj.transform, "QuestionText").gameObject;
        questionImage =TransformUtil.Find(PanelObj.transform, "QuestionImage").gameObject;
        scoreNumText = GameObject.Find("ScoreSquare/Square/NumText");
        speedNumText = GameObject.Find("SpeedSquare/Square/NumText");
        bar = new JourneyProgressBar(GameObject.Find("JourneyProgressBar/Slider").GetComponent<Slider>());
        
        EventManager.Instance.AddEvent<LevelData>(ClientEvent.QuestionController_NextQuestion, NextQuestionRefresh);
        EventManager.Instance.AddEvent(ClientEvent.QuestionController_AllQuestionDone, ClearQuestion);
        EventManager.Instance.AddEvent<int>(ClientEvent.RunningPanel_ScoreChange, SetScoreNumText);
        EventManager.Instance.AddEvent<float>(ClientEvent.RunningPanel_SpeedChange, SetSpeedNumText);

        Player.Instance.SetJoyStick(TransformUtil.Find(PanelObj.transform, "Joystick").transform.GetComponent<Joystick>());
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
            questionText.SetActive(false);
            questionImage.SetActive(true);
            string str = null;
            switch (levelData.questionType)
            {
                case QuestionTypeEnum.TrueOrFalse:
                    str = StaticString.TrueOrFalseQuestionImage;
                    break;
                case QuestionTypeEnum.TwoAnswerQuestion:
                    str = StaticString.TwoAnswerQuestionImage;
                    break;
                case QuestionTypeEnum.ThreeAnswerQuestion:
                    str = StaticString.ThreeAnswerQuestionImage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SetQuestionImage(Application.dataPath+"/CSVData/"+str+$"/{levelData.id}.jpeg");
        }
        else//文字类型
        {
            questionText.SetActive(true);
            questionImage.SetActive(false);
            SetQuestionText(questionStr);
        }
    }
    void SetQuestionText(string str)
    {
        if(questionText==null)return;
        questionText.GetComponent<TextMeshProUGUI>().text = str;
    }
    void SetQuestionImage(string imagePath)
    {
        if(questionImage==null)return;
        var texture=Util.Instance.LoadPNG(imagePath);
        var rawImageComponent = questionImage.GetComponent<RawImage>();
        rawImageComponent.texture = texture;
        rawImageComponent.SetNativeSize();
    }
    private void ClearQuestion()
    {
        questionText.SetActive(false);
        questionImage.SetActive(false);
    }
    void SetScoreNumText(int score)
    {
        if(scoreNumText==null)return;
        scoreNumText.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }
    void SetSpeedNumText(float walkSpeed)
    {
        if(speedNumText==null)return;
        speedNumText.GetComponent<TextMeshProUGUI>().text = walkSpeed.ToString();
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