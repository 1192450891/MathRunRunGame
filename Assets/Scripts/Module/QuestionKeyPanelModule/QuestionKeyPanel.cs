using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Framework.Core;
using Manager;
using Module.QuestionKeyPanelModule;
using Struct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionKeyPanel  : PanelBase
{
    private GameObject content;//滚动条内容obj
    private RectTransform contentRectTransform;
    


    // private float curItemIndex = 0;//当前显示的item下标
    private float itemOffsetPosX;

    public override void Show()
    {
        Bind();
        InitKeyCells();
        contentRectTransform.localPosition = new Vector3(300, contentRectTransform.localPosition.y, 0);
    }
    
    public override void Bind()
    {
        content =TransformUtil.Find(PanelObj.transform,"Content").gameObject;
        itemOffsetPosX=content.GetComponent<HorizontalLayoutGroup>().spacing-20;
        contentRectTransform = TransformUtil.Find(PanelObj.transform, "Content").GetComponent<RectTransform>();
        OnClick("ExitButton", ExitButtonOnClick);
        OnClick("NextButton", NextButtonOnClick);
        OnClick("LastButton", LastButtonOnClick);
    }



    private void InitKeyCells()
    {
        var levelDataNums = QuestionController.Instance.LevelData;
        for (var i = 0; i < levelDataNums.Length; i++)
        {
            var questionKeyCell = new QuestionKeyCell(content,levelDataNums[i],i+1);
        }
    }

    private const float offsetSumTime = 0.6f;
    private const float offsetRepeatTime = 0.02f;

    private void LastButtonOnClick()
    {
        DoOffset(itemOffsetPosX,offsetSumTime,offsetRepeatTime);
    }

    private void NextButtonOnClick()
    {
        DoOffset(-itemOffsetPosX,offsetSumTime,offsetRepeatTime);
    }

    private int offsetTimeIndex=-1;
    private void DoOffset(float sumOffset,float sumTime,float repeatTime)
    {
        if (offsetTimeIndex!=-1)
        {
            TimeTool.Instance.RemoveTimeEvent(offsetTimeIndex);
        }
        float eachOffset = sumOffset * (repeatTime/sumTime);
        float curTime = 0;
        offsetTimeIndex=TimeTool.Instance.Countdown(repeatTime,(() =>
        {
            var localPosition = contentRectTransform.localPosition;
            localPosition = new Vector3(localPosition.x+eachOffset,localPosition.y,0);
            contentRectTransform.localPosition = localPosition;
            curTime += repeatTime;
            if (curTime>=sumTime)
            {
                TimeTool.Instance.RemoveTimeEvent(offsetTimeIndex);
            }
        }));
    }
    
    private void ExitButtonOnClick()
    {
        Hide();
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }

    public override void BeforeHide()
    {
        TimeTool.Instance.RemoveTimeEvent(offsetTimeIndex);
    }
}
