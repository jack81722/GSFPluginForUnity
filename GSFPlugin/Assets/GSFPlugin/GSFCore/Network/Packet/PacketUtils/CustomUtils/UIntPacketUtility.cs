using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-6, true)]
    public class UIntPacketUtility : CustomPacketUtility<uint>
    {
        public override GSFPacket Pack(uint obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override uint Unpack(GSFPacket packet)
        {
            return (uint)packet.data;
        }
    }
}
