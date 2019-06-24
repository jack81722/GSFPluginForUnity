using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBoxJobSystem : MonoBehaviour, IPacketReceiver
{
    public GameObjectPool pool;

    public SimpleBoxInfo prefab;

    public GameObject box1, box2;

    private void Start()
    {
        pool = new GameObjectPool(prefab.gameObject);
        pool.Supple(10);

        box1 = pool.Get();
        box2 = pool.Get();
    }

    FormmaterSerializer serializer = new FormmaterSerializer();

    public int Code { get { return 1; } }

    public Vector3 ToVector3(float[] floats)
    {
        return new Vector3(floats[0], floats[1], floats[2]);
    }

    public void Receive(object packet)
    {
        float[][] floats = (float[][])packet;
        box1.transform.localPosition = ToVector3(floats[0]);
        box2.transform.localPosition = ToVector3(floats[1]);
    }
}

public interface IPacketReceiver
{
    int Code { get; }
    void Receive(object packet);
}
