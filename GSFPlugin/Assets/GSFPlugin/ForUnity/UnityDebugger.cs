using GameSystem.GameCore.Debugger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDebugger : LazySingleton<UnityDebugger>, IDebugger
{
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
