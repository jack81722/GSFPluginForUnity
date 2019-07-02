using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBoxJobSystem : MonoBehaviour, IPacketReceiver
{
    public ClientPeerLauncher peerLauncher;
    private ClientPeer peer;

    public ClientSimpleBoxPool pool;
    public ClientSimpleBox prefab;

    private void Start()
    {
        peer = peerLauncher.peer;
        pool = new ClientSimpleBoxPool(prefab);
        pool.Supple(10);
    }

    FormmaterSerializer serializer = new FormmaterSerializer();

    public int OperationCode { get { return SimpleGameMetrics.OperationCode.Game; } }

    public Vector3 ToVector3(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    public void Receive(object packet)
    {
        pool.Update((SimpleBoxManager.BoxInfo[])packet);
    }

    private void Update()
    {
        if (peer != null && peer.isConnected)
        {
            float horizon = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            //peerLauncher.Send(
            //    SimpleGameMetrics.OperationCode.Game, 
            //    new object[]{ SimpleGameMetrics.ClientGameSwitchCode.Control, new float[] { horizon, 0, vertical } }, 
            //    Reliability.Unreliable);
        }
    }
}

public class ClientSimpleBoxPool : TrackableObjectPool<ClientSimpleBox>
{
    private ClientSimpleBox prefab;

    public ClientSimpleBoxPool(ClientSimpleBox prefab)
    {
        this.prefab = prefab;
    }

    protected override int Comparison(ClientSimpleBox x, ClientSimpleBox y)
    {
        return x.id.CompareTo(y.id);
    }

    protected override void SuppleHandler(ClientSimpleBox item)
    {
        item.gameObject.SetActive(false);
    }

    protected override void GetHandler(ClientSimpleBox item)
    {
        item.gameObject.SetActive(true);
    }

    protected override void RecycleHandler(ClientSimpleBox item)
    {
        item.gameObject.SetActive(false);
    }

    protected override ClientSimpleBox Create()
    {
        return GameObject.Instantiate<ClientSimpleBox>(prefab);
    }

    protected override void Destroy(ClientSimpleBox item)
    {
        GameObject.Destroy(item);
    }

    public void Update(SimpleBoxManager.BoxInfo[] packet)
    {
        tracker.Diff<SimpleBoxManager.BoxInfo>(packet, 
            out List<SimpleBoxManager.BoxInfo> added, 
            out List<ClientSimpleBox> removed, 
            out List<ClientSimpleBox> existed, 
            out List<SimpleBoxManager.BoxInfo> updated, 
            Compare);
        for(int i = 0; i < added.Count; i++)
        {
            ClientSimpleBox box = Get();
            box.id = added[i].boxId;
            box.transform.position = new Vector3(added[i].boxPos[0], added[i].boxPos[1], added[i].boxPos[2]);
        }
        Recycle(removed);
        for(int i = 0; i < existed.Count; i++)
        {
            existed[i].transform.position = new Vector3(updated[i].boxPos[0], updated[i].boxPos[1], updated[i].boxPos[2]);
        }
    }

    public int Compare(ClientSimpleBox box, SimpleBoxManager.BoxInfo info)
    {   
        int result = info.boxId.CompareTo(box.id);
        return result;
    }
}


