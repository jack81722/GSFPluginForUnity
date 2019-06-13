using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class PackableAttribute : Attribute
    {
        public short classID;

        public PackableAttribute(short id)
        {
            classID = id;
        }
    }
}