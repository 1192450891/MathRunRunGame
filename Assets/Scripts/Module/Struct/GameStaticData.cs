using System;
using System.Collections.Generic;
using Manager;

namespace Struct
{
    public struct GameStaticData
    {
        public static bool GameHasStart;
        
        public static bool GameHasEnd;

        public static bool isFreeMoving;
        
        public static bool PlayerIsPlaying()//检测玩家是否正在跑道上闯关
        {
            return GameHasStart&&!GameHasEnd;
        }
        
        private static float sumJourneyLength;
        public static float SumJourneyLength
        {
            get
            {
                if (sumJourneyLength == 0)
                {
                    sumJourneyLength=QuestionController.Instance.QuestionAmount * 60 *RunwayManager.RUNWAY_LENGTH_MAGNIFICATION+30;//最后加上终点的偏移
                }
                return sumJourneyLength;
            }
        }
        
        public static bool CanOperate;//玩家当前是否能进行操作
        
        public static float InitSpeedNum=40;//玩家开局初始速度
        public static float CurSpeedNum;//玩家当前速度
        
        public static float MinWalkSpeed=40;//速度下限
        public static float MaxWalkSpeed=100;//速度上限

        private static float historyMaxWalkSpeed;
        public static float HistoryMaxWalkSpeed{
            get => historyMaxWalkSpeed;
            set => historyMaxWalkSpeed = Math.Max(value, historyMaxWalkSpeed);
        }//本局游戏达到的最高速度
        
        public static int HasPassedNum;//玩家本局已通过关卡计数
        public static int HasCorrectNum;//玩家本局已答对关卡计数

        private const int listCapacity = 20;
        public static List<string> CorrectQuestionIdList=new(listCapacity);//本局答对的题目ID列表
        public static List<string> WrongQuestionIdList=new(listCapacity);//本局答错的题目ID列表
        
        public static void ReStart()
        {
            GameHasStart = false;
            GameHasEnd = false;
            sumJourneyLength = 0;
            CanOperate = false;
            InitSpeedNum = 40;
            MinWalkSpeed = 40;
            MaxWalkSpeed = 100;
            historyMaxWalkSpeed = 0;
            HasPassedNum = 0;
            HasCorrectNum = 0;
            CorrectQuestionIdList = new List<string>(listCapacity);
            WrongQuestionIdList = new List<string>(listCapacity);
        }
    }
}