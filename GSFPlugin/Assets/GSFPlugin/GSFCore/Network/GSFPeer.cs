using LiteNetLib;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    public class GSFPeer : IPeer
    {
        public delegate void OnReceiveHandler(object obj);
        public OnReceiveHandler OnReceiveEvent;

        NetPeer peer;
        ISerializer serializer;

        public GSFPeer(NetPeer peer, ISerializer serializer)
        {
            this.peer = peer;
            this.serializer = serializer;
        }

        public void Receive(object obj)
        {
            OnReceiveEvent.Invoke(obj);
        }

        public void Send(object obj, Reliability reliability)
        {
            GSFPacket packet = PacketUtility.Pack(obj);
            peer.Send(serializer.Serialize(packet), (DeliveryMethod)reliability);
        }
    }

    public class GSFPeerManager
    {
        ISerializer serializer;

        EventBasedNetListener listener;
        NetManager manager;

        Dictionary<NetPeer, GSFPeer> peerDict = new Dictionary<NetPeer, GSFPeer>();

        public GSFPeerManager(int port)
        {
            listener = new EventBasedNetListener();
            manager = new NetManager(listener);
            manager.Start(port);

            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if(peerDict.TryGetValue(peer, out GSFPeer gsfPeer))
            {
                int length = reader.AvailableBytes;
                byte[] dgram = new byte[length];
                reader.GetBytes(dgram, length);
                GSFPacket packet = (GSFPacket)serializer.Deserialize(dgram);
                gsfPeer.Receive(packet);
            }
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            NetPeer peer;
            if(true)
            {
                peer = request.AcceptIfKey("Key");
                GSFPeer gsfPeer = new GSFPeer(peer, serializer);
                peerDict.Add(peer, gsfPeer);
            }
            else
            {
                request.Reject();
            }
        }
    }
}