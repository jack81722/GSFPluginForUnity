using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSFGoView : MonoBehaviour
{
    public uint SID;
    public string GameObjectName;
    public GameSystem.GameCore.GameSourceAdapter adapter;

    public List<GSFComponentView> components;

    public void Set(GameSystem.GameCore.GameSourceAdapter adapter)
    {
        SID = adapter.SID;
        this.adapter = adapter;
    }

    private void LateUpdate()
    {
        GameObjectName = adapter.GetMember("Name").stringValue;
    }

    public void Add(GameSystem.GameCore.GameSourceAdapter gsAdapter)
    {
        GSFComponentView component = gameObject.AddComponent<GSFComponentView>();
        components.Add(component);
    }

    public void Remove(GSFComponentView component)
    {
        components.Remove(component);
    }

    public void Clear()
    {
        for(int i = 0; i < components.Count; i++)
        {
            Destroy(components[i]);
        }
        components.Clear();
    }
    
}
