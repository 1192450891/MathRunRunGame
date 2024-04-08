using Framework.Core;
using Struct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.QuestionKeyPanelModule
{
    public class QuestionKeyCell
    {
        private GameObject gameObject;
        private Transform questionIndexText;
        private Transform questionText;
        private Transform questionRawImage;
        private Transform keyText;
        private const string keyCellPath = "Assets/Prebs/UI/QuestionKeyPanel/KeyCell.prefab";
        
        private const int imageWidth=520;

        public QuestionKeyCell(GameObject content,LevelData data,int index)
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("KeyCell", keyCellPath, content.transform,
                cell =>
                {
                    gameObject = cell;
                    SetChild(data);
                    FillCell(data,index);
                });
        }

        private void SetChild(LevelData data)
        {
            var transform = gameObject.transform;
            questionIndexText = TransformUtil.Find(transform, "QuestionIndexText");
            questionText = TransformUtil.Find(transform, "QuestionText");
            questionRawImage = TransformUtil.Find(transform, "QuestionRawImage");
            keyText = TransformUtil.Find(transform, "KeyText");

            if (data.Question==StaticString.NullStr)//题目是图片类型
            {
                questionText.gameObject.SetActive(false);
                questionRawImage.gameObject.SetActive(true);
            }
            else
            {
                questionText.gameObject.SetActive(true);
                questionRawImage.gameObject.SetActive(false);
            }
        }

        private void FillCell(LevelData data,int index)
        {
            
            questionIndexText.GetComponent<TextMeshProUGUI>().text = $"题目{index}:";
            if (data.Question==StaticString.NullStr)//题目是图片类型
            {
                Util.Instance.SetImageWithWidth(questionRawImage.GetComponent<RawImage>(),data.QuestionImagePath,imageWidth,Util.WidthMode.CertainWidth);
            }
            else
            {
                questionText.GetComponent<TextMeshProUGUI>().text = data.Question;
                
            }
            keyText.GetComponent<TextMeshProUGUI>().text = data.QuestionKey;
        }
    }
}