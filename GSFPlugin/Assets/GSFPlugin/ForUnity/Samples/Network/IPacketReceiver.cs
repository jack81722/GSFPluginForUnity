using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketReceiver
{
    int Code { get; }
    void Receive(object packet);
}
