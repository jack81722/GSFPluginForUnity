using System.Collections;
using System.Collections.Generic;
using GameSystem.GameCore.Network;
using UnityEngine;

[CustomPacketUtil(-30, true)]
public class QuaternionPacketUtility : CustomPacketUtility<Quaternion>
{
    public override GSFPacket Pack(Quaternion obj)
    {
        return new GSFPacket(classID,
            new float[] { obj.x, obj.y, obj.z, obj.w });
    }

    public override Quaternion Unpack(GSFPacket packet)
    {
        float[] values = (float[])packet.data;
        return new Quaternion(values[0], values[1], values[2], values[1]);
    }
}
