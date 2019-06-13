using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [Serializable]
    public class TypeInfo
    {
        public short BaseType;
        public TypeInfo[] GenericArgs;
    }
}
