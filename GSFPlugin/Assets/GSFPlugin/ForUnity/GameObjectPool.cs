using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : TrackableObjectPool<GameObject>
{
    public GameObject prefab;

    public GameObjectPool(GameObject prefab)
    {
        this.prefab = prefab;
    }

    protected override int Comparison(GameObject x, GameObject y)
    {
        return x.GetInstanceID().CompareTo(y.GetInstanceID());
    }

    protected override GameObject Create()
    {
        return GameObject.Instantiate(prefab);
    }

    protected override void SuppleHandler(GameObject item)
    {
        item.SetActive(false);
    }

    protected override void GetHandler(GameObject item)
    {
        item.SetActive(true);
    }

    protected override void RecycleHandler(GameObject item)
    {
        item.SetActive(false);
    }

    protected override void Destroy(GameObject item)
    {
        GameObject.Destroy(item);
    }
}
