using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    public enum TimeType
    {
        None,
        Timeout,
        Interval,
    }
    private List<TimeBase> timeList = new List<TimeBase>();
    private System.DateTime dateTime;
    public static Timer instance
    {
        get;
        private set;
    }

    public System.DateTime GetLocalTime(long timeStamp)
    {
        System.DateTime dtStart = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        System.TimeSpan toNow = new System.TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

    public long GetTimeStamp(System.DateTime dateTime)
    {
        var start = new System.DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
        return System.Convert.ToInt64((dateTime - start).TotalSeconds);
    }

    public long GetLocalTimeStamp(System.DateTime dateTime)
    {
        System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (long)(dateTime - startTime).TotalSeconds;

    }


    public void RemoveTimeout(System.Action<object[]> func)
    {
        instance.RemoveTimeBase(func, TimeType.Timeout);
    }

    public void RemoveTimeout(System.Action func)
    {
        instance.RemoveTimeBase(func, TimeType.Timeout);
    }

    public void RemoveAllTime()
    {
        instance.RemoveAllTimeBase(TimeType.None);
    }

    public void RemoveInterval(System.Action<int> func)
    {
        instance.RemoveTimeBase(func, TimeType.Interval);
    }

    public void RemoveInterval(System.Action func)
    {
        instance.RemoveTimeBase(func, TimeType.Interval);
    }

    public void RemoveAllInterval()
    {
        instance.RemoveAllTimeBase(TimeType.Interval);
    }

    public void RemoveAllTimeout()
    {
        instance.RemoveAllTimeBase(TimeType.Timeout);
    }

    public void SetTimeout(System.Action<object[]> func, int timeout, object[] args)
    {
        instance.SetTimeoutBegin(func, timeout, args);
    }

    public void SetTimeout(System.Action func, int timeout)
    {
        instance.SetTimeoutBegin(func, timeout);
    }

    public void SetInterval(System.Action<int> func, int timeout, int count = 0)
    {
        instance.SetIntervalBegin(func, timeout, count);
    }

    public void SetInterval(System.Action func, int timeout, int count = 0)
    {
        instance.SetIntervalBegin(func, timeout, count);
    }


    private void SetTimeoutBegin(object _func, int _timeout)
    {
        timeList.Add(new Timeout() { func = _func, timeout = _timeout, args = null });
    }

    private void SetTimeoutBegin(object _func, int _timeout, object[] _args)
    {
        timeList.Add(new Timeout() { func = _func, timeout = _timeout, args = _args });
    }

    private void SetIntervalBegin(object _func, int _timeout, int _count = 0)
    {
        timeList.Add(new Interval() { func = _func, timeout = _timeout, count = _count });
    }

    private void RemoveTimeBase(object func, TimeType type)
    {
        for (int i = 0, count = timeList.Count; i < count; i++)
        {
            if (timeList[i].type == type && timeList[i].func.Equals(func))
            {
                timeList[i].isRemove = true;
                return;
            }
        }
    }

    private void RemoveAllTimeBase(TimeType type)
    {
        for (int i = 0, count = timeList.Count; i < count; i++)
        {
            if (type == TimeType.None || timeList[i].type == type)
            {
                timeList[i].isRemove = true;
            }
        }
    }

    void Awake()
    {
        instance = this;
        dateTime = System.DateTime.Now;
    }

    void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        int t = (System.DateTime.Now - dateTime).Milliseconds;//如果程序挂起dateTime会保存挂起时的时间值
        dateTime = System.DateTime.Now;                       //等程序恢复后将时间差值传入
        TimeBase tb;
        for (int i = timeList.Count - 1; i >= 0; i--)
        {
            tb = timeList[i];
            if (tb.isRemove)
                timeList.RemoveAt(i);
            else
                tb.TimeCall(t);
        }
    }

    internal class TimeBase
    {
        public object func;
        public int timeout = -1;
        public bool isRemove = false;
        public TimeType type = TimeType.Timeout;
        protected int time;

        public TimeBase(TimeType t)
        {
            type = t;
        }

        internal void TimeCall(int t)
        {
            if (isRemove)
                return;
            time += t;
            if (time >= timeout)//中间有挂起的时间算出有多少次没有执行,添加进去。
            {
                Call(time / timeout);
            }
        }

        protected virtual void Call(int index) { }
    }

    internal class Interval : TimeBase
    {
        public int count = 0;
        private int index = 0;
        public Interval() : base(TimeType.Interval) { }
        protected override void Call(int i)
        {
            time -= timeout;
            index += i;
            if (func is System.Action)
            {
                (func as System.Action)();
            }
            else if (func is System.Action<int>)
            {
                (func as System.Action<int>)(index);
            }
            if (isRemove == false)
                isRemove = count > 0 && index >= count;
        }
    }

    internal class Timeout : TimeBase
    {
        public object[] args;
        public Timeout() : base(TimeType.Timeout) { }
        protected override void Call(int i)
        {
            isRemove = true;
            if (func is System.Action)
            {
                (func as System.Action)();
            }
            else if (func is System.Action<object>)
            {
                (func as System.Action<object>)(args);
            }
        }
    }
}






