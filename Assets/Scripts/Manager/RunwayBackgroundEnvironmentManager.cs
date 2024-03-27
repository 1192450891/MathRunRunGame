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

        private int index;//背景计数下标
        
        private float leftPosZOffest;
        private float rightPosZOffest;
        
        //控制闯关时跑道旁背景的生成
        public RunwayBackgroundEnvironmentManager(GameObject left,GameObject right)
        {
            root=new GameObject("RunwayBackgroundEnvironmentRoot");
            leftPos = left.transform;
            rightPos = right.transform;
            leftPosZOffest = leftPos.position.z;
            rightPosZOffest = rightPos.position.z;
            index = 0;
        }

        public void CreateNewRunwayBackgroundEnvironment()
        {
            if(GameStaticData.SumJourneyLength<leftPosZOffest||GameStaticData.SumJourneyLength<rightPosZOffest)return;
            
            string leftPath = "Assets/Prebs/Environment/BackgroundEnvironment/Left";
            string rightPath = "Assets/Prebs/Environment/BackgroundEnvironment/Right";
            int leftCount = GetBackgroundEnvironmentCount(leftPath);
            int rightCount = GetBackgroundEnvironmentCount(rightPath);

            var leftPosition = leftPos.position;
            var rightPosition = rightPos.position;
            
            CreateLeft(GetRandomNum(leftCount),leftPath,new Vector3(leftPosition.x,0,leftPosZOffest));
            CreateRight(GetRandomNum(rightCount),rightPath,new Vector3(rightPosition.x,0,rightPosZOffest));
            index++;
        }

        private void CreateLeft(int i,string filePath,Vector3 pos)
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("LeftBackgroundEnvironment",filePath+"/Left"+i.ToString()+".prefab",root.transform,
                    o =>
                    {
                        o.transform.position = pos;
                        leftPosZOffest += CalculateObjZLength(o.transform);//加上此次生成的背景物体的z长度 作为新的偏移
                    });
        }
        private void CreateRight(int i,string filePath,Vector3 pos)
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("RightBackgroundEnvironment",filePath+"/Right"+i.ToString()+".prefab",root.transform,
                o =>
                {
                    o.transform.position = pos;
                    rightPosZOffest += CalculateObjZLength(o.transform);//加上此次生成的背景物体的z长度 作为新的偏移
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