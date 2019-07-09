using GameSystem.GameCore.SerializableMath;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BoxInfo
{
    public int boxId;
    public float[] boxPos;
    public float[] boxRot;

    public BoxInfo(int id, float[] pos)
    {
        boxId = id;
        boxPos = pos;
        boxRot = new float[] { 0, 0, 0, 1 };
    }

    public BoxInfo(int id, float[] pos, float[] rot)
    {
        boxId = id;
        boxPos = pos;
        boxRot = rot;
    }

    public BoxInfo(int id, Vector3 pos)
    {
        boxId = id;
        boxPos = new float[] { pos.x, pos.y, pos.z };
        boxRot = new float[] { 0, 0, 0, 1 };
    }

    public BoxInfo(int id, Vector3 pos, Quaternion rot)
    {
        boxId = id;
        boxPos = new float[] { pos.x, pos.y, pos.z };
        boxRot = new float[] { rot.x, rot.y, rot.z, rot.w };
    }

    public override string ToString()
    {
        return $"Box[{boxId}]";
    }
}