using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-8, true)]
    public class ULongPacketUtility : CustomPacketUtility<ulong>
    {
        public override GSFPacket Pack(ulong obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override ulong Unpack(GSFPacket packet)
        {
            return (ulong)packet.data;
        }
    }
}
