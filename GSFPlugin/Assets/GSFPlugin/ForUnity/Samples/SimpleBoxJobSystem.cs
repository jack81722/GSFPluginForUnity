using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBoxJobSystem : MonoBehaviour
{
    public ClientPeer peer;
    public GameObjectPool pool;

    public SimpleBoxInfo prefab;

    public GameObject box1, box2;

    private void Start()
    {
        pool = new GameObjectPool(prefab.gameObject);
        pool.Supple(10);
        peer.OnRecvEvent += OnRecv;

        box1 = pool.Get();
        box2 = pool.Get();
    }

    FormmaterSerializer serializer = new FormmaterSerializer();

    public Vector3 ToVector3(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    public void OnRecv(byte[] dgram, Reliability reliability)
    {
        float[][] floats = serializer.Deserialize<float[][]>(dgram);
        Debug.Log($"box1 :{ToVector3(floats[0])}, box2 :{ToVector3(floats[1])}");
        box1.transform.localPosition = ToVector3(floats[0]);
        box2.transform.localPosition = ToVector3(floats[1]);
    }
}


