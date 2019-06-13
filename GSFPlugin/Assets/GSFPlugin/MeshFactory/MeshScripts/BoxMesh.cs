using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BoxMesh : AutoMesh
{
    public void SetHalfSize(float x, float y, float z)
    {
        Clear();
        Vector3 center = transform.localPosition;
        float xMin = center.x - x, xMax = center.x + x;
        float yMin = center.y - y, yMax = center.y + y;
        float zMin = center.z - z, zMax = center.z + z;

        // freeze min x
        AddQuad(
            new Vector3(xMin, yMin, zMin),
            new Vector3(xMin, yMax, zMin),
            new Vector3(xMin, yMin, zMax),
            new Vector3(xMin, yMax, zMax)
        );
        // freeze max x
        AddQuad(
            new Vector3(xMax, yMin, zMin),
            new Vector3(xMax, yMin, zMax),
            new Vector3(xMax, yMax, zMin),
            new Vector3(xMax, yMax, zMax)
        );
        // freeze max y
        AddQuad(
            new Vector3(xMin, yMax, zMin),
            new Vector3(xMax, yMax, zMin),
            new Vector3(xMin, yMax, zMax),
            new Vector3(xMax, yMax, zMax)
        );
        // freeze min y
        AddQuad(
            new Vector3(xMin, yMin, zMin),
            new Vector3(xMin, yMin, zMax),
            new Vector3(xMax, yMin, zMin),
            new Vector3(xMax, yMin, zMax)
        );
        // freeze min z
        AddQuad(
            new Vector3(xMin, yMin, zMin),
            new Vector3(xMax, yMin, zMin),
            new Vector3(xMin, yMax, zMin),
            new Vector3(xMax, yMax, zMin)
        );
        // freeze max z
        AddQuad(
            new Vector3(xMin, yMin, zMax),
            new Vector3(xMin, yMax, zMax),
            new Vector3(xMax, yMin, zMax),
            new Vector3(xMax, yMax, zMax)
        );

        Refresh();
    }
}
