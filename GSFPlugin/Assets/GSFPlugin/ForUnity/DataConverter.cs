using System.Collections;
using System.Collections.Generic;

public static class DataConverter
{
    public static UnityEngine.Vector3 ToUnity(this GameSystem.GameCore.SerializableMath.Vector3 v)
    {
        return new UnityEngine.Vector3(v.x, v.y, v.z);
    }

    public static GameSystem.GameCore.SerializableMath.Vector3 ToSerializable(this UnityEngine.Vector3 v)
    {
        return new GameSystem.GameCore.SerializableMath.Vector3(v.x, v.y, v.z);
    }

    public static UnityEngine.Quaternion ToUnity(this GameSystem.GameCore.SerializableMath.Quaternion q)
    {
        return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
    }

    public static GameSystem.GameCore.SerializableMath.Quaternion ToSerializable(this UnityEngine.Quaternion q)
    {
        return new GameSystem.GameCore.SerializableMath.Quaternion(q.x, q.y, q.z, q.w);
    }
}
