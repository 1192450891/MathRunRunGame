using System.Collections.Generic;
using System.IO;
using Framework.Core;
using GameBase.Player;
using Struct;
using UnityEngine;

namespace Manager
{
    public class RunwayBackgroundEnvironmentManager
    {
        private GameObject root;//根物体 生成的背景都在里面
        private Queue<GameObject> backgroundQueue;

        private Dictionary<string, float> boundLengthDic;//存储计算过的Z轴长度 避免重复运算

        private Transform leftPos;
        private Transform rightPos;

        private int leftCount;
        private int rightCount;
        
        private float leftPosZOffest;
        private float rightPosZOffest;
        
        private static string LEFT_PATH="Assets/Prebs/Environment/BackgroundEnvironment/Left";
        private static string RIGHT_PATH="Assets/Prebs/Environment/BackgroundEnvironment/Right";

        private static float Generate_Lower_Distance_Limit_Z = 340*RunwayManager.RUNWAY_LENGTH_MAGNIFICATION;//Z轴生成距离下限  生成距离大于终点距离则不生成
        
        //控制闯关时跑道旁背景的生成
        public RunwayBackgroundEnvironmentManager(GameObject left,GameObject right)
        {
            root=new GameObject("RunwayBackgroundEnvironmentRoot");
            boundLengthDic = new Dictionary<string, float>();
            leftPos = left.transform;
            rightPos = right.transform;
            leftCount = Util.Instance.GetFilesCount(LEFT_PATH);
            rightCount = Util.Instance.GetFilesCount(RIGHT_PATH);
            leftPosZOffest = leftPos.position.z;
            rightPosZOffest = rightPos.position.z;
            backgroundQueue = new Queue<GameObject>();
            CreateNewRunwayBackgroundEnvironment();
        }

        public void CreateNewRunwayBackgroundEnvironment()
        {
            float playerZPos = Player.Instance.transform.position.z;
            CreateLeft(Util.Instance.GetRandomNum(leftCount),LEFT_PATH,new Vector3(leftPos.position.x,0,leftPosZOffest),playerZPos);
            CreateRight(Util.Instance.GetRandomNum(rightCount),RIGHT_PATH,new Vector3(rightPos.position.x,0,rightPosZOffest),playerZPos);
        }

        private void CreateLeft(int i,string filePath,Vector3 pos,float playerZPos)
        {
            if(leftPosZOffest>GameStaticData.SumJourneyLength)return;
            LoadManager.Instance.LoadAndShowPrefabAsync("LeftBackgroundEnvironment",filePath+"/Left"+i.ToString()+".prefab",root.transform,
                    o =>
                    {
                        float selfZOffest=GetObjZLength(o.transform,"Left"+i.ToString());//自身带来的偏移长度
                        o.transform.position = new Vector3(pos.x,0,pos.z+selfZOffest/2);//找到生成中心点
                        leftPosZOffest += selfZOffest;//加上此次生成的背景物体的z长度 作为新的偏移
                        leftPosZOffest -= 2;
                        if (leftPosZOffest-playerZPos < Generate_Lower_Distance_Limit_Z)//不断补齐到设定的下限距离
                        {
                            CreateLeft(Util.Instance.GetRandomNum(leftCount),LEFT_PATH,new Vector3(leftPos.position.x,0,leftPosZOffest),playerZPos);
                        }
                        backgroundQueue.Enqueue(o);
                    });
        }
        private void CreateRight(int i,string filePath,Vector3 pos,float playerZPos)
        {
            if(rightPosZOffest>GameStaticData.SumJourneyLength)return;
            LoadManager.Instance.LoadAndShowPrefabAsync("RightBackgroundEnvironment",filePath+"/Right"+i.ToString()+".prefab",root.transform,
                o =>
                {
                    float selfZOffest=GetObjZLength(o.transform,"Right"+i.ToString());//自身带来的偏移长度
                    o.transform.position = new Vector3(pos.x,0,pos.z+selfZOffest/2);
                    rightPosZOffest += selfZOffest;//加上此次生成的背景物体的z长度 作为新的偏移
                    rightPosZOffest -= 2;
                    if (rightPosZOffest-playerZPos < Generate_Lower_Distance_Limit_Z)
                    {
                        CreateRight(Util.Instance.GetRandomNum(rightCount),RIGHT_PATH,new Vector3(rightPos.position.x,0,rightPosZOffest),playerZPos);
                    }
                    backgroundQueue.Enqueue(o);
                });
        }

        private float GetObjZLength(Transform transform,string objName)
        {
            if (boundLengthDic.ContainsKey(objName)) return boundLengthDic[objName];
            float zLength = Util.Instance.CalculateObjZLength(transform);
            boundLengthDic[objName] = zLength;
            return zLength;
        }

        // private int GetBackgroundEnvironmentCount(string path)//获取素材数量 /2代表减去对应的.meta文件的数量
        // {
        //     string[] fileEntries = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        //     return fileEntries.Length/2;
        // }
        // private int GetRandomNum(int maxValue)
        // {
        //     System.Random random = new System.Random();
        //     return random.Next(1, maxValue + 1);
        // }

        public void ReStart()
        {
            leftPosZOffest = leftPos.position.z;
            rightPosZOffest = rightPos.position.z;
            while (backgroundQueue.Count!=0)
            {
                Object.Destroy(backgroundQueue.Dequeue());
            }
            CreateNewRunwayBackgroundEnvironment();
        }
    }
}