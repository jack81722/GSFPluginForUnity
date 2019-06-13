using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Debugger
{
    public interface IDebugger
    {
        void Log(object obj);
        void LogWarning(object obj);
        void LogError(object obj);
    }
}
