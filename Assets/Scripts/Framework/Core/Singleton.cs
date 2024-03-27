using System;
using System.Threading;
using UnityEngine;

    //普通C#类的单例
    public class Singleton<T> where T :  new()
    {
        private static T singleton = default(T);
        private static readonly object _objectLock = new object();

        public static T Instance
        {
            get
            {
                if (singleton != null) return singleton;
                object obj;
                Monitor.Enter(obj = _objectLock); //加锁防止多线程创建单例
                try
                {
                    if (singleton == null)
                    {
                        singleton = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T)); //创建单例的实例
                    }
                }
                finally
                {
                    Monitor.Exit(obj);
                }

                return singleton;
            }
        }

        public static void DestroyInstance()
        {
            //singleton = null;
        }
    }

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        /**
          Returns the instance of this singleton. Do not add to GameObject in Editor
       */
        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = (T) FindObjectOfType(typeof(T));
                if (_instance != null) return _instance;
                var go = new GameObject("UpdateManager");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<T>();
                return _instance;
            }
            set => _instance = value;
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                // LogUtil.LogWarning("Repeat XComponentSingleton" + typeof(T), LogUtil.LogEnum.SYSTEM);
            }
        }
    }
