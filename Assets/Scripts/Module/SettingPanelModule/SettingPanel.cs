using Framework.Core;


public class SettingPanel : PanelBase
{
    public override void Show()
    {
        Bind();
    }

    public override void Bind()
    {
        OnClick("ExitButton", ExitButtonOnClick);
    }
    private void ReStartButtonOnClick()
    {

    }
    private void HomeButtonOnClick()
    {

    }

    private void ExitButtonOnClick()
    {
        Hide();
    }
    private void QuestionKeyButtonOnClick()
    {

    }

}