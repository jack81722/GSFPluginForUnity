using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    public enum Reliability
    {
        ReliableOrder = 3,
        ReliableSequence = 4,
        ReliableUnorder = 1,
        Sequence = 2,
        Unreliable = 0
    }

    public interface IPeer
    {
        void Send(object obj, Reliability reliability);
        void Receive(object obj);
    }

    public interface ISerializer
    {
        byte[] Serialize(object obj);
        object Deserialize(byte[] bytes);
    }

    public interface ISerializer<T>
    {
        byte[] Serialize(T item);
        T Deserialize(byte[] bytes);
    }
}