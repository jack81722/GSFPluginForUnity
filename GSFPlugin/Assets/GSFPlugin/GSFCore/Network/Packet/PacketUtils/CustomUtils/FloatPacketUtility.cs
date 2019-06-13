using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-9, true)]
    public class FloatPacketUtility : CustomPacketUtility<float>
    {
        public override GSFPacket Pack(float obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override float Unpack(GSFPacket packet)
        {
            return (float)packet.data;
        }
    }
}
