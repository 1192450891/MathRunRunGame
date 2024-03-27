using Framework.Core;
using TMPro;

public class TipsPanel : PanelBase
{
    public string TipText;
    public override void Show()
    {
        TransformUtil.Find(PanelObj.transform, "TipsText").GetComponent<TextMeshProUGUI>().text = TipText;
        TimeTool.Instance.Delay(3f, ()=>Hide());
    }
}