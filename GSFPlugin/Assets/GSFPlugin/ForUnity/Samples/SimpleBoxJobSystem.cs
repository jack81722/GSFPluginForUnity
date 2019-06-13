using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBoxJobSystem : MonoBehaviour
{
    public Stack<SimpleBoxInfo> storage;
    public List<SimpleBoxInfo> boxList;

    private void Start()
    {
        storage = new Stack<SimpleBoxInfo>();
        boxList = new List<SimpleBoxInfo>();
    }

    public void Receive(GSFPacket[] packet)
    {
        // find all add, remove, update target

    }

    public SimpleBoxInfo Get()
    {
        SimpleBoxInfo box = storage.Pop();
        box.gameObject.SetActive(true);
        boxList.Add(box);
        return box;
    }

    public void Recycle(SimpleBoxInfo box)
    {
        boxList.Remove(box);
        box.gameObject.SetActive(false);
        storage.Push(box);
    }

}
