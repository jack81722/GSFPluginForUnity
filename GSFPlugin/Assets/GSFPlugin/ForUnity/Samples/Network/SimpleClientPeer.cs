using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;

public class SimpleClientPeer : ClientPeer
{
    private Dictionary<int, List<IPacketReceiver>> receiverDict;

    public SimpleClientPeer(ISerializer serializer) : base(serializer)
    {
        receiverDict = new Dictionary<int, List<IPacketReceiver>>();
    }

    public void AddReceiver(IPacketReceiver receiver)
    {
        if (receiverDict.TryGetValue(receiver.Code, out List<IPacketReceiver> receivers))
            receivers.Add(receiver);
        else
            receiverDict.Add(receiver.Code, new List<IPacketReceiver>() { receiver });
    }

    public void RemoveAction(IPacketReceiver receiver)
    {
        if (receiverDict.TryGetValue(receiver.Code, out List<IPacketReceiver> receivers))
            receivers.Remove(receiver);
    }

    public override void OnReceivePacket(object packet, Reliability reliability)
    {
        object[] packetData = (object[])packet;
        int code = (int)packetData[0];
        if (receiverDict.TryGetValue(code, out List<IPacketReceiver> receivers))
        {
            receivers.ForEach(receiver => receiver.Receive(packetData[1]));
        }
    }

}
