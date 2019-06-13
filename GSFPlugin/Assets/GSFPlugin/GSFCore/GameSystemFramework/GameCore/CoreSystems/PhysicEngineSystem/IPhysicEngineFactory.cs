using GameSystem.GameCore.Debugger;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Physics
{
    public interface IPhysicEngineFactory
    {
        PhysicEngineProxy Create(IDebugger debugger);
    }
}
