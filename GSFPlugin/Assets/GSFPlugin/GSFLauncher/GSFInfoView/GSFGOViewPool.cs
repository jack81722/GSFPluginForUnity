using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSFGOViewPool : TrackableObjectPool<GSFGoView>
{
    public Transform simTrans;
    public Transform storage;

    public GSFGOViewPool(Transform simTrans, Transform storage)
    {
        this.simTrans = simTrans;
        this.storage = storage;
    }

    protected override int Comparison(GSFGoView x, GSFGoView y)
    {
        return x.SID.CompareTo(y.SID);
    }

    protected override void GetHandler(GSFGoView item, object arg)
    {
        item.gameObject.SetActive(true);
        if (simTrans != null) item.transform.SetParent(simTrans);
    }

    protected override void SuppleHandler(GSFGoView item)
    {
        if (storage != null) item.transform.SetParent(storage);
        item.gameObject.SetActive(false);
    }

    protected override void RecycleHandler(GSFGoView item)
    {
        item.gameObject.SetActive(false);
        if (storage != null) item.transform.SetParent(storage);
    }

    protected override GSFGoView Create()
    {
        GameObject go = new GameObject();
        go.name = "GSFGOView";
        return go.AddComponent<GSFGoView>();
    }

    protected override void Destroy(GSFGoView item)
    {
        GameObject.Destroy(item);
    }
}
