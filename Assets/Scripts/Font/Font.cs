using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class Font : MonoBehaviour
{
    public Text text;
    public TMP_Text tmpText;
    void Start()
    {
            WX.InitSDK((ret) =>
        {
            // fallbackFont作为旧版本微信或者无法获得系统字体文件时的备选CDN URL
            // 「注意」需要替换成游戏真实的字体URL！！
            var fallbackFont = "https://www.unicode.org/charts/PDF/U0000.pdf" + "fallback.ttf"; 
            WX.GetWXFont(fallbackFont, (font) =>
            {
                text.font = font;
                tmpText.font = TMP_FontAsset.CreateFontAsset(font);
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
