using GameSystem.GameCore.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFactory : MonoBehaviour
{
    private static MeshFactory _instance;
    public static MeshFactory instance
    {
        get
        {
            if(_instance == null)
            {
                if((_instance = FindObjectOfType<MeshFactory>()) == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<MeshFactory>();
                }
            }
            return _instance;
        }
    }

    public Material defaultMaterial;

    public void Start()
    {
        
    }

    public BoxMesh SpawnBoxMesh(Vector3 halfSize)
    {
        GameObject go = new GameObject();
        BoxMesh mesh = go.AddComponent<BoxMesh>();
        mesh.material = defaultMaterial;
        mesh.SetHalfSize(halfSize.x, halfSize.y, halfSize.z);
        return mesh;
    }

    public BoxMesh SpawnBoxMesh(GameObject go, Vector3 halfSize)
    {
        BoxMesh mesh = go.AddComponent<BoxMesh>();
        mesh.material = defaultMaterial;
        mesh.SetHalfSize(halfSize.x, halfSize.y, halfSize.z);
        return mesh;
    }

    public SphereMesh SpawnSphereMesh(float radius)
    {
        int subdivisions = 4;
        GameObject go = new GameObject();
        SphereMesh mesh = go.AddComponent<SphereMesh>();
        mesh.material = defaultMaterial;
        mesh.subdivisions = subdivisions;
        mesh.SetRadius(radius);
        return mesh;
    }

    public SphereMesh SpawnSphereMesh(GameObject go, float radius)
    {
        int subdivisions = 4;
        SphereMesh mesh = go.AddComponent<SphereMesh>();
        mesh.material = defaultMaterial;
        mesh.subdivisions = subdivisions;
        mesh.SetRadius(radius);
        return mesh;
    }

    public SphereMesh SpawnSphereMesh(float radius, int subdivisions = 4)
    {
        // clamp division in [1, 6]
        subdivisions = Mathf.Clamp(subdivisions, 1, 6);
        GameObject go = new GameObject();
        SphereMesh mesh = go.AddComponent<SphereMesh>();
        mesh.material = defaultMaterial;
        mesh.subdivisions = subdivisions;
        mesh.SetRadius(radius);
        return mesh;
    }

    public AutoMesh SpawnMesh(IBoxShape shape)
    {
        GameObject go = new GameObject();
        BoxMesh mesh = go.AddComponent<BoxMesh>();
        mesh.material = defaultMaterial;
        mesh.SetHalfSize(shape.HalfSize.x, shape.HalfSize.y, shape.HalfSize.z);
        return mesh;
    }

    public AutoMesh SpawnMesh(ISphereShape shape, int subdivisions = 4)
    {
        // clamp division in [1, 6]
        subdivisions = Mathf.Clamp(subdivisions, 1, 6);
        GameObject go = new GameObject();
        SphereMesh mesh = go.AddComponent<SphereMesh>();
        mesh.material = defaultMaterial;
        mesh.subdivisions = subdivisions;
        mesh.SetRadius(shape.Radius);
        return mesh;
    }
}
