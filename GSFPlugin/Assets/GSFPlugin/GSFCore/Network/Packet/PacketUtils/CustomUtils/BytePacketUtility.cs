using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-2, true)]
    public class BytePacketUtility : CustomPacketUtility<byte>
    {
        public override GSFPacket Pack(byte obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override byte Unpack(GSFPacket packet)
        {
            return (byte)packet.data;
        }
    }
}
