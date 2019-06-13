using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class UnpackableAttribute : Attribute
    {
        public short classID;

        public UnpackableAttribute(short id)
        {
            classID = id;
        }
    }
}