﻿using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Unpackable(0)]
public class ClientSimpleBox : MonoBehaviour
{
    [PacketMember(0)]
    public int id;
    [PacketMember(1)]
    public Vector3 position;

}

