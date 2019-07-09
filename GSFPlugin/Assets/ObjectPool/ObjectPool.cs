using System.Collections;
using System.Collections.Generic;

public abstract class ObjectPool<T>
{
    private List<T> storage;
    private int buffer = 16;
    public int Buffer
    {
        get { return buffer; }
        set { buffer = value > 0 ? value : 1; }
    }
    public int UsableCount { get { return storage.Count; } }

    public ObjectPool()
    {
        storage = new List<T>(Buffer);
    }

    #region Create/Destroy methods
    protected abstract T Create();

    protected abstract void Destroy(T item);
    #endregion

    public void Supple(int amount)
    {
        T[] items = new T[amount];
        for(int i = 0; i < amount; i++)
        {
            items[i] = Create();
            SuppleHandler(items[i]);
        }
        storage.AddRange(items);
    }

    protected virtual void SuppleHandler(T item) { }

    #region Get methods
    protected virtual void GetHandler(T item, object arg) { }

    public virtual T Get(object arg)
    {
        if(storage.Count <= 0)
        {
            Supple(Buffer);
        }
        T item = storage[storage.Count - 1];
        storage.RemoveAt(storage.Count - 1);
        GetHandler(item, arg);
        return item;
    }

    public virtual T[] Get(int amount, object arg)
    {
        if (storage.Count < amount)
        {
            int suppleAmount = Buffer > amount ? Buffer : amount;
            Supple(suppleAmount);
        }
        T[] items = storage.GetRange(storage.Count - amount, amount).ToArray();
        storage.RemoveRange(storage.Count - amount, amount);
        for(int i = 0; i < items.Length; i++)
        {
            GetHandler(items[i], arg);
        }
        return items;
    }
    #endregion

    #region Recycle methods
    protected virtual void RecycleHandler(T item) { }

    public virtual void Recycle(T item)
    {
        storage.Add(item);
        RecycleHandler(item);
    }

    public virtual void Recycle(IEnumerable<T> items)
    {
        storage.AddRange(items);
        foreach(var item in items)
        {
            RecycleHandler(item);
        }
    }
    #endregion

    public virtual void Release(int amount)
    {
        List<T> items = storage.GetRange(storage.Count - amount, amount);
        for(int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
        storage.RemoveRange(storage.Count - amount, amount);
    }

    public virtual void ReleaseAll()
    {
        for (int i = 0; i < storage.Count; i++)
            Destroy(storage[i]);
        storage.Clear();
    }
}
