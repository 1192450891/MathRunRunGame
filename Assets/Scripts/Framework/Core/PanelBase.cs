using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Core
{
    public class PanelBase
    {
        public GameObject PanelObj;
        
        public delegate void EventCallback0();
        /// <summary>
        /// 点击事件数据
        /// </summary>
        private Dictionary<GameObject, EventCallback0> _onClicks;

        public void Init()
        {
            // PanelObj.transform.position = Vector2.zero;
        }
        public virtual void Show()
        {
            
        }
        
        public virtual void Hide()
        {
            UIManager.Instance.HidePanel(GetType());
        }

        /// <summary>
        /// 控件添加点击事件,界面关闭自动释放
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="callback"></param>
        public void OnClick(string objName, UnityEngine.Events.UnityAction callback)
        {
            GameObject.Find(objName).gameObject.GetComponent<Button>().onClick.AddListener(callback);
        }

    }

}