using GameSystem.GameCore.Network;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;

public class ClientPeer : Peer
{
    public EventBasedNetListener listener;
    public NetManager netManager;

    public ClientPeer() : base()
    {
        listener = new EventBasedNetListener();
        listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
        netManager = new NetManager(listener);
        netManager.Start();
    }

    private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte[] dgram = new byte[reader.AvailableBytes];
        reader.GetBytes(dgram, dgram.Length);
        OnPeerReceiveEvent.Invoke(this, dgram, (Reliability)deliveryMethod);
    }

    public void Connect(string ipAddr, int port, string connectKey)
    {
        UnityEngine.Debug.Log($"Connect to [{ipAddr}:{port}, \"{connectKey}\"]");
        NetPeer p = netManager.Connect(ipAddr, port, connectKey);
        p.Tag = this;
    }

    public override void Disconnect()
    {
        netManager.DisconnectAll();
    }

    public override void Send(byte[] bytes, Reliability reliability)
    {
        netManager.SendToAll(bytes, (DeliveryMethod)reliability);
    }

    public override void Poll()
    {
        netManager.PollEvents();
    }
}
