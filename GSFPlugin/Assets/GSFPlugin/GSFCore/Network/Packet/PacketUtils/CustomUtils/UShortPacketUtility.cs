using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-4, true)]
    public class UShortPacketUtility : CustomPacketUtility<ushort>
    {
        public override GSFPacket Pack(ushort obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override ushort Unpack(GSFPacket packet)
        {
            return (ushort)packet.data;
        }
    }
}
