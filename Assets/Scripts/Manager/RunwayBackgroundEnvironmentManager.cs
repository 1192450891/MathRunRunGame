using System.IO;
using Struct;
using UnityEngine;

namespace Manager
{
    public class RunwayBackgroundEnvironmentManager
    {
        private GameObject root;//根物体 生成的背景都在里面

        private Transform leftPos;
        private Transform rightPos;

        private int leftCount;
        private int rightCount;
        
        private float leftPosZOffest;
        private float rightPosZOffest;
        
        private static string LEFT_PATH="Assets/Prebs/Environment/BackgroundEnvironment/Left";
        private static string RIGHT_PATH="Assets/Prebs/Environment/BackgroundEnvironment/Right";

        private static int Generate_Lower_Distance_Limit_Z = 340;//Z轴生成距离下限  生成距离大于终点距离则不生成
        
        //控制闯关时跑道旁背景的生成
        public RunwayBackgroundEnvironmentManager(GameObject left,GameObject right)
        {
            root=new GameObject("RunwayBackgroundEnvironmentRoot");
            leftPos = left.transform;
            rightPos = right.transform;
            leftCount = GetBackgroundEnvironmentCount(LEFT_PATH);
            rightCount = GetBackgroundEnvironmentCount(RIGHT_PATH);
            leftPosZOffest = leftPos.position.z;
            rightPosZOffest = rightPos.position.z;
        }

        public void CreateNewRunwayBackgroundEnvironment()
        {
            float playerZPos = Player.Instance.transform.position.z;
            CreateLeft(GetRandomNum(leftCount),LEFT_PATH,new Vector3(leftPos.position.x,0,leftPosZOffest),playerZPos);
            CreateRight(GetRandomNum(rightCount),RIGHT_PATH,new Vector3(rightPos.position.x,0,rightPosZOffest),playerZPos);
        }

        private void CreateLeft(int i,string filePath,Vector3 pos,float playerZPos)
        {
            if(leftPosZOffest>GameStaticData.SumJourneyLength)return;
            LoadManager.Instance.LoadAndShowPrefabAsync("LeftBackgroundEnvironment",filePath+"/Left"+i.ToString()+".prefab",root.transform,
                    o =>
                    {
                        o.transform.position = pos;
                        o.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                        leftPosZOffest += CalculateObjZLength(o.transform);//加上此次生成的背景物体的z长度 作为新的偏移
                        if (leftPosZOffest-playerZPos < Generate_Lower_Distance_Limit_Z)//不断补齐到设定的下限距离
                        {
                            CreateLeft(GetRandomNum(leftCount),LEFT_PATH,new Vector3(leftPos.position.x,0,leftPosZOffest),playerZPos);
                        }
                    });
        }
        private void CreateRight(int i,string filePath,Vector3 pos,float playerZPos)
        {
            if(rightPosZOffest>GameStaticData.SumJourneyLength)return;
            LoadManager.Instance.LoadAndShowPrefabAsync("RightBackgroundEnvironment",filePath+"/Right"+i.ToString()+".prefab",root.transform,
                o =>
                {
                    o.transform.position = pos;
                    o.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    rightPosZOffest += CalculateObjZLength(o.transform);//加上此次生成的背景物体的z长度 作为新的偏移
                    if (rightPosZOffest-playerZPos < Generate_Lower_Distance_Limit_Z)
                    {
                        CreateRight(GetRandomNum(rightCount),RIGHT_PATH,new Vector3(rightPos.position.x,0,rightPosZOffest),playerZPos);
                    }
                });
        }

        private float CalculateObjZLength(Transform transform)
        {
            // 获取当前物体的边界框
            Bounds bounds=new Bounds(transform.position,Vector3.zero);
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // 计算边界框在Z轴上的长度
            float zLength = bounds.max.z - bounds.min.z;
            return zLength;
        }

        private int GetBackgroundEnvironmentCount(string path)//获取素材数量 /2代表减去对应的.meta文件的数量
        {
            string[] fileEntries = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            return fileEntries.Length/2;
        }
        private int GetRandomNum(int maxValue)
        {
            System.Random random = new System.Random();
            return random.Next(1, maxValue + 1);
        }
    }
}