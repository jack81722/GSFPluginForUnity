using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.SerializableMath;
using System.Collections;
using System.Collections.Generic;

public class SimpleGameBuilder : SceneBuilder
{
    public SimpleGameBuilder(IDebugger debugger) : base(debugger) { }

    protected override void Building()
    {
        GameObject manager_go = CreateGameObject("Simple Box Manager");
        manager_go.AddComponent<SimpleBoxManager>();
    }
}
