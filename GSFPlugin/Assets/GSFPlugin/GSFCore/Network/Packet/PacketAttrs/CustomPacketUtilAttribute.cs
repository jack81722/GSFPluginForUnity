using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPacketUtilAttribute : Attribute
    {
        public short classID { get; private set; }
        public bool isDefault { get; private set; }

        public CustomPacketUtilAttribute(short id, bool isDefault = false)
        {
            classID = id;
            this.isDefault = isDefault;
        }
    }
}
