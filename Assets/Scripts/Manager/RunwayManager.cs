using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Wx;
using Object = UnityEngine.Object;

namespace Manager
{
    public class RunwayManager : Singleton<RunwayManager>
    {
        [Tooltip("存储跑道")] public Queue<GameObject> runWays; //入队列时创建对应的栅栏 需要销毁时按出列顺序销毁

        public int LevelDataIndex; //用于创建新跑道时的指针下标

        private int gameStartInitRunwaysCount = 4; //初始化游戏时创建的跑道数量

        private GameObject runwaysGameObjectRoot; //跑道根节点

        public List<string> questionKeyData;//题解内容存储

        public static float RUNWAY_LENGTH_MAGNIFICATION;//跑道长度倍率 （用于调整回答问题时间的大小）
        
        public GameObject FinishLine;//终点线preb实例

        private static string TwoAnswerFenceAssetablePath ="Assets/Prebs/Environment/Fence/2AnswerFence.prefab" ;
        private static string ThreeAnswerFenceAssetablePath = "Assets/Prebs/Environment/Fence/3AnswerFence.prefab";

        public void InitRunways()
        {
            if (runWays != null)//点击了重新开始游戏 需要销毁前一局的跑道
            {
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
            questionKeyData = new List<string>();
            LevelDataIndex = 0;
            for (int i = 0; i < gameStartInitRunwaysCount; i++)
            {
                CreateNewRunway();
            }
        }

        public void CreateNewRunway()
        {
            string objPath = ""; //这次的栅栏类型
            int index = LevelDataIndex;
            LevelDataIndex++;
            var curQuestionLevelData = QuestionController.Instance.levelData[index];
            var curQuestionType = curQuestionLevelData.questionType;
            if (curQuestionType == QuestionTypeEnum.TrueOrFalse)
            {
                objPath = TwoAnswerFenceAssetablePath;
            }
            else if (curQuestionType == QuestionTypeEnum.TwoAnswerQuestion)
            {
                objPath = TwoAnswerFenceAssetablePath;
            }
            else if (curQuestionType == QuestionTypeEnum.ThreeAnswerQuestion)
            {
                objPath = ThreeAnswerFenceAssetablePath;
            }

            LoadManager.Instance.LoadAndShowPrefabAsync("Runways", "Assets/Prebs/Environment/Runway.prefab",
                runwaysGameObjectRoot.transform,
                obj =>
                {
                    RUNWAY_LENGTH_MAGNIFICATION = obj.transform.localScale.z;
                    obj.transform.position = new Vector3(0, 0, (2*index + 1) * 30*RUNWAY_LENGTH_MAGNIFICATION+30);
                    Transform FencePos = obj.transform.Find("FencePos");
                    LoadManager.Instance.LoadAndShowPrefabAsync("Fence", objPath, FencePos,
                        fence =>
                        {
                            fence.transform.position = FencePos.position;
                            InitFence(fence.transform, curQuestionLevelData);
                        });
                    runWays.Enqueue(obj);
                });
            questionKeyData.Add(curQuestionLevelData.questionKey);
        }

        private void InitFence(Transform transform, QuestionController.LevelData runwayData)
        {
            int correctWay = runwayData.way; //正确的答案 从左往右 从0开始 每次交换0号和目标位置的木板
            Transform zeroText = TransformUtil.Find(transform,"0 Text");
            Transform targetText = TransformUtil.Find(transform,"{correctWay} Text");
            if (runwayData.questionType != QuestionTypeEnum.TrueOrFalse) //判断题的T在位置0 F在位置1
            {
                if (correctWay != 0)
                {
                    var transform1 = targetText.transform;
                    var transform2 = zeroText.transform;
                    (transform1.position, transform2.position) =
                        (transform2.position, transform1.position);
                }
            }

            for (int i = 0; i < runwayData.answers.Count; i++)
            {
                
                Transform curTransform = TransformUtil.Find(transform,$"{i} Text");
                var curTextMesh = curTransform.GetComponent<TMP_Text>();
                curTextMesh.text = runwayData.answers[i];
            }
        }

        public void CreateFinishLine()
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("FinishLine", "Assets/Prebs/Environment/Finish Line.prefab",runwaysGameObjectRoot.transform,
                (o =>
                {
                    o.transform.position = new Vector3(0, 0,
                        questionKeyData.Count * 60 * RunwayManager.RUNWAY_LENGTH_MAGNIFICATION + 60);
                    FinishLine = o;
                }));
        }

        public void DestoryFinishLine()
        {
            if (FinishLine!=null)
            {
                Object.Destroy(FinishLine);
            }
        }
    }
}