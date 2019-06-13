using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameSystem.GameCore.Network
{
    [CustomPacketUtil(-15, true)]
    public class DictionaryPacketUtility : CustomPacketUtility
    {
        public DictionaryPacketUtility()
        {
            classID = -15;
            classType = typeof(Dictionary<,>);
        }
        
        public override GSFPacket pack(object dict)
        {
            Type dictType = dict.GetType();
            Type[] genericArgs = dictType.GetGenericArguments();
            TypeInfo typeArgs = PackType(dictType);
            int length = (int)dictType.GetProperty("Count").GetValue(dict);
            Type pairType = typeof(KeyValuePair<,>).MakeGenericType(genericArgs);
            PropertyInfo keyProp = pairType.GetProperty("Key");
            PacketUtility keyUtil = GetUtil(keyProp.PropertyType);
            PropertyInfo valProp = pairType.GetProperty("Value");
            PacketUtility valUtil = GetUtil(valProp.PropertyType);
            IEnumerable e = (IEnumerable)dict;
            object[] values = new object[length * 2];
            int p = 0;
            foreach (var pair in e)
            {
                values[p++] = keyUtil.pack(keyProp.GetValue(pair));
                values[p++] = valUtil.pack(valProp.GetValue(pair));
            }
            return new GSFPacket(100, new object[] { typeArgs, values });
        }

        public override object unpack(GSFPacket packet)
        {
            object[] data = (object[])packet.data;
            Type dictType = UnpackType((TypeInfo)data[0]);
            Type keyType = dictType.GetGenericArguments()[0];
            Type valType = dictType.GetGenericArguments()[1];
            PacketUtility keyUtil = GetUtil(keyType);
            PacketUtility valUtil = GetUtil(valType);
            object dict = Activator.CreateInstance(dictType);
            object[] values = (object[])data[1];
            MethodInfo addMethod = dictType.GetMethod("Add");
            for (int i = 0; i < values.Length; i += 2)
            {
                addMethod.Invoke(dict, new object[] { keyUtil.unpack((GSFPacket)values[i], keyType), valUtil.unpack((GSFPacket)values[i + 1], valType) });
            }
            return dict;
        }

        public override T unpack<T>(GSFPacket packet)
        {
            object[] data = (object[])packet.data;
            Type dictType = typeof(T);
            Type keyType = dictType.GetGenericArguments()[0];
            Type valType = dictType.GetGenericArguments()[1];
            PacketUtility keyUtil = GetUtil(keyType);
            PacketUtility valUtil = GetUtil(valType);
            T dict = Activator.CreateInstance<T>();
            object[] values = (object[])data[1];
            MethodInfo addMethod = dictType.GetMethod("Add");
            for (int i = 0; i < values.Length; i += 2)
            {
                addMethod.Invoke(dict, new object[] { keyUtil.unpack((GSFPacket)values[i], keyType), valUtil.unpack((GSFPacket)values[i + 1], valType) });
            }
            return dict;
        }

        public override void unpack(ref object target, GSFPacket packet)
        {
            object[] data = (object[])packet.data;
            Type dictType = target.GetType();
            Type keyType = dictType.GetGenericArguments()[0];
            Type valType = dictType.GetGenericArguments()[1];
            PacketUtility keyUtil = GetUtil(keyType);
            PacketUtility valUtil = GetUtil(valType);
            object[] values = (object[])data[1];
            MethodInfo clearMethod = dictType.GetMethod("Clear");
            clearMethod.Invoke(target, new object[0]);
            MethodInfo addMethod = dictType.GetMethod("Add");
            for (int i = 0; i < values.Length; i += 2)
            {
                addMethod.Invoke(target, new object[] { keyUtil.unpack((GSFPacket)values[i], keyType), valUtil.unpack((GSFPacket)values[i + 1], valType) });
            }
        }


        public override object unpack(GSFPacket packet, Type type)
        {
            object[] data = (object[])packet.data;
            Type keyType = type.GetGenericArguments()[0];
            Type valType = type.GetGenericArguments()[1];
            PacketUtility keyUtil = GetUtil(keyType);
            PacketUtility valUtil = GetUtil(valType);
            object dict = Activator.CreateInstance(type);
            object[] values = (object[])data[1];
            MethodInfo addMethod = type.GetMethod("Add");
            for (int i = 0; i < values.Length; i += 2)
            {
                addMethod.Invoke(dict, new object[] { keyUtil.unpack((GSFPacket)values[i]), valUtil.unpack((GSFPacket)values[i + 1]) });
            }
            return dict;
        }
    }
}
