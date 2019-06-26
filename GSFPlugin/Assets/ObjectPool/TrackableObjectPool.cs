using AdvancedGeneric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrackableObjectPool<T> : ObjectPool<T>
{
    protected AutoSortList<T> tracker;
    public int UsingCount { get { return tracker.Count; } }

    public TrackableObjectPool()
    {
        tracker = new AutoSortList<T>(Comparison);
    }

    protected abstract int Comparison(T x, T y);

    protected abstract override T Create();

    public override T Get()
    {
        T item = base.Get();
        tracker.Add(item);
        return item;
    }

    public override void Recycle(T item)
    {
        base.Recycle(item);
        tracker.Remove(item);
    }

    public override void Recycle(IEnumerable<T> items)
    {
        base.Recycle(items);
        tracker.ExceptWith(items);
    }

    protected abstract override void Destroy(T item);

    public void RecycleAll()
    {
        Recycle(tracker.ToList());
        tracker.Clear();
    }

    public override void ReleaseAll()
    {
        RecycleAll();
        base.ReleaseAll();
    }

}
