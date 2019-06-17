using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    /// <summary>
    /// Group of stack packets and send 
    /// </summary>
    public class PeerGroup
    {
        protected ISerializer serializer;
        protected Dictionary<int, IPeer> peers;

        protected PacketEventPool eventPool;

        public delegate void OnReceiveHandler(IPeer peer, byte[] dgram, Reliability reliability);
        public OnReceiveHandler OnReceiveEvent;

        public PeerGroup(ISerializer serializer)
        {
            this.serializer = serializer;
            peers = new Dictionary<int, IPeer>();
            eventPool = new PacketEventPool();
        }

        public void AddPeer(IPeer peer)
        {
            if (!peers.ContainsKey(peer.Id))
            {
                peers.Add(peer.Id, peer);
                UnityEngine.Debug.Log("Add peer success");
            }
        }

        public IPeer GetPeer(int peerID)
        {
            if(peers.TryGetValue(peerID, out IPeer peer))
            {
                return peer;
            }
            throw new InvalidOperationException("Cannot find peer.");
        }

        public bool TryGetPeer(int peerID, IPeer peer)
        {
            return peers.TryGetValue(peerID, out peer);
        }

        public bool RemovePeer(int peerID)
        {
            return peers.Remove(peerID);
        }

        public void Poll()
        {
            PacketEvent[] events = eventPool.DequeueAll();
            for (int i = 0; i < events.Length; i++)
            {
                IPeer peer = events[i].GetPeer();
                byte[] data = events[i].GetData();
                Reliability reliability = events[i].GetReliability();
                OnReceiveEvent(peer, data, reliability);
            }
        }

        public void Broadcast(object obj, Reliability reliability)
        {
            byte[] bytes = serializer.Serialize(obj);
            foreach (var peer in peers.Values)
            {
                peer.Send(bytes, reliability);
            }
        }

        public void Send(int peerID, object obj, Reliability reliability)
        {
            if (peers.TryGetValue(peerID, out IPeer peer))
            {
                byte[] bytes = serializer.Serialize(obj);
                peer.Send(bytes, reliability);
            }

        }

        public void AddEvent(IPeer peer, byte[] dgram, Reliability reliability)
        {
            eventPool.Enqueue(peer, dgram, reliability);
        }
    }

    public class RUDPPeer : IPeer
    {
        NetPeer peer;

        public RUDPPeer(NetPeer peer)
        {
            this.peer = peer;
            events = new Queue<PacketEvent>();
        }

        public int Id { get { return peer.Id; } }

        public OnReceiveHandler OnRecvEvent { get; set; }
        public Queue<PacketEvent> events { get; set; }

        public void Disconnect()
        {
            peer.Disconnect();
        }

        public void Poll()
        {
            lock (events)
            {
                PacketEvent[] es = events.ToArray();
                events.Clear();
                if (OnRecvEvent != null)
                {
                    for (int i = 0; i < es.Length; i++)
                    {
                        OnRecvEvent.Invoke(es[i].GetData(), es[i].GetReliability());
                    }
                }
            }
        }

        public void Recv(byte[] bytes, Reliability reliability)
        {
            lock (events)
            {
                PacketEvent e = new PacketEvent(this, bytes, reliability);
                events.Enqueue(e);
            }
        }

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }
    }

    public class PacketEventPool
    {
        private Queue<PacketEvent> events;

        public PacketEventPool()
        {
            events = new Queue<PacketEvent>();
        }

        public void Enqueue(IPeer peer, byte[] dgram, Reliability reliability)
        {
            events.Enqueue(new PacketEvent(peer, dgram, reliability));
        }

        public PacketEvent Dequeue()
        {
            return events.Dequeue();
        }

        public PacketEvent[] DequeueAll()
        {
            PacketEvent[] all;
            lock (events)
                all = events.ToArray();
            events.Clear();
            return all;
        }
    }
 
    public class PacketEvent
    {
        IPeer peer;
        byte[] dgram;
        Reliability reliability;

        public PacketEvent(IPeer peer, byte[] dgram, Reliability reliability)
        {
            this.peer = peer;
            this.dgram = dgram;
            this.reliability = reliability;
        }

        public IPeer GetPeer()
        {
            return peer;
        }

        public byte[] GetData()
        {
            return dgram;
        }

        public Reliability GetReliability()
        {
            return reliability;
        }
    }

}