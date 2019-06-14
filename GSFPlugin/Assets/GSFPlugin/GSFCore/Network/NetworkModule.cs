using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    public class PeerGroup
    {
        protected ISerializer serializer;
        protected Dictionary<int, IPeer> peers;

        public PeerGroup(ISerializer serializer)
        {
            this.serializer = serializer;
            peers = new Dictionary<int, IPeer>();
        }

        public void AddPeer(IPeer peer)
        {   
            if(!peers.ContainsKey(peer.Id))
                peers.Add(peer.Id, peer);
        }

        public IPeer GetPeer(int peerID)
        {
            if(peers.TryGetValue(peerID, out IPeer peer))
            {
                return peer;
            }
            throw new InvalidOperationException("Cannot find peer.");
        }

        public void RemovePeer(int peerID)
        {
            peers.Remove(peerID);
        }

        public virtual void Poll()
        {
            foreach (var peer in peers.Values)
            {
                peer.Poll();
            }
        }

        public virtual void Broadcast(object obj, Reliability reliability)
        {
            byte[] bytes = serializer.Serialize(obj);
            foreach (var peer in peers.Values)
            {
                peer.Send(bytes, reliability);
            }
        }

        public virtual void Send(int peerID, object obj, Reliability reliability)
        {
            if (peers.TryGetValue(peerID, out IPeer peer))
            {
                byte[] bytes = serializer.Serialize(obj);
                peer.Send(bytes, reliability);
            }

        }
    }

    public class RUDPPeer : IPeer
    {
        NetPeer peer;
        PacketEventPool eventPool;
        public delegate void OnReceiveHandler(byte[] dgram);
        public OnReceiveHandler OnReceiveEvent;

        public RUDPPeer(NetPeer peer)
        {
            this.peer = peer;
        }

        public int Id { get { return peer.Id; } }

        public void AddPacketEvent(byte[] dgram)
        {
            eventPool.Enqueue(dgram);
        }

        public void Poll()
        {
            eventPool.Invoke(OnReceiveEvent);
        }

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }
    }

    public class PacketEventPool
    {
        private Queue<byte[]> packets;

        public PacketEventPool()
        {
            packets = new Queue<byte[]>();
        }

        public void Enqueue(byte[] dgram)
        {
            packets.Enqueue(dgram);
        }

        public void Invoke(RUDPPeer.OnReceiveHandler d)
        {
            while(packets.Count > 0)
                d.Invoke(packets.Dequeue());
            packets.Clear();
        }
    }
 
}