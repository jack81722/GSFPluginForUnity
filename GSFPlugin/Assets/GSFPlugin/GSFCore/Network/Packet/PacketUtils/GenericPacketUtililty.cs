using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameSystem.GameCore.Network
{
    class GenericPacketUtililty : PacketUtility
    {
        public GenericPacketUtililty(Type type)
        {
            classID = type.GetCustomAttribute<PackableAttribute>().classID;
            classType = type.GetGenericTypeDefinition();
            isPackable = true;
        }

        public override GSFPacket pack(object obj)
        {
            Type type = obj.GetType();
            object[] data = new object[2];
            // save generic args
            data[0] = PackType(type);
            // add all of members
            Dictionary<byte, object> values = new Dictionary<byte, object>();
            List<MemberUtility> memUtils = initMemberUtils(type);
            for(int i = 0; i < memUtils.Count; i++)
            {
                values.Add(memUtils[i].memberID, memUtils[i].pack(memUtils[i].Get(obj)));
            }
            data[1] = values;
            return new GSFPacket(classID, data);
        }

        protected List<MemberUtility> initMemberUtils(Type type)
        {
            // get all member and build member packers
            List<MemberUtility> memUtils = new List<MemberUtility>();
            MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].IsDefined(packetMemAttr))
                {
                    PacketMemberAttribute memAttr = members[i].GetCustomAttribute<PacketMemberAttribute>();
                    byte memID = memAttr.memeberID;
                    memUtils.Add(new MemberUtility(memID, members[i]));
                }
            }
            memUtils.Sort(MemberUtility.Compare);
            return memUtils;
        }

        public override object unpack(GSFPacket packet)
        {
            object[] data = (object[])packet.data;
            Type type = UnpackType((TypeInfo)data[0]);
            object instance = Activator.CreateInstance(type);
            Dictionary<byte, object> values = (Dictionary<byte, object>)((object[])packet.data)[1];
            List<MemberUtility> memUtils = initMemberUtils(type);
            object value;
            for (int i = 0; i < memUtils.Count; i++)
            {
                if (values.TryGetValue(memUtils[i].memberID, out value))
                    memUtils[i].Set(instance, memUtils[i].unpack((GSFPacket)value));
            }
            return instance;
        }

        public override T unpack<T>(GSFPacket packet)
        {
            object[] data = (object[])packet.data;
            Type type = typeof(T);
            T instance = Activator.CreateInstance<T>();
            Dictionary<byte, object> values = (Dictionary<byte, object>)((object[])packet.data)[1];
            List<MemberUtility> memUtils = initMemberUtils(type);
            object value;
            for (int i = 0; i < memUtils.Count; i++)
            {
                if (values.TryGetValue(memUtils[i].memberID, out value))
                    memUtils[i].Set(instance, memUtils[i].unpack((GSFPacket)value));
            }
            return instance;
        }

        public override void unpack(ref object target, GSFPacket packet)
        {
            Type type = target.GetType();
            Dictionary<byte, object> values = (Dictionary<byte, object>)((object[])packet.data)[1];
            List<MemberUtility> memUtils = initMemberUtils(type);
            object value;
            for (int i = 0; i < memUtils.Count; i++)
            {
                if (values.TryGetValue(memUtils[i].memberID, out value))
                    memUtils[i].Set(target, memUtils[i].unpack((GSFPacket)value));
            }
        }
    }
}
