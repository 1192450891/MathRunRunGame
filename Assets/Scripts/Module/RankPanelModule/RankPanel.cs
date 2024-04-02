using Framework.Core;
using UnityEngine;

public class RankPanel:PanelBase
{
    private GameObject _exitButton;
    public override void Show()
    {
        Bind();
    }
    public override void Bind()
    {
        OnClick("ExitButton", ExitButtonOnClick);
    }
    private void ExitButtonOnClick()
    {
        Hide();
        UIManager.Instance.ShowPanel<MainPanel>();
    }
}