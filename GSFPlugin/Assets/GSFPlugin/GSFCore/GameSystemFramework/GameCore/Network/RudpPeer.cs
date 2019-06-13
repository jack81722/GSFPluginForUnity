using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

public class RudpPeer : PeerProxy
{
    private EventBasedNetListener listener;
    private NetManager netMgr;

    public RudpPeer(ISerializer serializer) : base(serializer)
    {
        listener = new EventBasedNetListener();
        netMgr = new NetManager(listener);
    }

    public void Connect(string ip, int port, string key)
    {
        netMgr.Start();
        netMgr.Connect(ip, port, key);
        listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent; ;
    }

    private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        int length = reader.AvailableBytes;
        byte[] dgram = new byte[length];
        reader.GetBytes(dgram, length);
        OnReceiveEvent.Invoke(dgram);
        reader.Recycle();
    }

    public override void Send(object obj)
    {
        byte[] dgram = serializer.Serialize(obj);
        NetDataWriter writer = new NetDataWriter();
        writer.Put(dgram);
        
    }
}
