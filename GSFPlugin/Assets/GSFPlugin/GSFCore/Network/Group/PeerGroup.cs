using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    /// <summary>
    /// Group of stack packets and send 
    /// </summary>
    public class PeerGroup
    {
        private static IdentityPool idPool = new IdentityPool();
        public int Id { get; private set; }
        protected ISerializer serializer;
        protected Dictionary<int, IPeer> peers;
        private List<JoinGroupRequest> queueing;

        protected PacketEventPool eventPool;
        private bool isPolling;
        private bool isClosed = false;

        public OnReceivePacketHandler OnGroupReceiveEvent;

        public PeerGroup(ISerializer serializer)
        {
            Id = idPool.NewID();
            this.serializer = serializer;
            peers = new Dictionary<int, IPeer>();
            eventPool = new PacketEventPool();
            queueing = new List<JoinGroupRequest>();
        }

        public void Poll()
        {
            PacketEvent[] events = eventPool.DequeueAll();
            isPolling = true;
            for (int i = 0; i < events.Length; i++)
            {
                IPeer peer = events[i].GetPeer();
                object data = events[i].GetData();
                Reliability reliability = events[i].GetReliability();
                OnGroupReceiveEvent.Invoke(peer, data, reliability);    // delegate method
            }
            isPolling = false;
            if (isClosed)
                SafeClose();
        }

        #region Send methods
        public void Broadcast(object obj, Reliability reliability)
        {
            byte[] bytes = serializer.Serialize(obj);
            foreach (var peer in peers.Values)
            {
                peer.Send(bytes, reliability);
            }
        }

        public async Task BroadcastAsync(object obj, Reliability reliability)
        {
            Task task = Task.Run(() => Broadcast(obj, reliability));
            await task;
        }

        public void Send(int peerID, object obj, Reliability reliability)
        {
            if (peers.TryGetValue(peerID, out IPeer peer))
            {
                byte[] bytes = serializer.Serialize(obj);
                peer.Send(bytes, reliability);
            }
        }

        public async Task SendAsync(int peerId, object obj, Reliability reliability)
        {
            Task task = Task.Run(() => Send(peerId, obj, reliability));
            await task;
        }
        #endregion

        public void AddEvent(IPeer peer, object data, Reliability reliability)
        {
            if(!isClosed)
                eventPool.Enqueue(peer, data, reliability);
        }

        #region Join/Exit methods
        public async Task<JoinGroupResponse> JoinAsync(IPeer peer, object arg)
        {
            // check if group is closed
            if (isClosed)
                return new JoinGroupResponse(Id, JoinGroupResponse.ResultType.Cancelled, string.Format("Group is closed."));

            // check if peer has joined
            if (peers.ContainsKey(peer.Id))
                return new JoinGroupResponse(Id, JoinGroupResponse.ResultType.HasJoined, string.Format("Peer[{0}] has joined group.", peer.Id));

            // add request into queue and waiting result
            JoinGroupRequest request = new JoinGroupRequest(Id, peer, arg);
            queueing.Add(request);
            JoinGroupResponse result = await request.Task;
            queueing.Remove(request);
            if (result.type == JoinGroupResponse.ResultType.Accepted)
            {
                lock (peers) peers.Add(peer.Id, peer);
            }
            return result;
        }

        public async Task<bool> ExitAsync(IPeer peer, object arg)
        {
            return false;
        }

        public int GetQueueingCount()
        {
            return queueing.Count;
        }

        public JoinGroupRequest DequeueJoinRequest()
        {
            lock (queueing)
            {
                JoinGroupRequest request = queueing[0];
                queueing.RemoveAt(0);
                return request;
            }
        }
        #endregion

        public IPeer GetPeer(int peerID)
        {
            if (peers.TryGetValue(peerID, out IPeer peer))
            {
                return peer;
            }
            throw new InvalidOperationException("Cannot find peer.");
        }

        public bool TryGetPeer(int peerID, out IPeer peer)
        {
            return peers.TryGetValue(peerID, out peer);
        }

        public List<IPeer> GetPeerList()
        {
            return new List<IPeer>(peers.Values);
        }

        
        public void Close()
        {
            isClosed = true;
            if (!isPolling)
                SafeClose();
        }

        private void SafeClose()
        {
            lock (peers) peers.Clear();
            lock (queueing)
            {
                foreach (var req in queueing)
                {
                    req.Cancel("Group is closed.");
                }
                queueing.Clear();
            }
            eventPool.Clear();
        }

    }

    public class IdentityPool
    {
        private int serialId;
        private Queue<int> idPool;

        public IdentityPool()
        {
            serialId = 0;
            idPool = new Queue<int>();
        }

        public int NewID()
        {
            lock (idPool)
            {
                if (idPool.Count > 0)
                    return idPool.Dequeue();
            }
            return serialId++;
        }

        public void RecycleID(int id)
        {
            lock (idPool)
            {
                if (!idPool.Contains(id))
                    idPool.Enqueue(id);
            }
        }
    }

    public enum QueueStatus
    {
        Smooth,     // means amount of queuing and in group are not over maximum
        Crowded,    // means amount of queuing and in group are over maximum
        Full        // means group is full
    }
}