using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-3, true)]
    public class ShortPacketUtility : CustomPacketUtility<short>
    {
        public override GSFPacket Pack(short obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override short Unpack(GSFPacket packet)
        {
            return (short)packet.data;
        }
    }
}
