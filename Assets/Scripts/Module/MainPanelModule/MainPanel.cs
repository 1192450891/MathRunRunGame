using Framework.Core;
using Struct;
using UnityEngine;

public class MainPanel:PanelBase
{
    private GameObject _startButton;
    private GameObject _shareButton;
    private GameObject _rankButton;
    private GameObject _settingButton;
    public override void Show()
    {
        Bind();
    }

    public override void Bind()
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
        GameStaticData.GameHasStart = true;
        QuestionController.Instance.ManualStart();
    }
}