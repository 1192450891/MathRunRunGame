using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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

        public enum WidthMode
        {
            MaxWidth,
            MinWidth,
            CertainWidth
        }
        public void SetImageWithWidth(RawImage rawImage,string imagePath,int width,WidthMode mode)
        {
            if(rawImage==null)return;
            var texture=LoadPNG(imagePath);
            rawImage.texture = texture;

            // 获取图片的原始尺寸
            int originalWidth = rawImage.texture.width;
            int originalHeight = rawImage.texture.height;

            float ratio;//宽高比
            int newHeight=0;
            switch (mode)
            {
                case WidthMode.MaxWidth:
                    // 判断是否超过设定宽度
                    if (originalWidth > width)
                    {
                        // 计算新的高度以保持宽高比
                        ratio = (float)originalHeight / originalWidth;
                        newHeight = Mathf.RoundToInt(width * ratio);
                    }
                    else
                    {
                        newHeight = originalHeight;
                    }
                    break;
                case WidthMode.MinWidth:
                    // 判断是否小于设定宽度
                    if (originalWidth < width)
                    {
                        ratio = (float)originalHeight / originalWidth;
                        newHeight = Mathf.RoundToInt(width * ratio);
                    }
                    else
                    {
                        newHeight = originalHeight;
                    }
                    break;
                case WidthMode.CertainWidth:
                    newHeight = originalHeight;
                    break;
            }
            rawImage.rectTransform.sizeDelta = new Vector2(width, newHeight);


        }
    }
}