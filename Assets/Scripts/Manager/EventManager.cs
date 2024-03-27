using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

    public struct ChildThreadTriggerData
    {
        public ClientEvent evtId;
        public object[] evtParams;
    }

    public class EventManager : Singleton<EventManager>
    {
        public static int ALL_EVENT_COUNT = 0;

        // 一帧最多处理 TRIGGER_NUM_PER_FRAME 事件派发
        private const int TRIGGER_NUM_PER_FRAME = 10;

        private readonly Dictionary<ClientEvent, Delegate> _eventDict;
        private readonly Dictionary<ClientEvent, Delegate> _eventFailDict;
        private readonly ConcurrentQueue<ChildThreadTriggerData> _triggerQueue;
        
        private readonly Dictionary<ClientEvent, Delegate> _onceEventDict;

        public EventManager()
        {
            _eventDict = new Dictionary<ClientEvent, Delegate>(150);
            _eventFailDict = new Dictionary<ClientEvent, Delegate>(150);
            _onceEventDict = new Dictionary<ClientEvent, Delegate>(10);
            _triggerQueue = new ConcurrentQueue<ChildThreadTriggerData>();
        }
        public void OnceEvent(ClientEvent evtId, Action call)
        {
            var oriCall = call;
            if (!_onceEventDict.TryGetValue(evtId, out var act))
                _onceEventDict.Add(evtId, oriCall);
            else if(act == (Delegate) oriCall)
                return;
            call += () =>
            {
                RemoveEvent(evtId, oriCall);
                _onceEventDict.Remove(evtId);
            };
            AddEvent(evtId, call);
        }
        
        public void AddEvent(ClientEvent evtId, Action call)
        {
            if (_eventDict.TryGetValue(evtId, out var fun))
            {
                if (fun is Action act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDict[evtId] = act;
                }
                else
                {
                    var str = $"try AddEvent funInfo:{fun.GetMethodInfo().Name} ，evtId :{evtId}";
                    throw new Exception(str);
                }
            }
            else
            {
                _eventDict.Add(evtId, call);
                ALL_EVENT_COUNT += 1;
            }
        }
        
        public void AddEvent<T>(ClientEvent evtId, Action<T> call)
        {
            if (_eventDict.TryGetValue(evtId, out var fun))
            {
                if (fun is Action<T> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDict[evtId] = act;
                }
                else
                {
                    var str = $"try AddEvent<T>  funInfo:{fun.GetMethodInfo().Name} ，evtId :{evtId}";
                    throw new Exception(str);
                }
            }
            else
            {
                _eventDict.Add(evtId, call);
                ALL_EVENT_COUNT += 1;
            }
        }


        public void AddEvent<T, U>(ClientEvent evtId, Action<T, U> call)
        {
            if (_eventDict.TryGetValue(evtId, out var fun))
            {
                if (fun is Action<T, U> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDict[evtId] = act;
                }
                else
                {
                    var str = $"try AddEvent<T,U>  funInfo:{fun.GetMethodInfo().Name} ，evtId :{evtId}";
                    throw new Exception(str);
                }
            }
            else
            {
                _eventDict.Add(evtId, call);
                ALL_EVENT_COUNT += 1;
            }
        }

        public void RemoveEvent(ClientEvent evtId, Delegate call)
        {
            if (call == null) return;
            if (!_eventDict.TryGetValue(evtId, out Delegate fun)) return;
            if (fun.GetType() == call.GetType())
            {
                var len1 = fun.GetInvocationList().Length;
                fun = Delegate.RemoveAll(fun, call);
                if (fun == null)
                {
                    _eventDict.Remove(evtId);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDict[evtId] = fun;
                    var len2 = fun.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
        }
        
        public void RemoveEvent(ClientEvent evtId, Action call)
        {
            if (call == null) return;
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            if (fun is Action act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDict.Remove(evtId);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDict[evtId] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
            else
            {
                throw new Exception("EventManager evtId is null");
            }
        }

        public void RemoveEvent<T>(ClientEvent evtId, Action<T> call)
        {
            if (call == null) return;
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            if (fun is Action<T> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDict.Remove(evtId);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDict[evtId] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
            else
            {
                throw new Exception("EventManager evtId is null");
            }
        }

        public void RemoveEvent<T, U>(ClientEvent evtId, Action<T, U> call)
        {
            if (call == null) return;

            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            if (fun is Action<T, U> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDict.Remove(evtId);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDict[evtId] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
            }
            else
            {
                throw new Exception("EventManager evtId is null");
            }
        }

        /// <summary>
        /// 删除同一个evtId绑定的所有事件
        /// 对于有在多个模块中使用同一evtId的删除绑定时务必谨慎调用！！！！
        /// </summary>
        /// <param name="evtId"></param>
        public void RemoveEvent(ClientEvent evtId)
        {
            // 先更新数量
            if (_eventDict.TryGetValue(evtId, out var fun))
            {
                ALL_EVENT_COUNT -= fun.GetInvocationList().Length;
                // 然后移除整个
                _eventDict.Remove(evtId);
            }
        }

        public void TriggerEvent(ClientEvent evtId)
        {
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; ++i)
            {
                if (!(list[i] is Action act)) continue;
                try
                {
                    act();
                }
                catch (Exception e)
                {
                    if (Debug.unityLogger.logEnabled)
                    {
                        Debug.LogErrorFormat("响应事件异常，事件id({0})\n{1}\n{2}", evtId, e.Message, e.StackTrace);
                    }

                    if (_eventFailDict.TryGetValue(evtId, out var failFun))
                    {
                        var failList = failFun.GetInvocationList();
                        for (var j = 0; j < failList.Length; j++)
                        {
                            if (!(failList[j] is Action failAct)) continue;
                            failAct?.Invoke();
                        }
                    }
                }
            }
        }

        public void TriggerEvent<T>(ClientEvent evtId, T v1)
        {
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; ++i)
            {
                if (!(list[i] is Action<T> act)) continue;
                try
                {
                    act(v1);
                }
                catch (Exception e)
                {
                    if (Debug.unityLogger.logEnabled)
                    {
                        Debug.LogErrorFormat("响应事件异常，事件id({0})\n{1}\n{2}", evtId, e.Message, e.StackTrace);
                    }
                }
            }
        }

        public void TriggerEvent<T, U>(ClientEvent evtId, T v1, U v2)
        {
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; ++i)
            {
                if (!(list[i] is Action<T, U> act)) continue;
                try
                {
                    act(v1, v2);
                }
                catch (Exception e)
                {
                    if (Debug.unityLogger.logEnabled)
                    {
                        Debug.LogErrorFormat("响应事件异常，事件id({0})\n{1}\n{2}", evtId, e.Message, e.StackTrace);
                    }
                }
            }
        }

        public void TriggerEvent<T, U, V>(ClientEvent evtId, T v1, U v2, V v3)
        {
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; ++i)
            {
                if (!(list[i] is Action<T, U, V> act)) continue;
                try
                {
                    act(v1, v2, v3);
                }
                catch (Exception e)
                {
                    if (Debug.unityLogger.logEnabled)
                    {
                        Debug.LogErrorFormat("响应事件异常，事件id({0})\n{1}\n{2}", evtId, e.Message, e.StackTrace);
                    }
                }
            }
        }

        public void TriggerEvent<T, U, V, W>(ClientEvent evtId, T v1, U v2, V v3, W V4)
        {
            if (!_eventDict.TryGetValue(evtId, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; ++i)
            {
                if (!(list[i] is Action<T, U, V, W> act)) continue;
                try
                {
                    act(v1, v2, v3, V4);
                }
                catch (Exception e)
                {
                    if (Debug.unityLogger.logEnabled)
                    {
                        Debug.LogErrorFormat("响应事件异常，事件id({0})\n{1}\n{2}", evtId, e.Message, e.StackTrace);
                    }
                }
            }
        }

        private void OnUpdate()
        {
            // if (_triggerQueue.Count <= 0)
            // {
            //     GameManager.Instance.RemoveUpdate(OnUpdate);
            //     return;
            // }
            //
            // var count = 0;
            // while (count < TRIGGER_NUM_PER_FRAME && _triggerQueue.Count > 0)
            // {
            //     count += 1;
            //     if (!_triggerQueue.TryDequeue(out var trigger)) continue;
            //     if (trigger.evtParams.Length >= 4)
            //     {
            //         TriggerEvent(trigger.evtId, trigger.evtParams[0], trigger.evtParams[1],
            //             trigger.evtParams[2], trigger.evtParams[3]);
            //     }
            //     else if (trigger.evtParams.Length == 3)
            //     {
            //         TriggerEvent(trigger.evtId, trigger.evtParams[0], trigger.evtParams[1], trigger.evtParams[2]);
            //     }
            //     else if (trigger.evtParams.Length == 2)
            //     {
            //         TriggerEvent(trigger.evtId, trigger.evtParams[0], trigger.evtParams[1]);
            //     }
            //     else if (trigger.evtParams.Length == 1)
            //     {
            //         TriggerEvent(trigger.evtId, trigger.evtParams[0]);
            //     }
            //     else
            //     {
            //         TriggerEvent(trigger.evtId);
            //     }
            // }
        }
    }
