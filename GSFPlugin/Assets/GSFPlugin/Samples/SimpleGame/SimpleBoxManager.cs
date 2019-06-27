using GameSystem.GameCore;
using GameSystem.GameCore.Components;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;
using System.Collections;
using System.Collections.Generic;

public class SimpleBoxManager : Component
{
    Dictionary<int, ServerSimpleBox> boxes;
    Vector3 spawnPoint = new Vector3(0, 0, 0);
    GameObject prefab;

    public override void Start()
    {
        OnReceiveGamePacket += OnReceiveControlPacket;
        prefab = CreatePrefab();
        boxes = new Dictionary<int, ServerSimpleBox>();

        var joinReqs = GetJoinRequests();
        for(int i = 0; i < joinReqs.Length; i++)
        {
            AcceptPlayer(joinReqs[i]);
        }
    }

    public GameObject CreatePrefab()
    {
        GameObject prefab = CreateGameObject();
        prefab.Name = "Box1";
        // add simple box component
        ServerSimpleBox component = prefab.AddComponent<ServerSimpleBox>();
        component.speed = 3f;
        // add collider component
        BoxCollider collider = prefab.AddComponent<BoxCollider>();
        collider.SetSize(new Vector3(0.1f));
        prefab.SetActive(false);
        return prefab;
    }

    public void AcceptPlayer(JoinGroupRequest request)
    {
        IPeer peer = request.Accept(GetGameID());
        // check if peer is connected
        if (peer.isConnected)
        {
            GameObject go = Instantiate(prefab);
            go.SetActive(true);
            ServerSimpleBox component = go.GetComponent<ServerSimpleBox>();
            component.id = peer.Id;
            boxes.Add(component.id, component);
        }
    }

    public float[] ToFloatArray(Vector3 v)
    {
        return new float[] { v.x, v.y, v.z };
    }

    public Vector3 ToVector3(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    public void OnReceiveControlPacket(IPeer peer, object packet)
    {
        float[] direction = (float[])packet;
        Vector3 d = ToVector3(direction);
        if(boxes.TryGetValue(peer.Id, out ServerSimpleBox box))
        {
            box.direction = d;
        }
    }

    public override void Update()
    {
        float second = (float)DeltaTime.TotalSeconds;
        int posIndex = 0;
        BoxInfo[] packet = new BoxInfo[boxes.Count];
        foreach (var box in boxes.Values)
        {
            Vector3 pos = box.transform.position;
            pos += box.velocity * second;
            packet[posIndex] = new BoxInfo(box.id, pos);
            box.transform.position = pos;
        }
        
        Broadcast(new object[] { 1, packet }, Reliability.Sequence);
    }

    [System.Serializable]
    public class BoxInfo
    {
        public int boxId;
        public float[] boxPos;

        public BoxInfo(int id, float[] pos)
        {
            boxId = id;
            boxPos = pos;
        }

        public BoxInfo(int id, Vector3 pos)
        {
            boxId = id;
            boxPos = new float[] { pos.x, pos.y, pos.z };
        }

        public override string ToString()
        {
            return $"Box[{boxId}]";
        }
    }

}
