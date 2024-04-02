using System;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private Dictionary<Type, PanelBase> _panelDic = new Dictionary<Type, PanelBase>();
    
    public T ShowPanel<T>() where T : PanelBase
    {
        return (T)ShowPanel(typeof(T),typeof(T).ToString());
    }
    
    private PanelBase ShowPanel(Type panelType,string panelName)
    {
        if (_panelDic.TryGetValue(panelType, out var panel)) return panel;
        panel = (PanelBase) Activator.CreateInstance(panelType);
        _panelDic[panelType] = panel;
        
         LoadManager.Instance.LoadAndShowPrefabAsync(panelName,
            $"Assets/Prebs/UI/{panelName}/{panelName}.prefab", transform, (pObj) =>
            {
                panel.PanelObj = pObj;
                panel.Init();
                panel.Show();
            });
        return panel;
    }
    
    public TipsPanel ShowTipsPanel(string tipText)
    {
        if (_panelDic.TryGetValue(typeof(TipsPanel), out var panel)) return (TipsPanel)panel;
        var newTipsPanel = (TipsPanel) Activator.CreateInstance(typeof(TipsPanel));
        _panelDic[typeof(TipsPanel)] = newTipsPanel;
        LoadManager.Instance.LoadAndShowPrefabAsync("TipsPanel",
            $"Assets/Prebs/UI/TipsPanel/TipsPanel.prefab", transform, (pObj) =>
            {
                newTipsPanel.PanelObj = pObj;
                newTipsPanel.TipText = tipText;
                newTipsPanel.Init();
                newTipsPanel.Show();
            });
        return newTipsPanel;
    }
    
    public void HidePanel<T>() where T : PanelBase
    {
        HidePanel(typeof(T));
    }
    public void HidePanel(Type panelType)
    {
        if (_panelDic.TryGetValue(panelType, out var panel))
        {
            panel.BeforeHide();
            panel.PanelObj.SetActive(false);
            Destroy(panel.PanelObj);
            _panelDic[panelType] = null;
            _panelDic.Remove(panelType);
        }
    }
    public void HideAllPanel()
    {
        foreach (var (key, value) in _panelDic)
        {
            value.BeforeHide();
            value.PanelObj.SetActive(false);
            Destroy(value.PanelObj);
        }
        _panelDic = new Dictionary<Type, PanelBase>();

        // foreach (KeyValuePair<Type,PanelBase> keyValuePair in _panelDic)
        // {
        //     keyValuePair.Value.PanelObj.SetActive(false);
        //     Destroy(keyValuePair.Value.PanelObj);
        //     _panelDic[keyValuePair.Key] = null;
        //     _panelDic.Remove(keyValuePair.Key);
        // }
    }
    private void Start()
    {
        ShowPanel<MainPanel>();
        // ShowPanel<QuestionKeyPanel>();
    }
}