using Framework.Core;
using Struct;
using WeChatWASM;

public class MainPanel:PanelBase
{
    public override void Show()
    {
        Bind();
    }

    public override void Bind()
    {
        OnClick("StartButton", StartButtonOnClick);
        OnClick("ShareButton", ShareButtonOnClick);
        OnClick("SettingButton", SettingButtonOnClick);
        // OnClick("RankButton", RankButtonOnClick);
    }

    private void RankButtonOnClick()
    {
        Hide();
        UIManager.Instance.ShowPanel<RankPanel>();
    }

    private void SettingButtonOnClick()
    {
        UIManager.Instance.ShowPanel<SettingPanel>();
    }

    private void ShareButtonOnClick()
    {
        UIManager.Instance.ShowTipsPanel("功能还没有开发完成噢");
    }

    private void StartButtonOnClick()
    {
        Hide();
        UIManager.Instance.HideAllPanel();
        UIManager.Instance.ShowPanel<RunningPanel>();
        GameStaticData.GameHasStart = true;
        QuestionController.Instance.ManualStart();
    }
}