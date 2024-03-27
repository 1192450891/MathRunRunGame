using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Core;
using Manager;
using TMPro;
using UnityEngine;

public class QuestionKeyPanel  : PanelBase
{
    private GameObject content;//滚动条内容obj
    public override void Show()
    {
        content = PanelObj.transform.Find("Scroll/Content").gameObject;
        OnClick("ExitButton", ExitButtonOnClick);
        InitKeyCells();
    }

    private void InitKeyCells()
    {
        foreach (var questionKey in RunwayManager.Instance.questionKeyData)
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("KeyCell", "Assets/Prebs/UI/QuestionKeyPanel/KeyCell.prefab", content.transform,
                cell =>
                {
                    cell.transform.Find("Background/KeyText").GetComponent<TextMeshProUGUI>().text = questionKey;
                });
        }
    }

    private void ExitButtonOnClick()
    {
        Hide();
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }
}
