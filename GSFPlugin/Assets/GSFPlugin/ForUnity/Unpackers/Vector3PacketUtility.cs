using System.Collections;
using System.Collections.Generic;
using GameSystem.GameCore.Network;
using UnityEngine;

[CustomPacketUtil(-31, true)]
public class Vector3PacketUtility : CustomPacketUtility<Vector3>
{
    public override GSFPacket Pack(Vector3 obj)
    {
        return new GSFPacket(classID,
            new float[] { obj.x, obj.y, obj.z });
    }

    public override Vector3 Unpack(GSFPacket packet)
    {
        float[] values = (float[])packet.data;
        return new Vector3(values[0], values[1], values[2]);
    }
}
