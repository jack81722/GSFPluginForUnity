using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameSystem.GameCore.Network
{
    public class DefaultPacketUtility : PacketUtility
    {
        protected MemberUtility[] memberUtils;

        public DefaultPacketUtility(Type type)
        {
            classType = type;
            if (classType.IsDefined(packableAttr))
            {
                PackableAttribute packAttr = type.GetCustomAttribute<PackableAttribute>();
                classID = packAttr.classID;
                isPackable = true;
            }
            else if (classType.IsDefined(unpackableAttr))
            {
                UnpackableAttribute unpackAttr = type.GetCustomAttribute<UnpackableAttribute>();
                classID = unpackAttr.classID;
                isPackable = false;
            }
            else
            {
                throw new InvalidOperationException("Cannot initialize packet utility without Packable or Unpackable attribute.");
            }
            initMemberUtils(type);
        }

        /// <summary>
        /// Initialize all of members in utility
        /// </summary>
        protected void initMemberUtils(Type type)
        {
            // get all member and build member packers
            List<MemberUtility> memUtils = new List<MemberUtility>();
            MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < members.Length; i++)
            {
                // check if member has member attribute
                if (members[i].IsDefined(packetMemAttr))
                {
                    PacketMemberAttribute memAttr = members[i].GetCustomAttribute<PacketMemberAttribute>();
                    byte memID = memAttr.memeberID;
                    memUtils.Add(new MemberUtility(memID, members[i]));
                }
            }
            memUtils.Sort(MemberUtility.Compare);
            memberUtils = memUtils.ToArray();
        }

        /// <summary>
        /// Pack object into packet
        /// </summary>
        public override GSFPacket pack(object obj)
        {
            if (isPackable)
            {
                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                for (int i = 0; i < memberUtils.Length; i++)
                {
                    dict.Add(memberUtils[i].memberID, memberUtils[i].pack(memberUtils[i].Get(obj)));
                }
                return new GSFPacket(classID, dict);
            }
            throw new InvalidOperationException("Object must be packable.");
        }

        /// <summary>
        /// Unpack packet
        /// </summary>
        public override object unpack(GSFPacket packet)
        {
            if (packet.classID != classID)
                throw new InvalidOperationException("Cannot unpack packet by different class id.");

            object instance = Activator.CreateInstance(classType);
            Dictionary<byte, object> dict = (Dictionary<byte, object>)packet.data;
            for (int i = 0; i < memberUtils.Length; i++)
            {
                object value;
                if (dict.TryGetValue(memberUtils[i].memberID, out value))
                {
                    // unpack value and set unpacked value to member
                    memberUtils[i].Set(instance, memberUtils[i].unpack((GSFPacket)value));
                }
            }
            return instance;
        }

        /// <summary>
        /// Unpack packet and override values of target
        /// </summary>
        /// <param name="target">overrided target</param>
        public override void unpack(ref object target, GSFPacket packet)
        {
            Dictionary<byte, object> dict = (Dictionary<byte, object>)packet.data;
            for (int i = 0; i < memberUtils.Length; i++)
            {
                object value;
                if (dict.TryGetValue(memberUtils[i].memberID, out value))
                {
                    // unpack value and set unpacked value to member
                    memberUtils[i].Set(target, memberUtils[i].unpack((GSFPacket)value));

                }
            }
        }

    }
}