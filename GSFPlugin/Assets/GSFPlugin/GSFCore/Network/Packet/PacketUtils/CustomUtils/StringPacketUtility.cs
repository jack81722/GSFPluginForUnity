using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-12, true)]
    public class StringPacketUtility : CustomPacketUtility<string>
    {
        public override GSFPacket Pack(string obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override string Unpack(GSFPacket packet)
        {
            return (string)packet.data;
        }
    }
}
