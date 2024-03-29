using Framework.Core;
using UnityEngine;

public class MainPanel:PanelBase
{
    private GameObject _startButton;
    private GameObject _shareButton;
    private GameObject _rankButton;
    private GameObject _settingButton;
    public override void Show()
    {
        OnClick("StartButton", StartButtonOnClick);
        OnClick("ShareButton", ShareButtonOnClick);
        OnClick("SettingButton", SettingButtonOnClick);
        OnClick("RankButton", RankButtonOnClick);
    }

    private void RankButtonOnClick()
    {
        UIManager.Instance.HidePanel<MainPanel>();
        UIManager.Instance.ShowPanel<RankPanel>();
    }

    private void SettingButtonOnClick()
    {
        UIManager.Instance.ShowPanel<SettingPanel>();
    }

    private void ShareButtonOnClick()
    {
        UIManager.Instance.ShowTipsPanel("还没好");
    }

    private void StartButtonOnClick()
    {
        UIManager.Instance.HidePanel<MainPanel>();
        UIManager.Instance.ShowPanel<RunningPanel>();
        Player.Instance.SetHasStart(true);
        QuestionController.Instance.ManualStart();
    }
}