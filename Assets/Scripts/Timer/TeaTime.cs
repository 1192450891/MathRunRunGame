using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class TeaTime
    {
        internal readonly List<TeaTime> waiting = new List<TeaTime>();
        internal MonoBehaviour instance { get; } = null;

        private readonly List<TimeTask> _tasks = new List<TimeTask>();

        private int _curTaskIndex = 0;
        private int _executedCount = 0;
        private int _lastExecutedCount = 0;
        private Coroutine _curCoroutine = null;

        private bool _isPlaying = false;
        private bool _isPaused = false;
        private bool _isImmutable = false;
        private bool _isRepeating = false;
        private bool _isConsuming = false;
        private bool _isReversed = false;
        private bool _isYoyo = false;

        public TeaTime(MonoBehaviour instance)
        {
            this.instance = instance;
        }

        public YieldInstruction WaitForCompletion()
        {
            return instance.StartCoroutine(WaitForCompletion(this));
        }

        public bool IsComplete => _curTaskIndex >= _tasks.Count && !_isPlaying;


        private IEnumerator WaitForCompletion(TeaTime tt)
        {
            while (!tt.IsComplete) yield return null;
        }

        private TeaTime Add(float delay, Action call1, Action<TimeHandler> call2, Func<float> call3, bool isRealtime)
        {
            if (!_isImmutable)
            {
                var task = new TimeTask {time = delay, callBack1 = call1, callBack2 = call2, callBack3 = call3};
                _tasks.Add(task);
            }

            return _isPaused || _isPlaying ? this : Play(isRealtime);
        }

        public TeaTime Add(float delay, Action call1, bool isRealtime)
        {
            return Add(delay, call1, null, null, isRealtime);
        }

        public TeaTime Add(Action call1, Func<float> call3, bool isRealtime)
        {
            return Add(0, call1, null, call3, isRealtime);
        }

        public TeaTime Add(float delay, Action<TimeHandler> call2, bool isRealtime)
        {
            return Add(delay, null, call2, null, isRealtime);
        }

        public TeaTime Add(Action<TimeHandler> call2, Func<float> call3, bool isRealtime)
        {
            return Add(0, null, call2, call3, isRealtime);
        }

        public TeaTime Add(float delay, bool isRealtime)
        {
            return Add(delay, null, null, null, isRealtime);
        }

        public TeaTime Add(Func<float> call3, bool isRealtime)
        {
            return Add(0, null, null, call3, isRealtime);
        }

        public TeaTime Add(Action call1, bool isRealtime)
        {
            return Add(0, call1, null, null, isRealtime);
        }

        public TeaTime Add(Action<TimeHandler> call2, bool isRealtime)
        {
            return Add(0, null, call2, null, isRealtime);
        }

        /// <summary>
        /// duration < 0 无限循环
        /// </summary>
        /// <param name="duration"></param> 
        /// <param name="call2"></param>
        /// <param name="call3"></param>
        /// <returns></returns>
        private TeaTime Loop(float duration, Action<TimeHandler> call2, Func<float> call3, bool isRealtime)
        {
            if (!_isImmutable)
            {
                var task = new TimeTask {isLoop = true, time = duration, callBack2 = call2, callBack3 = call3};
                _tasks.Add(task);
            }

            return _isPaused || _isPlaying ? this : Play(isRealtime);
        }

        public TeaTime Loop(float duration, Action<TimeHandler> call2, bool isRealtime)
        {
            return Loop(duration, call2, null, isRealtime);
        }


        public TeaTime Loop(Action<TimeHandler> call2, Func<float> call3, bool isRealtime)
        {
            return Loop(0, call2, call3, isRealtime);
        }

        public TeaTime Loop(Action<TimeHandler> call2, bool isRealtime)
        {
            return Loop(-1, call2, null, isRealtime);
        }

        /// <summary>
        /// Enables Immutable mode, the queue will ignore new appends (.Add
        /// .Loop .If)
        /// </summary>
        public TeaTime Immutable()
        {
            _isImmutable = true;
            return this;
        }


        /// <summary>
        /// Enables Repeat mode, the queue will always be restarted on
        /// completion.
        /// </summary>
        public TeaTime Repeat()
        {
            _isRepeating = true;

            return this;
        }

        /// <summary>
        /// Enables Consume mode, the queue will remove each callback after
        /// execution.
        /// </summary>
        public TeaTime Consume()
        {
            _isConsuming = true;

            return this;
        }

        /// <summary>
        /// Reverses the callback execution order (From .Forward() to
        /// .Backward() mode and viceversa).
        /// </summary>
        public TeaTime Reverse()
        {
            _isReversed = !_isReversed;
            if (_isPlaying)
            {
                _curTaskIndex = _tasks.Count - _curTaskIndex;
            }

            return this;
        }

        /// <summary>
        /// Enables Backward mode, executing callbacks on reverse order (including Loops).
        /// </summary>
        public TeaTime Backward()
        {
            return !_isReversed ? Reverse() : this;
        }

        /// <summary>
        /// Enables Forward mode (the default), executing callbacks one after the other.
        /// </summary>
        public TeaTime Forward()
        {
            return _isReversed ? Reverse() : this;
        }

        /// <summary>
        /// Enables Yoyo mode, that will .Reverse() the callback execution order
        /// when the queue is completed. Only once per play without Repeat mode.
        /// </summary>
        public TeaTime Yoyo()
        {
            _isYoyo = true;
            return this;
        }

        /// <summary>
        /// Disables all modes (Immutable, Repeat, Consume, Backward, Yoyo).
        /// Just like new.
        /// </summary>
        public TeaTime Release()
        {
            _isImmutable = _isRepeating = _isConsuming = _isYoyo = false;
            return Forward();
        }

        /// <summary>
        /// Pauses the queue execution (use .Play() to resume).
        /// </summary>
        public TeaTime Pause()
        {
            _isPaused = true;

            return this;
        }

        /// <summary>
        /// Stops the queue execution (use .Play() to start over).
        /// </summary>
        public TeaTime Stop()
        {
            if (null != _curCoroutine)
            {
                instance.StopCoroutine(_curCoroutine);
            }

            _curTaskIndex = 0;
            _isPlaying = false;
            for (int i = 0, len = waiting.Count; i < len; i++)
            {
                waiting[i].Stop();
            }

            waiting.Clear();
            return this;
        }

        public TeaTime Play(bool isRealtime)
        {
            _isPaused = false;
            if (_isPlaying)
            {
                return this;
            }

            if (_tasks.Count < 0)
            {
                return this;
            }

            if (_curTaskIndex >= _tasks.Count)
            {
                _curTaskIndex = 0;
            }

            _curCoroutine = instance.StartCoroutine(ExecuteQueue(isRealtime));
            return this;
        }

        public TeaTime Wait(Func<bool> func, float checkDelay, bool isRealtime)
        {
            return Loop((call1) =>
                {
                    if (func())
                    {
                        call1.EndLoop();
                    }

                    call1.Wait(checkDelay);
                }, isRealtime
            );
        }

        /// <summary>
        /// This is the main algorithm. Executes all tasks, one after the
        /// other, calling their callbacks according to type, time and queue config.
        /// </summary>
        private IEnumerator ExecuteQueue(bool isRealtime)
        {
            _isPlaying = true;
            var reverseLastTask = -1;
            _lastExecutedCount = 0;

            while (_curTaskIndex < _tasks.Count)
            {
                var taskId = _curTaskIndex;
                var task = _tasks[taskId];
                _curTaskIndex++;
                yield return TeaTimeYield.endOfFrame;

                if (task.isLoop)
                {
                    var loopDuration = task.time;
                    if (task.callBack3 != null)
                    {
                        loopDuration += task.callBack3();
                    }

                    if (loopDuration == 0)
                    {
                        continue;
                    }

                    var loopHandler = new TimeHandler {self = this, isLoop = true};

                    var isInfinite = loopDuration < 0;
                    var rate = isInfinite ? 0 : 1 / loopDuration;
                    if (loopHandler.isReversed)
                    {
                        loopHandler.time = 1f;
                        rate = -rate;
                    }

                    while (loopHandler.isLoop &&
                           (loopHandler.isReversed ? loopHandler.time >= 0 : loopHandler.time <= 1))
                    {
                        if (_isReversed != loopHandler.isReversed)
                        {
                            rate = -rate;
                            loopHandler.isReversed = _isReversed;
                        }

                        var deltaTime = Time.deltaTime;
                        if (!isInfinite)
                        {
                            loopHandler.time += rate * deltaTime;
                        }

                        loopHandler.deltaTime = isInfinite
                            ? deltaTime
                            : 1 / (loopDuration - loopHandler.timeSinceStart) * deltaTime;
                        if (loopHandler.isReversed)
                        {
                            loopHandler.deltaTime = -loopHandler.deltaTime;
                        }

                        loopHandler.timeSinceStart += deltaTime;

                        while (_isPaused)
                        {
                            yield return null;
                        }

                        task.callBack2(loopHandler);

                        if (loopHandler.yieldInstructions != null)
                        {
                            for (int i = 0, len = loopHandler.yieldInstructions.Count; i < len; i++)
                            {
                                yield return loopHandler.yieldInstructions[i];
                            }

                            loopHandler.yieldInstructions.Clear();
                        }

                        if (loopHandler.yieldInstructions == null)
                        {
                            yield return null;
                        }
                    }

                    _executedCount += 1;
                    _lastExecutedCount += 1;
                }
                else
                {
                    var delayDuration = task.time;
                    if (task.callBack3 != null)
                    {
                        delayDuration += task.callBack3();
                    }

                    if (delayDuration > 0)
                    {
                        if (isRealtime)
                            yield return new WaitForSecondsRealtime(delayDuration);
                        else
                            yield return TeaTimeYield.Seconds(delayDuration);
                    }

                    while (_isPaused)
                    {
                        yield return null;
                    }

                    task.callBack1?.Invoke();

                    if (task.callBack2 != null)
                    {
                        var handler = new TimeHandler
                        {
                            self = this, time = 1, timeSinceStart = delayDuration, deltaTime = Time.deltaTime
                        };
                        task.callBack2(handler);

                        if (handler.yieldInstructions == null)
                        {
                            for (int i = 0, len = handler.yieldInstructions.Count; i < len; i++)
                            {
                                yield return handler.yieldInstructions[i];
                            }

                            handler.yieldInstructions.Clear();
                        }

                        if (delayDuration <= 0 && handler.yieldInstructions == null)
                        {
                            yield return null;
                        }
                    }
                    else if (delayDuration < 0)
                    {
                        yield return null;
                    }

                    _executedCount += 1;
                    _lastExecutedCount += 1;
                }

                if (_isConsuming)
                {
                    _curTaskIndex -= 1;
                    _tasks.Remove(task);
                    reverseLastTask = -1;
                }

                if (_isYoyo && _curTaskIndex >= _tasks.Count && (_lastExecutedCount <= _tasks.Count || _isRepeating))
                {
                    Reverse();
                    reverseLastTask = -1;
                }

                if (_isRepeating && _tasks.Count > 0 && _curTaskIndex >= _tasks.Count)
                {
                    _curTaskIndex = 0;
                    reverseLastTask = -1;
                }

                if (_tasks.Count > 0 && _curTaskIndex >= _tasks.Count)
                {
                    waiting.Clear();
                }
            }

            _isPlaying = false;
            yield return null;
        }
    }

    internal class TimeTask
    {
        public bool isLoop = false;
        public float time = 0;

        public Action callBack1;
        public Action<TimeHandler> callBack2;
        public Func<float> callBack3;
    }

    public class TimeHandler
    {
        public TeaTime self;

        public float time = 0;
        public float deltaTime = 0;
        public float timeSinceStart = 0;

        internal bool isLoop = false;
        internal bool isReversed = false;
        internal List<YieldInstruction> yieldInstructions = null;

        public void EndLoop()
        {
            isLoop = false;
        }

        /// <summary>
        /// Appends a YieldInstruction to wait after the current callback execution.
        /// </summary>
        /// <param name="yi"></param>
        private void Wait(YieldInstruction yi)
        {
            if (yieldInstructions == null)
            {
                yieldInstructions = new List<YieldInstruction>();
            }

            yieldInstructions.Add(yi);
        }

        /// <summary>
        /// Appends a TeaTime to wait after the current callback execution that
        /// is also affected by the queue .Stop() and .Reset().
        /// </summary>
        /// <param name="t"></param>
        public void Wait(TeaTime t)
        {
            if (self.waiting.Contains(t)) return;
            self.waiting.Add(t);
            Wait(t.WaitForCompletion());
        }

        /// <summary>
        /// Appends a time delay to wait after the current callback execution.
        /// </summary>
        /// <param name="time"></param>
        public void Wait(float time)
        {
            if (time < 0)
            {
                return;
            }

            Wait(new WaitForSeconds(time));
        }


        /// <summary>
        /// Appends a boolean condition to wait until true after the current callback execution.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="checkDelay"></param>
        /// <param name="isRealtime"></param>
        public void Wait(Func<bool> condition, float checkDelay, bool isRealtime)
        {
            Wait(self.instance.Create().Wait(condition, checkDelay, isRealtime));
        }
    }

    public static class TeaTimeExtension
    {
        private static Dictionary<MonoBehaviour, Dictionary<string, TeaTime>> _timeDict;

        public static TeaTime Create(this MonoBehaviour instance)
        {
            return new TeaTime(instance);
        }

        public static TeaTime Create(this MonoBehaviour instance, string queueName)
        {
            if (_timeDict == null)
            {
                _timeDict = new Dictionary<MonoBehaviour, Dictionary<string, TeaTime>>();
            }

            if (!_timeDict.ContainsKey(instance))
            {
                _timeDict[instance] = new Dictionary<string, TeaTime>();
            }

            var dict = _timeDict[instance];
            if (!dict.ContainsKey(queueName))
            {
                dict[queueName] = new TeaTime(instance);
            }

            return dict[queueName];
        }
    }

    public static class TeaTimeYield
    {
        private static readonly Dictionary<float, WaitForSeconds> _sceondsDict =
            new Dictionary<float, WaitForSeconds>(100, new TeaFloatComparer());

        private static readonly WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();

        public static WaitForEndOfFrame endOfFrame => _endOfFrame;

        private static readonly WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();

        public static WaitForFixedUpdate fixedUpdate => _fixedUpdate;


        public static WaitForSeconds Seconds(float seconds)
        {
            WaitForSeconds sec = null;
            if (!_sceondsDict.TryGetValue(seconds, out sec))
            {
                _sceondsDict.Add(seconds, sec = new WaitForSeconds(seconds));
            }

            return sec;
        }

        private class TeaFloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y)
            {
                return x == y;
            }

            int IEqualityComparer<float>.GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }
    }
