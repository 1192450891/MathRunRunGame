using System;
using System.Collections.Generic;
using Framework.Core;
using Module.Enum;
using Struct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx;
using Object = UnityEngine.Object;

namespace Manager
{
    public class RunwayManager : Singleton<RunwayManager>
    {
        private Queue<GameObject> runWays; //存储跑道 入队列时创建对应的栅栏 需要销毁时按出列顺序销毁

        private int levelDataIndex; //用于创建新跑道时的指针下标

        private const int gameStartInitRunwaysCount = 4; //初始化游戏时创建的跑道数量

        private GameObject runwaysGameObjectRoot; //跑道根节点

        public static float RUNWAY_LENGTH_MAGNIFICATION;//跑道长度倍率 （用于调整回答问题时间的大小）

        private GameObject finishLine;//终点线preb实例
        
        private bool hasEndLine;//是否已经生成终点

        private const string runwayAddressablePath = "Assets/Prebs/Environment/Runway.prefab";
        private const string finishLineAddressablePath =     "Assets/Prebs/Environment/Finish Line.prefab";
        private const string twoAnswerFenceAddressablePath ="Assets/Prebs/Environment/Fence/2AnswerFence.prefab" ;
        private const string threeAnswerFenceAddressablePath = "Assets/Prebs/Environment/Fence/3AnswerFence.prefab";

        public void InitRunways()
        {
            if (runWays != null)//点击了重新开始游戏 需要销毁前一局的跑道
            {
                hasEndLine = false;
                while (runWays.Count!=0)
                {
                   Object.Destroy(runWays.Dequeue().gameObject);
                }
            }
            else
            {
                runwaysGameObjectRoot = new GameObject("RunwaysGameObjectRoot");
            }
            runWays = new Queue<GameObject>();
            levelDataIndex = 0;
            for (int i = 0; i < gameStartInitRunwaysCount; i++)
            {
                CreateNewRunway();
            }
        }

        public void CreateNewRunway()
        {
            string objPath = ""; //这次的栅栏类型
            int index = levelDataIndex;
            levelDataIndex++;
            var curQuestionLevelData = QuestionController.Instance.LevelData[index];
            var curQuestionType = curQuestionLevelData.questionType;
            if (curQuestionType == QuestionTypeEnum.TrueOrFalse)
            {
                objPath = twoAnswerFenceAddressablePath;
            }
            else if (curQuestionType == QuestionTypeEnum.TwoAnswerQuestion)
            {
                objPath = twoAnswerFenceAddressablePath;
            }
            else if (curQuestionType == QuestionTypeEnum.ThreeAnswerQuestion)
            {
                objPath = threeAnswerFenceAddressablePath;
            }

            LoadManager.Instance.LoadAndShowPrefabAsync("Runways", runwayAddressablePath,
                runwaysGameObjectRoot.transform,
                obj =>
                {
                    RUNWAY_LENGTH_MAGNIFICATION = obj.transform.localScale.z;
                    obj.transform.position = new Vector3(0, 0, (2*index + 1) * 30*RUNWAY_LENGTH_MAGNIFICATION+30);
                    Transform fencePos = obj.transform.Find("FencePos");
                    LoadManager.Instance.LoadAndShowPrefabAsync("Fence", objPath, fencePos,
                        fence =>
                        {
                            fence.transform.position = fencePos.position;
                            InitFence(fence.transform, curQuestionLevelData);
                        });
                    runWays.Enqueue(obj);
                });
        }

        private void InitFence(Transform transform, LevelData runwayData)
        {
            FillFenceAnswer(transform,runwayData);
            if (runwayData.questionType == QuestionTypeEnum.TrueOrFalse)return;//判断题不需要调整答案位置
            AdjustAnswerPosition(transform,runwayData);
        }

        private void FillFenceAnswer(Transform transform,LevelData runwayData)
        {
            for (int i = 0; i < runwayData.answers.Count; i++)
            {
                if (runwayData.answers[i] == "null")//图片类型
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
                    textMeshComponent.text = runwayData.answers[i];
                }
            }
        }

        private void AdjustAnswerPosition(Transform transform,LevelData runwayData)
        {
            //判断题的T在位置0 F在位置1
            int correctWay = runwayData.way; //正确的答案 从左往右 从0开始 每次交换0号和目标位置的木板
            
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
        
        private string GetRawImageSourcePath(LevelData levelData,int i)
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

            string typeStr;
            switch (levelData.questionType)
            {
                case QuestionTypeEnum.TrueOrFalse:
                    typeStr = StaticString.TrueOrFalseQuestionImage;
                    break;
                case QuestionTypeEnum.TwoAnswerQuestion:
                    typeStr = StaticString.TwoAnswerQuestionImage;
                    break;
                case QuestionTypeEnum.ThreeAnswerQuestion:
                    typeStr = StaticString.ThreeAnswerQuestionImage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return StaticString.CsvDataPath + typeStr + $"/{levelData.id + suffix}.jpeg";
        }
        public bool IsAllQuestionHasCreated()
        {
            if (levelDataIndex<QuestionController.Instance.QuestionAmount)
            {
                return false;
            }

            if (hasEndLine == false)
            {
                CreateFinishLine();
                hasEndLine = true;
            }

            return true;
        }
        public void CreateFinishLine()
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("FinishLine", finishLineAddressablePath,runwaysGameObjectRoot.transform,
                (o =>
                {
                    o.transform.position = new Vector3(0, 0,
                        levelDataIndex * 60 * RunwayManager.RUNWAY_LENGTH_MAGNIFICATION + 60);
                    finishLine = o;
                }));
        }

        public void DestroyFinishLine()
        {
            if (finishLine!=null)
            {
                Object.Destroy(finishLine);
            }
        }
    }
}