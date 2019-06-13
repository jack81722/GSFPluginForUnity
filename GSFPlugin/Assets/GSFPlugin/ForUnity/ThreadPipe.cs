using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Helper of calling monebehaviour method in other thread
/// </summary>
public class ThreadPipe : MonoBehaviour
{
    private static Thread main = Thread.CurrentThread;
    private static ThreadPipe _instance;
    public static ThreadPipe instance
    {
        get
        {
            if(_instance == null)
            {
                if((_instance = FindObjectOfType<ThreadPipe>()) == null)
                {
                    GameObject go = new GameObject();
                    go.name = "_ThreadPipe";
                    _instance = go.AddComponent<ThreadPipe>();
                }
            }
            return _instance;
        }
    }

    public enum CallTime
    {
        FixedUpdate,
        Update,
        LateUpdate
    }

    private class ActionProxy
    {
        Delegate action;
        object[] args;
        Action<object> callback;

        public ActionProxy(Delegate action, object[] args)
        {
            if (action.Method.GetParameters().Length != args.Length)
                throw new InvalidOperationException("Argument count is not matched method.");
            this.action = action;
            this.args = args;
            this.callback = null;
        }

        public ActionProxy(Delegate action, object[] args, Action<object> callback)
        {
            if (action.Method.GetParameters().Length != args.Length)
                throw new InvalidOperationException("Argument count is not matched method.");
            this.action = action;
            this.args = args;
            this.callback = callback;
        }

        public void Execute()
        {
            object result = action.DynamicInvoke(args);
            if(callback != null)
                callback.Invoke(result);
        }

        public override string ToString()
        {
            System.Reflection.MethodInfo methodInfo = action.Method;
            System.Reflection.ParameterInfo[] paramInfos = methodInfo.GetParameters();
            string str = methodInfo.Name + "(";
            for(int i = 0; i < paramInfos.Length - 1; i++)
            {
                str += paramInfos[i].ParameterType.MemberType.ToString() + ", ";
            }
            if (args.Length > 0)
                str += args[args.Length - 1] + ")";
            return str;
        }
    }

    private List<ActionProxy> fixedActs;
    private List<ActionProxy> updateActs;
    private List<ActionProxy> lateActs;

    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        fixedActs = new List<ActionProxy>();
        updateActs = new List<ActionProxy>();
        lateActs = new List<ActionProxy>();
    }

    private void FixedUpdate()
    {
        executeAction(fixedActs);
    }

    private void Update()
    {
        executeAction(updateActs);
    }

    private void LateUpdate()
    {
        executeAction(lateActs);
    }

    private static bool inMainThread()
    {
        return Thread.CurrentThread == main;
    }

    private void executeAction(List<ActionProxy> proxies)
    {   
        for (int i = 0; i < proxies.Count; i++)
        {
            try
            {
                proxies[i].Execute();
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to execute {0} : {1}, {2}", proxies[i].ToString(), e.Message, e.StackTrace));
            }
        }
        proxies.Clear();
    }

    public static void Call<T, TResult>(Func<T, TResult> func, T t, Action<TResult> callback, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            var result = func.Invoke(t);
            if (callback != null)
                callback.Invoke(result);
        }
        else
        {
            Action<object> objCallback = null;
            if (callback != null)
                objCallback = (obj) => callback((TResult)obj);
            Save(func, objCallback, callTime, t);
        }
    }

    public static void Call<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 t1, T2 t2, Action<TResult> callback, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            var result = func.Invoke(t1, t2);
            if (callback != null)
                callback.Invoke(result);
        }
        else
        {
            Action<object> objCallback = null;
            if (callback != null)
                objCallback = (obj) => callback((TResult)obj);
            Save(func, objCallback, callTime, t1, t2);
        }
    }

    public static void Call<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 t1, T2 t2, T3 t3, Action<TResult> callback, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            var result = func.Invoke(t1, t2, t3);
            if (callback != null)
                callback.Invoke(result);
        }
        else
        {
            Action<object> objCallback = null;
            if (callback != null)
                objCallback = (obj) => callback((TResult)obj);
            Save(func, objCallback, callTime, t1, t2, t3);
        }
    }

    public static void Call<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, Action<TResult> callback, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            var result = func.Invoke(t1, t2, t3, t4);
            if (callback != null)
                callback.Invoke(result);
        }
        else
        {
            Action<object> objCallback = null;
            if (callback != null)
                objCallback = (obj) => callback((TResult)obj);
            Save(func, objCallback, callTime, t1, t2, t3, t4);
        }
    }

    public static void Call<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, Action<TResult> callback, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            var result = func.Invoke(t1, t2, t3, t4, t5);
            if (callback != null)
                callback.Invoke(result);
        }
        else
        {
            Action<object> objCallback = null;
            if (callback != null)
                objCallback = (obj) => callback((TResult)obj);
            Save(func, objCallback, callTime, t1, t2, t3, t4, t5);
        }
    }

    public static void Call(Action action, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            action.Invoke();
        }
        else
            Save(action, null, callTime, null);
    }

    public static void Call<T>(Action<T> action, T t, CallTime callTime = CallTime.Update)
    {
        if (inMainThread())
        {
            action.Invoke(t);
        }
        else
            Save(action, null, callTime, t);
    }

    private static void Save(Delegate action, Action<object> callback, CallTime callTime, params object[] args)
    {
        switch (callTime)
        {
            case CallTime.FixedUpdate:
                instance.fixedActs.Add(new ActionProxy(action, args, callback));
                break;
            case CallTime.LateUpdate:
                instance.lateActs.Add(new ActionProxy(action, args, callback));
                break;
            default:
                instance.updateActs.Add(new ActionProxy(action, args, callback));
                break;
        }

    }
}
