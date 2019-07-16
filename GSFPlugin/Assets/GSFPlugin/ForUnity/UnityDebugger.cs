using GameSystem.GameCore.Debugger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDebugger : IDebugger
{
    private static object instLock = new object();
    private static UnityDebugger _inst;
    public static UnityDebugger instance
    {
        get
        {
            if (_inst == null)
            {
                lock (instLock)
                {
                    if (_inst == null)
                    {
                        _inst = new UnityDebugger();
                    }
                }
            }
            return _inst;
        }
    }

    public void Log(object obj)
    {
#if UNITY_EDITOR
        Debug.Log(obj);
#endif
    }

    public void LogError(object obj)
    {
#if UNITY_EDITOR
        Debug.LogError(obj);
#endif
    }

    public void LogWarning(object obj)
    {
#if UNITY_EDITOR
        Debug.LogWarning(obj);
#endif
    }
}
