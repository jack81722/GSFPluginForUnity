using GameSystem.GameCore.Debugger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDebugger : IDebugger
{
    public void Log(object obj)
    {
        Debug.Log(obj);
    }

    public void LogError(object obj)
    {
        Debug.LogError(obj);
    }

    public void LogWarning(object obj)
    {
        Debug.LogWarning(obj);
    }
}
