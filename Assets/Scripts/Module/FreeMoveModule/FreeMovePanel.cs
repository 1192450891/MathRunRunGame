using Framework.Core;
using GameBase.Player;
using Struct;

public class FreeMovePanel : PanelBase
{

    public override void Show()
    {
        Bind();
        GameStaticData.CanOperate = true;
        GameStaticData.isFreeMoving = true;
    }
    public override void Bind()
    {
        OnClick("GameOverPanelButton", GameOverPanelButtonOnClick);
        Player.Instance.SetJoyStick(TransformUtil.Find(PanelObj.transform, "Joystick").transform.GetComponent<Joystick>());
    }

    private void GameOverPanelButtonOnClick()
    {
        Hide();
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }

    public override void BeforeHide()
    {
        GameStaticData.CanOperate = false;
        GameStaticData.isFreeMoving = false;
    }
}