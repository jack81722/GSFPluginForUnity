using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-7, true)]
    public class LongPacketUtility : CustomPacketUtility<long>
    {
        public override GSFPacket Pack(long obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override long Unpack(GSFPacket packet)
        {
            return (long)packet.data;
        }
    }
}
