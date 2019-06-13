using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-13, true)]
    public class ObjectPacketUtility : CustomPacketUtility<object>
    {
        public override GSFPacket Pack(object obj)
        {
            Type type = obj.GetType();
            PacketUtility util = GetUtil(type);
            return util.pack(obj);
        }

        public override object Unpack(GSFPacket packet)
        {
            PacketUtility util = GetUtil(packet.classID);
            return util.unpack(packet);
        }

    }
}
