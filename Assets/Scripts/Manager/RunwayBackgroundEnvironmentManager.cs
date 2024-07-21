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

        private float leftPosZOffest;
        private float rightPosZOffest;

        public static List<GameObject> Left_ObjList = new List<GameObject>();
        public static List<GameObject> Right_ObjList = new List<GameObject>();

        private static float GenerateLowerDistanceLimitZ;//Z轴生成距离下限  生成距离大于终点距离则不生成
        
        //控制闯关时跑道旁背景的生成
        public RunwayBackgroundEnvironmentManager(GameObject left,GameObject right)
        {
            root=new GameObject("RunwayBackgroundEnvironmentRoot");
            boundLengthDic = new Dictionary<string, float>();
            leftPos = left.transform;
            rightPos = right.transform;
            GenerateLowerDistanceLimitZ = 340 * RunwayManager.RUNWAY_LENGTH_MAGNIFICATION;
            leftPosZOffest = leftPos.position.z;
            rightPosZOffest = rightPos.position.z;
            backgroundQueue = new Queue<GameObject>();
            CreateNewRunwayBackgroundEnvironment();
        }

        public void CreateNewRunwayBackgroundEnvironment()
        {
            float playerZPos = Player.Instance.transform.position.z;
            CreateLeft(new Vector3(leftPos.position.x,0,leftPosZOffest),playerZPos);
            CreateRight(new Vector3(rightPos.position.x,0,rightPosZOffest),playerZPos);
        }

        private void CreateLeft(Vector3 pos,float playerZPos)
        {
            if(leftPosZOffest>GameStaticData.SumJourneyLength)return;
            int index = Util.Instance.GetRandomNum(Left_ObjList.Count)-1;
            var newObjOriginal = Left_ObjList[index];
            float selfZOffset=GetObjZLength(newObjOriginal.transform,"Left"+index);//自身带来的偏移长度
            var newObjPos= new Vector3(pos.x,0,pos.z+selfZOffset/2);
            var newObj=Object.Instantiate(newObjOriginal,newObjPos,Quaternion.identity,root.transform);
            backgroundQueue.Enqueue(newObj);
            leftPosZOffest += selfZOffset;//加上此次生成的背景物体的z长度 作为新的偏移
            leftPosZOffest -= 2;
            if (leftPosZOffest-playerZPos < GenerateLowerDistanceLimitZ)//不断补齐到设定的下限距离
            {
                CreateLeft(new Vector3(leftPos.position.x,0,leftPosZOffest),playerZPos);
            }
        }
        private void CreateRight(Vector3 pos,float playerZPos)
        {
            if(rightPosZOffest>GameStaticData.SumJourneyLength)return;
            int index = Util.Instance.GetRandomNum(Right_ObjList.Count)-1;
            var newObjOriginal = Right_ObjList[index];
            float selfZOffset=GetObjZLength(newObjOriginal.transform,"Right"+index);//自身带来的偏移长度
            var newObjPos= new Vector3(pos.x,0,pos.z+selfZOffset/2);
            var newObj=Object.Instantiate(newObjOriginal,newObjPos,Quaternion.identity,root.transform);
            backgroundQueue.Enqueue(newObj);
            rightPosZOffest += selfZOffset;//加上此次生成的背景物体的z长度 作为新的偏移
            rightPosZOffest -= 2;
            if (rightPosZOffest-playerZPos < GenerateLowerDistanceLimitZ)//不断补齐到设定的下限距离
            {
                CreateRight(new Vector3(rightPos.position.x,0,rightPosZOffest),playerZPos);
            }
        }

        private float GetObjZLength(Transform transform,string objName)
        {
            if (boundLengthDic.ContainsKey(objName)) return boundLengthDic[objName];
            float zLength = Util.Instance.CalculateObjZLength(transform);
            boundLengthDic[objName] = zLength;
            return zLength;
        }

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