using System.IO;
using UnityEngine;

namespace Framework.Core
{
    public class Util:Singleton<Util>
    {
        public void SwapParent(Transform transform1,Transform transform2)
        {
            var transform1Parent = transform1.parent;
            transform1.SetParent(transform2.parent);
            transform2.SetParent(transform1Parent);
        }
        public void Swap(ref Object obj1,ref Object obj2)
        {
            (obj1, obj2) = (obj2, obj1);
        }
        public float CalculateObjZLength(Transform transform)
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
        public Texture2D LoadPNG(string path)
        {
            Texture2D texture = null;
            byte[] fileData;
 
            if (File.Exists(path))
            {
                fileData = File.ReadAllBytes(path);
                texture = new Texture2D(2, 2); // 创建Texture2D实例
                texture.LoadImage(fileData); // 加载图片数据
            }
 
            return texture;
        }
    }
}