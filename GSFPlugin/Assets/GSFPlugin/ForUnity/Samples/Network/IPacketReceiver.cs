using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketReceiver
{
    int OperationCode { get; }
    void Receive(object packet);
}

[System.Serializable]
public class GenericPacket
{
    public int InstCode;
    public object Data;
}