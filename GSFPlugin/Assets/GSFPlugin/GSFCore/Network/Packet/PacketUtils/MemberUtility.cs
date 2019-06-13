using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameSystem.GameCore.Network
{
    public class MemberUtility : PacketUtility
    {
        public byte memberID { get; private set; }
        public bool isField { get; private set; }
        private MemberInfo memberInfo;
        private PacketUtility proxy;

        public MemberUtility(byte memberID, MemberInfo memberInfo)
        {
            this.memberID = memberID;
            this.memberInfo = memberInfo;
            isField = memberInfo.MemberType == MemberTypes.Field;

            Type memType = GetMemberType();
            proxy = GetUtil(memType);
        }

        public object Get(object target)
        {
            if (isField)
                return ((FieldInfo)memberInfo).GetValue(target);
            else
            {
                var property = (PropertyInfo)memberInfo;
                if (property.CanRead)
                    return property.GetValue(target);
            }
            return null;
        }

        public void Set(object target, object value)
        {
            if (isField)
                ((FieldInfo)memberInfo).SetValue(target, value);
            else
            {
                var property = (PropertyInfo)memberInfo;
                if (property.CanWrite)
                    property.SetValue(target, value);
            }
        }

        public static int Compare(MemberUtility x, MemberUtility y)
        {
            return x.memberID.CompareTo(y.memberID);
        }

        public override GSFPacket pack(object obj)
        {
            return proxy.pack(obj);
        }

        public override object unpack(GSFPacket packet)
        {
            return proxy.unpack(packet);
        }

        public override void unpack(ref object target, GSFPacket packet)
        {
            proxy.unpack(ref target, packet);
        }
        
        public Type GetMemberType()
        {
            Type memType;
            if (isField)
            {
                memType = ((FieldInfo)memberInfo).FieldType;
            }
            else
            {
                memType = ((PropertyInfo)memberInfo).PropertyType;
            }
            return memType;
        }
    }
}