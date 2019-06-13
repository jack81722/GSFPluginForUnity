using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-11, true)]
    public class CharPacketUtility : CustomPacketUtility<char>
    {
        public override GSFPacket Pack(char obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override char Unpack(GSFPacket packet)
        {
            return (char)packet.data;
        }
    }
}
