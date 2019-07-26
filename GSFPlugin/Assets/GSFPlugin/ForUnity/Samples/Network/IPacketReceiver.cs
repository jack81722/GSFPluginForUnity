using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketReceiver
{
    int OperationCode { get; }
    void Receive(object packet);
}
