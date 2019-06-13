using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-5, true)]
    public class IntPacketUtility : CustomPacketUtility<int>
    {
        public override GSFPacket Pack(int obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override int Unpack(GSFPacket packet)
        {
            return (int)packet.data;
        }
    }
}
