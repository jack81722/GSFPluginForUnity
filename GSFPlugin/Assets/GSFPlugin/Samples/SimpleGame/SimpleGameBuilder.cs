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
        float speed = 3f;

        // create box1
        GameObject box_go1 = CreateGameObject();
        var char_com = box_go1.AddComponent<SimpleBox>();
        Log($"go == null? {char_com.gameObject == null}");
        char_com.speed = speed;
        var char_col = box_go1.AddComponent<BoxCollider>();
        char_col.SetSize(new Vector3(0.1f));

        // create box2 cloned by box1
        GameObject box_go2 = Instantiate(box_go1);
        box_go2.GetComponent<SimpleBox>().speed = -speed;

        // initialize positions of boxs
        box_go1.transform.position = new Vector3(-10, 0, 0);
        box_go2.transform.position = new Vector3(10, 0, 0);

        Log($"Box1 started moving at {box_go1.transform.position}");
        Log($"Box2 started moving at {box_go2.transform.position}");
    }
}
