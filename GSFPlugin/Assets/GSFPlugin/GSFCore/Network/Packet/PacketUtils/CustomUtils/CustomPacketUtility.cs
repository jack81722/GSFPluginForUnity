using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public abstract class CustomPacketUtility : PacketUtility
{
    public override void unpack(ref object target, GSFPacket packet)
    {
        target = unpack(packet);
    }
}

public abstract class CustomPacketUtility<T> : CustomPacketUtility
{
    public CustomPacketUtility()
    {   
        CustomPacketUtilAttribute attr = GetType().GetCustomAttribute<CustomPacketUtilAttribute>();
        classType = typeof(T);
        classID = attr.classID;
        isPackable = true;
    }
    
    public sealed override GSFPacket pack(object obj)
    {
        return Pack((T)obj);
    }
    public abstract GSFPacket Pack(T obj);

    public sealed override object unpack(GSFPacket packet)
    {
        return Unpack(packet);
    }
    public abstract T Unpack(GSFPacket packet);

    public sealed override void unpack(ref object target, GSFPacket packet)
    {
        T temp = (T)target;
        Unpack(ref temp, packet);
    }
    public virtual void Unpack(ref T target, GSFPacket packet)
    {
        target = Unpack(packet);
    }

}
