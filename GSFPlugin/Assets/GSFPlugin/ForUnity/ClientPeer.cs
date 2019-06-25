using GameSystem.GameCore.Network;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    public abstract class ClientPeer : IPeer
    {
        protected EventBasedNetListener listener;
        protected NetManager netManager;
        protected NetPeer peer;
        protected ISerializer serializer;

        public int Id { get { return peer.Id; } }

        public ClientPeer(ISerializer serializer) : base()
        {
            this.serializer = serializer;
            listener = new EventBasedNetListener();
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            netManager = new NetManager(listener);
            netManager.Start();
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte[] dgram = new byte[reader.AvailableBytes];
            reader.GetBytes(dgram, dgram.Length);
            object packet = serializer.Deserialize(dgram);
            OnReceivePacket(packet, (Reliability)deliveryMethod);
        }

        public void Connect(string ipAddr, int port, string connectKey)
        {
            UnityEngine.Debug.Log($"Connect to [{ipAddr}:{port}, \"{connectKey}\"]");
            peer = netManager.Connect(ipAddr, port, connectKey);
            peer.Tag = this;
            
            //UnityEngine.Debug.Log($"Connect result : {peer.ConnectionState}");
        }

        public abstract void OnReceivePacket(object packet, Reliability reliability);

        public void Send(object packet, Reliability reliability)
        {
            byte[] dgram = serializer.Serialize(packet);
            peer.Send(dgram, (DeliveryMethod)reliability);
        }

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }

        public void Poll()
        {
            netManager.PollEvents();
        }

        public void Disconnect()
        {
            netManager.DisconnectAll();
        }
    }
}