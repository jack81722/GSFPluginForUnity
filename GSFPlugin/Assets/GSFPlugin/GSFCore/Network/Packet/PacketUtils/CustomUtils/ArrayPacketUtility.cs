using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GameSystem.GameCore.Network;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-14, true)]
    public class ArrayPacketUtility : CustomPacketUtility
    {
        public ArrayPacketUtility()
        {
            classID = -14;
        }
        
        public override GSFPacket pack(object obj)
        {
            Type type = obj.GetType();
            Type elementType = type.GetElementType();
            TypeInfo typeInfo = PackType(type);
            PacketUtility util = GetUtil(elementType);
            int length = (int)type.GetProperty("Length").GetValue(obj);
            MethodInfo getMethod = type.GetMethod("Get");
            GSFPacket[] values = new GSFPacket[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = util.pack(getMethod.Invoke(obj, new object[] { i }));
            }
            return new GSFPacket(classID, new object[] { typeInfo, values });
        }

        public override object unpack(GSFPacket packet)
        {
            object[] data = (object[])packet.data;
            Type type = UnpackType((TypeInfo)data[0]);
            GSFPacket[] values = (GSFPacket[])data[1];
            // get utility of element type
            PacketUtility util = GetUtil(type.GetElementType());
            object array = Activator.CreateInstance(type, new object[] { values.Length });
            MethodInfo setMethod = type.GetMethod("Set");
            for(int i = 0; i < values.Length; i++)
            {
                setMethod.Invoke(array, new object[] { i, util.unpack(values[i]) });
            }
            return array;
        }
    }
}
