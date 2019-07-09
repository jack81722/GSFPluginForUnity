﻿using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameClientManager : MonoBehaviour, IPacketReceiver
{
    public ClientPeerLauncher peerLauncher;
    private ClientPeer peer;

    public ClientSimpleBoxPool boxPool;
    public ClientSimpleBox boxPrefab;
    public Transform boxStorage;

    public ClientBullet bulletPrefab;
    public ClientBulletPool bulletPool;
    public Transform bulletStorage;

    private void Start()
    {
        peer = peerLauncher.peer;
        boxPool = new ClientSimpleBoxPool(boxPrefab, boxStorage);
        boxPool.Supple(10);

        bulletPool = new ClientBulletPool(bulletPrefab, bulletStorage);
        bulletPool.Supple(30);
    }

    FormmaterSerializer serializer = new FormmaterSerializer();

    public int OperationCode { get { return SimpleGameMetrics.OperationCode.Game; } }

    public Vector3 ToVector3(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    public void Receive(object packet)
    {
        object[] gamePacket = packet as object[];
        int switchCode = (int)gamePacket[0];

        switch (switchCode)
        {
            case SimpleGameMetrics.ServerGameSwitchCode.BoxInfo:
                boxPool.Update((BoxInfo[])gamePacket[1]);
                break;
            case SimpleGameMetrics.ServerGameSwitchCode.BulletInfo:
                bulletPool.Update((Bullet.BulletInfo[])gamePacket[1]);
                break;
        }
    }

    private void Update()
    {
        if (peer != null && peer.isConnected && peerLauncher.ExistGroup(SimpleGameMetrics.OperationCode.Game))
        {
            float horizon = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            peerLauncher.Send(
                SimpleGameMetrics.OperationCode.Game,
                new object[] { SimpleGameMetrics.ClientGameSwitchCode.Move, new float[] { horizon, 0, vertical } },
                Reliability.Unreliable);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                peerLauncher.Send(
                SimpleGameMetrics.OperationCode.Game,
                new object[] { SimpleGameMetrics.ClientGameSwitchCode.Shoot },
                Reliability.Unreliable);
            }
        }
    }
}

public class ClientSimpleBoxPool : TrackableObjectPool<ClientSimpleBox>
{
    private ClientSimpleBox prefab;
    private Transform storage;

    public ClientSimpleBoxPool(ClientSimpleBox prefab, Transform storage)
    {
        this.prefab = prefab;
        this.storage = storage;
    }

    protected override int Comparison(ClientSimpleBox x, ClientSimpleBox y)
    {
        return x.id.CompareTo(y.id);
    }

    protected override void SuppleHandler(ClientSimpleBox item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(storage);
    }

    protected override void GetHandler(ClientSimpleBox item, object arg)
    {
        item.gameObject.SetActive(true);
        item.id = (int)arg;
    }

    protected override void RecycleHandler(ClientSimpleBox item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(storage);
    }

    protected override ClientSimpleBox Create()
    {
        return GameObject.Instantiate<ClientSimpleBox>(prefab);
    }

    protected override void Destroy(ClientSimpleBox item)
    {
        GameObject.Destroy(item);
    }

    public void Update(BoxInfo[] packet)
    {
        tracker.Diff<BoxInfo>(packet, 
            out List<BoxInfo> added, 
            out List<ClientSimpleBox> removed, 
            out List<ClientSimpleBox> existed, 
            out List<BoxInfo> updated, 
            Compare);
        for(int i = 0; i < added.Count; i++)
        {
            ClientSimpleBox box = Get(added[i].boxId);
            box.transform.position = new Vector3(added[i].boxPos[0], added[i].boxPos[1], added[i].boxPos[2]);
            Quaternion q = new Quaternion(added[i].boxRot[0], added[i].boxRot[1], added[i].boxRot[2], added[i].boxRot[3]);
            //Debug.Log(q);
            box.transform.rotation = q;
        }
        Recycle(removed);
        for(int i = 0; i < existed.Count; i++)
        {
            existed[i].transform.position = new Vector3(updated[i].boxPos[0], updated[i].boxPos[1], updated[i].boxPos[2]);
            Quaternion q = new Quaternion(updated[i].boxRot[0], updated[i].boxRot[1], updated[i].boxRot[2], updated[i].boxRot[3]);
            //Debug.Log(q);
            existed[i].transform.rotation = q;
        }
    }

    public int Compare(ClientSimpleBox box, BoxInfo info)
    {   
        int result = info.boxId.CompareTo(box.id);
        return result;
    }
}

public class ClientBulletPool : TrackableObjectPool<ClientBullet>
{
    private ClientBullet prefab;
    private Transform storage;

    public ClientBulletPool(ClientBullet prefab, Transform storage)
    {
        this.prefab = prefab;
        this.storage = storage;
    }

    protected override int Comparison(ClientBullet x, ClientBullet y)
    {
        return x.id.CompareTo(y.id);
    }

    protected override void GetHandler(ClientBullet item, object arg)
    {
        item.gameObject.SetActive(true);
        Debug.Log($"Get new bullet[{arg}]");
        item.id = (int)arg;
    }

    protected override void RecycleHandler(ClientBullet item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(storage);
    }

    protected override void SuppleHandler(ClientBullet item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(storage);
    }

    protected override ClientBullet Create()
    {
        return GameObject.Instantiate(prefab);
    }

    protected override void Destroy(ClientBullet item)
    {
        GameObject.Destroy(item);
    }

    public void Update(Bullet.BulletInfo[] packet)
    {
        tracker.Diff(packet,
            out List<Bullet.BulletInfo> added,
            out List<ClientBullet> removed,
            out List<ClientBullet> existed,
            out List<Bullet.BulletInfo> updated,
            Compare);
        Debug.Log($"Added : {added.Count}, Removed : {removed.Count}, Updated : {updated.Count}");
        for(int i = 0; i < added.Count; i++)
        {
            var bullet = Get(added[i].id);
            bullet.transform.position = new Vector3(added[i].pos[0], added[i].pos[1], added[i].pos[2]);
        }
        Recycle(removed);
        for(int i = 0; i < existed.Count; i++)
        {
            existed[i].transform.position = new Vector3(updated[i].pos[0], updated[i].pos[1], updated[i].pos[2]);
        }
    }

    public int Compare(ClientBullet bullet, Bullet.BulletInfo info)
    {
        return bullet.id.CompareTo(info.id);
    }
}
