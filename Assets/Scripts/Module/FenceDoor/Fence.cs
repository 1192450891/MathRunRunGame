using Framework.Core;
using GameBase.Player;
using Module.Enum;
using Struct;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BrokenVector.LowPolyFencePack
{
    public class Fence
    {
        private Transform fenceTransform;
        private LevelData fenceLevelData;

        private FenceAni fenceAni0;
        private FenceAni fenceAni1;
        private FenceAni fenceAni2;
        public Fence(Transform transform, LevelData levelData)
        {
            fenceTransform = transform;
            fenceLevelData = levelData;
            TransformUtil.Find(transform, "0 Fence Type1 Door").AddComponent<FenceAni>();
            TransformUtil.Find(transform, "1 Fence Type1 Door").AddComponent<FenceAni>();
            if(levelData.QuestionType==QuestionTypeEnum.ThreeAnswerQuestion)TransformUtil.Find(transform, "2 Fence Type1 Door").AddComponent<FenceAni>();
            FillFenceAnswer(transform,levelData);
            if (levelData.QuestionType == QuestionTypeEnum.TrueOrFalse)return;//判断题不需要调整答案位置
            AdjustAnswerPosition(transform,levelData);
        }
        
        private void FillFenceAnswer(Transform transform,LevelData runwayData)
        {
            for (int i = 0; i < runwayData.Answers.Count; i++)
            {
                if (runwayData.Answers[i] == "null")//图片类型
                {

                    Transform canvasTransform = TransformUtil.Find(transform,$"{i} RawImage");
                    canvasTransform.gameObject.SetActive(true);
                    var rawImageComponent = canvasTransform.GetChild(0).GetComponent<RawImage>();
                    rawImageComponent.texture = Util.Instance.LoadPNG(GetRawImageSourcePath(runwayData,i));
                }
                else//文字类型
                {
                    Transform textTransform = TransformUtil.Find(transform,$"{i} Text");
                    textTransform.gameObject.SetActive(true);
                    var textMeshComponent = textTransform.GetComponent<TMP_Text>();
                    textMeshComponent.text = runwayData.Answers[i];
                }
            } 
            string GetRawImageSourcePath(LevelData levelData,int i)
            {
                char suffix = ' ';
                switch (i)
                {
                    case 0:
                        suffix = 'a';
                        break;
                    case 1:
                        suffix = 'b';
                        break;
                    case 2:
                        suffix = 'c';
                        break;
                }
                return $"{levelData.ID + suffix}";
            }
        }
        
        private void AdjustAnswerPosition(Transform transform,LevelData runwayData)
        {
            //判断题的T在位置0 F在位置1
            int correctWay = runwayData.Way; //正确的答案 从左往右 从0开始 每次交换0号和目标位置的木板
            
            if (correctWay != 0)
            {
                var zeroTextTransform = TransformUtil.Find(transform,"0 Text").transform;
                var targetTextTransform = TransformUtil.Find(transform,$"{correctWay} Text").transform;
                Util.Instance.SwapParent(zeroTextTransform,targetTextTransform);

                (zeroTextTransform.position, targetTextTransform.position) =
                    (targetTextTransform.position, zeroTextTransform.position);
                
                var zeroRawImageTransform = TransformUtil.Find(transform,"0 RawImage").transform;
                var targetRawImageTransform = TransformUtil.Find(transform,$"{correctWay} RawImage").transform;
                Util.Instance.SwapParent(zeroRawImageTransform,targetRawImageTransform);

                (zeroRawImageTransform.position, targetRawImageTransform.position) =
                    (targetRawImageTransform.position, zeroRawImageTransform.position);
            }
        }

    }
}