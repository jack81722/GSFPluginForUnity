using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-1, true)]
    public class BoolPacketUtility : CustomPacketUtility<bool>
    {
        public override GSFPacket Pack(bool obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override bool Unpack(GSFPacket packet)
        {
            return (bool)packet.data;
        }
    }
}
