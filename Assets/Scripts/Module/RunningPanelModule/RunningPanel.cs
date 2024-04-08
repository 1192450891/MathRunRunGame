using System;
using System.IO;
using Framework.Core;
using GameBase.Player;
using Module.Enum;
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
    private GameObject questionBackground;
    private GameObject scoreNumText;
    private GameObject speedNumText;
    private GameObject answerTipText;
    private JourneyProgressBar bar;
    private int barCountdownTimeIndex=-1;
    
    private const int imageWidth=400;
    public override void Show()
    {
        Bind();
        questionBackground.SetActive(true);
        SetScoreNumText(0);
        SetSpeedNumText(GameStaticData.InitSpeedNum);
        barCountdownTimeIndex=TimeTool.Instance.Countdown(0.1f, SetJourneyProgressBar);
    }

    public override void Bind()
    {
        questionText = TransformUtil.Find(PanelObj.transform, "QuestionText").gameObject;
        questionImage =TransformUtil.Find(PanelObj.transform, "QuestionImage").gameObject;
        questionBackground =TransformUtil.Find(PanelObj.transform, "QuestionBackground").gameObject;
        scoreNumText = GameObject.Find("ScoreSquare/Square/NumText");
        speedNumText = GameObject.Find("SpeedSquare/Square/NumText");
        bar = new JourneyProgressBar(GameObject.Find("JourneyProgressBar/Slider").GetComponent<Slider>());
        SetAnswerTipSquare();
        
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
        string questionStr = levelData.Question;
        if (questionStr ==StaticString.NullStr)
        {
            questionText.SetActive(false);
            questionImage.SetActive(true);
            Util.Instance.SetImageWithWidth(questionImage.GetComponent<RawImage>(),levelData.QuestionImagePath,imageWidth,Util.WidthMode.MaxWidth);
        }
        else//文字类型
        {
            questionText.SetActive(true);
            questionImage.SetActive(false);
            SetQuestionText(questionStr);
        }

        SetAnswerTip(levelData.Way);
    }
    void SetQuestionText(string str)
    {
        if(questionText==null)return;
        questionText.GetComponent<TextMeshProUGUI>().text = str;
    }

    private void ClearQuestion()
    {
        questionText.SetActive(false);
        questionImage.SetActive(false);
        questionBackground.SetActive(false);
        if (answerTipText)
        {
            answerTipText.SetActive(false);
        }
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
    private void SetAnswerTip(int index)
    {
        if (!GameStart.Instance.DevelopToggle)return;
        answerTipText.GetComponent<TextMeshProUGUI>().text = $"正确道路：{index}";
    }
    private void SetAnswerTipSquare()
    {
        var squarePanel=TransformUtil.Find(PanelObj.transform, "AnswerTipSquare").gameObject;
        if (GameStart.Instance.DevelopToggle)
        {
            
            answerTipText=TransformUtil.Find(squarePanel.transform, "AnswerTipText").gameObject;
        }
        else
        {
            squarePanel.SetActive(false);
        }
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