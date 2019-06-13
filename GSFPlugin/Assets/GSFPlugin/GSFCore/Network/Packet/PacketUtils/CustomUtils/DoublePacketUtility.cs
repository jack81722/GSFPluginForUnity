using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-10, true)]
    public class DoublePacketUtility : CustomPacketUtility<double>
    {
        public override GSFPacket Pack(double obj)
        {
            return new GSFPacket(classID, obj);
        }

        public override double Unpack(GSFPacket packet)
        {
            return (double)packet.data;
        }
    }
}
