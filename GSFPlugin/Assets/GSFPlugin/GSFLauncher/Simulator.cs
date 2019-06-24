using AdvancedGeneric;
using GameSystem.GameCore.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Read collider object and create debug geometric object
/// </summary>
public class Simulator : MonoBehaviour
{
    private GSFGOViewPool pool;
    public Transform poolStorage;

    private AutoSortList<GameSystem.GameCore.GameSourceAdapter> gameSourceAdapterList;

    public MeshFactory factory;
    

    private void Awake()
    {
        gameSourceAdapterList = new AutoSortList<GameSystem.GameCore.GameSourceAdapter>(GameSystem.GameCore.GameSourceAdapter.CompareSID);
        
        factory = MeshFactory.instance;

        pool = new GSFGOViewPool(transform, poolStorage);
        pool.Supple(30);
    }

    /// <summary>
    /// Create mesh by shape
    /// </summary>
    /// <param name="gs"></param>
    public void CreateMesh(GameObject go, GameSystem.GameCore.GameSource gs)
    {
        if (gs.GetType().IsSubclassOf(typeof(GameSystem.GameCore.Collider)))
        {
            try
            {
                if (typeof(IBoxShape).IsAssignableFrom(gs.GetType()))
                {
                    Debug.Log("Create box mesh");
                    IBoxShape shape = (IBoxShape)gs;
                    //ThreadPipe.Call(factory.SpawnBoxMesh, go, shape.HalfSize.ToUnity(), null);
                }
                else if (typeof(ISphereShape).IsAssignableFrom(gs.GetType()))
                {
                    Debug.Log("Create sphere mesh");
                    ISphereShape shape = (ISphereShape)gs;
                    ThreadPipe.Call(factory.SpawnSphereMesh, shape.Radius, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    private GameObject createGameObject(GameSystem.GameCore.GameSourceAdapter gs)
    {
        if (gs.isGameObject)
        {
            // create game object and set transform
            GSFGoView goView = pool.Get();
            goView.Set(gs);
            return goView.gameObject;
        }
        else if (gs.isComponent)
        {
            //var goView = gsViews.Find(gs.SID, (sid, v) => sid.CompareTo(v.SID));
            //var comView = goView.gameObject.AddComponent<GSFComponentView>();
        }
        return null;
    }

}

