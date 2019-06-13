using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PacketMemberAttribute : Attribute
    {
        public byte memeberID { get; private set; }

        public PacketMemberAttribute(byte id)
        {
            memeberID = id;
        }
    }
}