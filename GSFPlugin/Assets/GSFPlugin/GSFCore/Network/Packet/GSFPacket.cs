using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    [Serializable]
    public class GSFPacket
    {
        /// <summary>
        /// identity of class for wrapper/unwrapper
        /// </summary>
        public short classID;

        /// <summary>
        /// dictionary of data
        /// </summary>
        public object data;

        public GSFPacket()
        {
            classID = -1;
            data = new Dictionary<byte, object>();
        }

        public GSFPacket(short classID, object value)
        {
            this.classID = classID;
            data = value;
        }
    }

    
}
