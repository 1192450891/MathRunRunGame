using System.IO;
using UnityEngine;

namespace Manager
{
    public class RunwayBackgroundEnvironmentManager
    {
        private GameObject root;//根物体 生成的背景都在里面
        private Transform leftPos;
        private Transform rightPos;

        private int index;//背景计数下标
        
        //控制闯关时跑道旁背景的生成
        public RunwayBackgroundEnvironmentManager(GameObject left,GameObject right)
        {
            root=new GameObject("RunwayBackgroundEnvironmentRoot");
            leftPos = left.transform;
            rightPos = right.transform;
            index = 0;
        }
        //控制闯关时跑道旁背景的生成
        // public void Init(GameObject left,GameObject right)
        // {
        //     root=new GameObject("RunwayBackgroundEnvironmentRoot");
        //     leftPos = left.transform;
        //     rightPos = right.transform;
        //     index = 0;
        // }
        
        public void CreateNewRunwayBackgroundEnvironment()
        {
            string leftPath = "Assets/Prebs/Environment/BackgroundEnvironment/Left";
            string rightPath = "Assets/Prebs/Environment/BackgroundEnvironment/Right";
            int leftCount = GetBackgroundEnvironmentCount(leftPath);
            int rightCount = GetBackgroundEnvironmentCount(rightPath);
            var leftPosition = leftPos.position;
            var rightPosition = rightPos.position;
            CreateLeft(GetRandomNum(leftCount),leftPath,new Vector3(leftPosition.x,0,leftPosition.z+index*60));
            CreateRight(GetRandomNum(rightCount),rightPath,new Vector3(rightPosition.x,0,rightPosition.z+index*60));
            index++;
        }

        private void CreateLeft(int i,string filePath,Vector3 pos)
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("LeftBackgroundEnvironment",filePath+"/Left"+i.ToString()+".prefab",root.transform,
                    o =>
                    {
                        o.transform.position = pos;
                    });
        }
        private void CreateRight(int i,string filePath,Vector3 pos)
        {
            LoadManager.Instance.LoadAndShowPrefabAsync("RightBackgroundEnvironment",filePath+"/Right"+i.ToString()+".prefab",root.transform,
                o =>
                {
                    o.transform.position = pos;
                });
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