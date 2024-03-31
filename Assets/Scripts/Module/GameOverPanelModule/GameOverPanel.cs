using Framework.Core;
using GameBase;
using Manager;
using Struct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameOverPanel : PanelBase
{
    public override void Show()
    {
        SetFinalScoreNumText();
        OnClick("ReStartButton", ReStartButtonOnClick);
        OnClick("HomeButton", HomeButtonOnClick);
        OnClick("ExitButton", ExitButtonOnClick);
        OnClick("QuestionKeyButton", QuestionKeyButtonOnClick);
    }

    private void SetFinalScoreNumText()
    {
        var numText=GameObject.Find("Background/ScoreText/ScoreNumText");
        if(numText==null)return;
        numText.GetComponent<TextMeshProUGUI>().text = ScoreManager.Instance.Score.ToString();
    }

    private void ReStartButtonOnClick()//直接开始游戏
    {
        GameOverClass.Instance.GameOver();
        GameStaticData.GameHasStart = true;
        UIManager.Instance.ShowPanel<RunningPanel>();
    }
    private void HomeButtonOnClick()
    {
        GameOverClass.Instance.GameOver();
        UIManager.Instance.ShowPanel<MainPanel>();

    }

    private void ExitButtonOnClick()
    {
        Hide();
    }
    private void QuestionKeyButtonOnClick()
    {
        Hide();
        UIManager.Instance.ShowPanel<QuestionKeyPanel>();
    }

}