using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GameSystem.GameCore.Network
{
    public abstract class PacketUtility
    {
        #region Static packet attribute type
        protected static Type packableAttr = typeof(PackableAttribute);
        protected static Type unpackableAttr = typeof(UnpackableAttribute);
        protected static Type packetMemAttr = typeof(PacketMemberAttribute);
        protected static Type customUtilAttr = typeof(CustomPacketUtilAttribute);
        #endregion

        #region Packet parameters
        public short classID { get; protected set; }
        public Type classType { get; protected set; }

        public bool isPackable { get; protected set; }
        #endregion

        #region Abstract pack/unpack method instances
        public abstract GSFPacket pack(object obj);

        public abstract object unpack(GSFPacket packet);

        public virtual T unpack<T>(GSFPacket packet)
        {
            return (T)unpack(packet);
        }

        public virtual object unpack(GSFPacket packet, Type type)
        {
            PacketUtility util = GetUtil(type);
            return util.unpack(packet);
        }

        public abstract void unpack(ref object target, GSFPacket packet);

        public virtual void unpack<T>(ref T target, GSFPacket packet)
        {
            object temp = target;
            unpack(ref temp, packet);
        }

        public TypeInfo PackType(Type type)
        {
            if (type.IsGenericType)
            {
                Type baseType = type.GetGenericTypeDefinition();
                short classID = GetClassID(baseType);
                Type[] genericArgs = type.GetGenericArguments();
                TypeInfo[] args = new TypeInfo[genericArgs.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    args[i] = PackType(genericArgs[i]);
                }
                return new TypeInfo() { BaseType = classID, GenericArgs = args };
            }
            else if (type.IsArray)
            {                   
                short classID = -14;
                TypeInfo[] arg = new TypeInfo[] { PackType(type.GetElementType()) };
                return new TypeInfo() { BaseType = classID, GenericArgs = arg };
            }
            else
            {
                return new TypeInfo() { BaseType = GetUtil(type).classID, GenericArgs = new TypeInfo[0] };
            }
        }

        public Type UnpackType(TypeInfo typeArgs)
        {
            if(typeArgs.BaseType == -14)        // Array
            {
                TypeInfo[] args = typeArgs.GenericArgs;
                Type elementType = UnpackType(args[0]);
                return elementType.MakeArrayType();
            }
            Type baseType = GetClass(typeArgs.BaseType);
            if(typeArgs.GenericArgs != null && typeArgs.GenericArgs.Length > 0)     // Generic type
            {
                Type[] genericArgs = new Type[typeArgs.GenericArgs.Length];
                for(int i = 0; i < genericArgs.Length; i++)
                {
                    genericArgs[i] = UnpackType(typeArgs.GenericArgs[i]);
                }
                return baseType.MakeGenericType(genericArgs);
            }
            return baseType;
        }
        #endregion

        #region Static pack/unpack method
        /// <summary>
        /// Pack object with unknown type
        /// </summary>
        public static GSFPacket Pack(object obj)
        {
            Type type = obj.GetType();
            PacketUtility packer = GetUtil(type);
            return packer.pack(obj);
        }

        /// <summary>
        /// Pack object with assigned type
        /// </summary>
        public static GSFPacket Pack<T>(T obj)
        {
            Type type = typeof(T);
            PacketUtility packer = GetUtil(type);
            return packer.pack(obj);
        }

        /// <summary>
        /// Unpack and override target from packet
        /// </summary>
        /// <typeparam name="T">unpackable type</typeparam>
        public static T Unpack<T>(GSFPacket packet)
        {
            Type type = typeof(T);
            PacketUtility packer = GetUtil(type);
            return packer.unpack<T>(packet);
        }

        /// <summary>
        /// Unpack and override target from packet
        /// </summary>
        /// <typeparam name="T">unpackable type</typeparam>
        /// <param name="target">unpackable target</param>
        public static void Unpack<T>(ref T target, GSFPacket packet)
        {
            Type type = typeof(T);
            PacketUtility packer = GetUtil(type);
            packer.unpack(ref target, packet);
        }
        #endregion


        #region UtilsPool
        static Dictionary<short, PacketUtility> defaults;
        static Dictionary<Type, PacketUtility> others;

        protected static void CacheUtil(PacketUtility util, bool isDefault)
        {
            if (defaults == null && others == null)
                initUtilities();
            if (isDefault)
                defaults.Add(util.classID, util);
            if(util.classType != null)
                others.Add(util.classType, util);
        }

        protected static PacketUtility GetUtil(Type type)
        {
            if (defaults == null && others == null)
                initUtilities();
            if (type.IsArray)
                return GetUtil(-14);
            type = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (others.TryGetValue(type, out PacketUtility util))
                return util;
            throw new InvalidOperationException("Cannot find compatible packet utility for this type.");
        }

        protected static PacketUtility GetUtil(short classID)
        {
            if (defaults == null && others == null)
                initUtilities();
            if (defaults.TryGetValue(classID, out PacketUtility util))
                return util;
            throw new InvalidOperationException("Cannot find class identity of packet utility.");
        }

        protected static short GetClassID(Type type)
        {
            if (defaults == null && others == null)
                initUtilities();
            if (others.TryGetValue(type, out PacketUtility util))
                return util.classID;
            throw new InvalidOperationException("Cannot find compatible packet utility for this type.");
        }

        protected static Type GetClass(short classID)
        {
            if (defaults == null && others == null)
                initUtilities();
            if (defaults.TryGetValue(classID, out PacketUtility util))
                return util.classType;
            throw new InvalidOperationException("Cannot find class identity of class.");
        }

        protected static void initUtilities()
        {
            defaults = new Dictionary<short, PacketUtility>();
            others = new Dictionary<Type, PacketUtility>();
            
            Type[] types = Assembly.GetAssembly(typeof(PacketUtility)).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {   
                if (types[i].IsDefined(packableAttr))
                {
                    // add all utilities defined packable/unpackable attribute
                    PacketUtility util;
                    if (types[i].ContainsGenericParameters)
                        util = new GenericPacketUtililty(types[i]);
                    else
                        util = new DefaultPacketUtility(types[i]);
                    CacheUtil(util, true);
                }
                else if (types[i].IsDefined(customUtilAttr))
                {  
                    var attr = types[i].GetCustomAttribute<CustomPacketUtilAttribute>();
                    var util = (PacketUtility)Activator.CreateInstance(types[i]);
                    CacheUtil(util, attr.isDefault);
                }
                else if (types[i].IsDefined(unpackableAttr))
                {
                    PacketUtility packer = new DefaultPacketUtility(types[i]);
                    CacheUtil(packer, false);
                }
            }
        }
        #endregion
    }
}
