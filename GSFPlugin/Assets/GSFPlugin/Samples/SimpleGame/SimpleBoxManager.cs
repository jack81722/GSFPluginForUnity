using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.SerializableMath;
using System.Collections;
using System.Collections.Generic;

public class SimpleBoxManager : Component
{
    GameObject box_go1, box_go2;
    SimpleBox char_com1, char_com2;
    BoxCollider char_col1, char_col2;

    public override void Start()
    {
        var joinReqs = GetJoinRequests();
        for(int i = 0; i < joinReqs.Length; i++)
        {
            Log("Accept");
            joinReqs[i].Accept(GetGameID());
        }

        box_go1 = CreateGameObject();
        box_go1.Name = "Box1";
        // add simple box component
        char_com1 = box_go1.AddComponent<SimpleBox>();
        char_com1.velocity = new Vector3(3, 0, 0);
        // add collider component
        char_col1 = box_go1.AddComponent<BoxCollider>();
        char_col1.SetSize(new Vector3(0.1f));

        // create box2 cloned by box1
        box_go2 = Instantiate(box_go1);
        box_go2.Name = "box2";
        // get simple box component
        char_com2 = box_go2.GetComponent<SimpleBox>();
        char_com2.velocity = new Vector3(-3, 0, 0);
        // get collider component
        char_col2 = box_go2.GetComponent<BoxCollider>();


        // initialize positions of boxs
        box_go1.transform.position = new Vector3(-10, 0, 0);
        box_go2.transform.position = new Vector3(10, 0, 0);

        Log($"Box1 started moving at {box_go1.transform.position}");
        Log($"Box2 started moving at {box_go2.transform.position}");
    }

    public float[] ToFloatArray(Vector3 v)
    {
        return new float[] { v.x, v.y, v.z };
    }

    public override void Update()
    {
        float second = (float)DeltaTime.TotalSeconds;
        box_go1.transform.position += char_com1.velocity * second;
        box_go2.transform.position += char_com2.velocity * second;
        float[][] positions = new float[][] { ToFloatArray(box_go1.transform.position), ToFloatArray(box_go2.transform.position) };
        Broadcast(new object[] { 1, positions }, GameSystem.GameCore.Network.Reliability.Sequence);
    }
}
