using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultIGSFFactory : MonoBehaviour, IGSFFactory
{
    public Dictionary<short, Type> monoDict;

    public GameObject Create(GSFPacket packet)
    {
        GameObject gameObject = new GameObject();
        var component = gameObject.AddComponent(monoDict[packet.classID]);
        PacketUtility.Unpack(ref component, packet);
        return gameObject;
    }
}