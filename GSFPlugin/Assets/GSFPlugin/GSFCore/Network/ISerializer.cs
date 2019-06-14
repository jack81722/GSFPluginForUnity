using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.GameCore.Network
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);
        T Deserialize<T>(byte[] bytes);
    }

    public interface ISerializer<T>
    {
        byte[] Serialize(T item);
        T Deserialize(byte[] bytes);
    }
}