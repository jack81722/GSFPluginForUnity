using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameSystem.GameCore
{
    public class GameSourceAdapter
    {
        public uint SID { get { return source.SID; } }
        private GameSource source;
        private List<Member> members;
        public int Count { get { return members.Count; } }

        public bool isGameObject { get; private set; }
        private int[] comIDs;

        public bool isComponent { get; private set; }

        public GameSourceAdapter(GameSource source)
        {   
            this.source = source;
            isGameObject = source.GetType() == typeof(GameObject);
            isComponent = source.GetType().IsSubclassOf(typeof(Component));
            InitMembers();
        }

        private void InitMembers()
        {
            members = new List<Member>();
            MemberInfo[] memberInfos = source.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            for(int i = 0; i < memberInfos.Length; i++)
            {
                if (memberInfos[i].MemberType == MemberTypes.Field || memberInfos[i].MemberType == MemberTypes.Property)
                {
                    members.Add(new Member(source, memberInfos[i]));
                }
            }
        }

        public Member GetMember(string name)
        {
            return GetMembers().Find(m => m.MemberName == name);
        }

        public List<Member> GetMembers()
        {
            return members;
        }

        public static int CompareSID(GameSourceAdapter adapter1, GameSourceAdapter adapter2)
        {
            return adapter1.SID.CompareTo(adapter2.SID);
        }

        public void Refresh()
        {
            for(int i = 0; i < members.Count; i++)
            {
                members[i].Refresh();
            }
        }
    }

    public class Member
    {
        public bool edited { get; private set; }
        private object tempValue;

        private Type valueType;
        private object source;
        public string MemberName
        {
            get { return info.Name; }
        }
        private MemberTypes memberType;
        private MemberInfo info;

        public Member(object source, MemberInfo info)
        {
            this.source = source;
            valueType = source.GetType();
            this.info = info;
        }

        public bool boolValue
        {
            get
            {
                if (source != null && info != null)
                    return GetValue<bool>(info, source);
                throw new InvalidOperationException("Type is not boolean.");
            }
            set
            {
                if (this.source != null && info != null && valueType != value.GetType())
                {
                    edited = true;
                    tempValue = value;
                }
                throw new InvalidOperationException("Type is not boolean.");
            }
        }

        public string stringValue
        {
            get
            {
                if (source != null && info != null)
                    return GetValue<string>(info, source);
                throw new InvalidOperationException("Type is not string.");
            }
            set
            {
                if (source != null && info != null && valueType != value.GetType())
                {
                    edited = true;
                    tempValue = value;
                }
                throw new InvalidOperationException("Type is not string.");
            }
        }

        public object objectValue
        {
            get
            {
                if (source != null && info != null)
                    return GetValue(info, source);
                throw new InvalidOperationException("Type is not string.");
            }
            set
            {
                if (source != null && info != null && valueType != value.GetType())
                {
                    edited = true;
                    tempValue = value;
                }
                throw new InvalidOperationException("Type is not string.");
            }
        }

        private object GetValue(MemberInfo info, object source)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)info).GetValue(source);
                case MemberTypes.Property:
                    return ((PropertyInfo)info).GetValue(source);
            }
            return default;
        }

        private T GetValue<T>(MemberInfo info, object source)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    return (T)((FieldInfo)info).GetValue(source);
                case MemberTypes.Property:
                    return (T)((PropertyInfo)info).GetValue(source);
            }
            return default;
        }

        private void SetValue(MemberInfo info, object source, object value)
        {
            switch (info.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)info).SetValue(source, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)info).SetValue(source, value);
                    break;
            }
        }

        public void Refresh()
        {
            if (edited)
                SetValue(info, source, tempValue);
        }
    }
}
