using Framework.Core;
using Manager;
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

    private void ReStartButtonOnClick()
    {
        UIManager.Instance.HideAllPanel();
        GameStart.Instance.GameReStart();
    }
    private void HomeButtonOnClick()
    {
        UIManager.Instance.HideAllPanel();
        GameStart.Instance.BackToMainPanel();
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