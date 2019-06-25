﻿using LiteNetLib;
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

        /// <summary>
        /// Peers in group
        /// </summary>
        protected Dictionary<int, IPeer> peers;

        /// <summary>
        /// Queue of requests want to join
        /// </summary>
        private List<JoinGroupRequest> queueing;
        /// <summary>
        /// List of requests are handling but not respond yet
        /// </summary>
        private List<JoinGroupRequest> handling;

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
            handling = new List<JoinGroupRequest>();
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
                CloseSafely();
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

        #region Join/Exit request methods
        public async Task<JoinGroupResponse> JoinAsync(IPeer peer, object arg)
        {
            // check if group is closed
            if (isClosed)
                return new JoinGroupResponse(Id, JoinGroupResponse.ResultType.Cancelled, string.Format("Group is closed."));

            // check if peer has joined
            if (peers.ContainsKey(peer.Id))
                return new JoinGroupResponse(Id, JoinGroupResponse.ResultType.HasJoined, string.Format("Has joined in group"));

            // check if peer in queue
            if (queueing.Exists(req => req.Peer.Id == peer.Id))
                return new JoinGroupResponse(Id, JoinGroupResponse.ResultType.InQueue, string.Format("Join request is in queue."));

            // check if peer is handling
            if(handling.Exists(req => req.Peer.Id == peer.Id))
                return new JoinGroupResponse(Id, JoinGroupResponse.ResultType.Handling, string.Format("Join request is handling."));

            // add request into queue and waiting result
            JoinGroupRequest request = new JoinGroupRequest(Id, peer, arg);
            queueing.Add(request);
            JoinGroupResponse result = await request.Task;
            handling.Remove(request);       // remove request from handling (because finished)
            if (result.type == JoinGroupResponse.ResultType.Accepted)
            {
                lock (peers) peers.Add(peer.Id, peer);
            }
            return result;
        }

        public async Task<bool> ExitAsync(IPeer peer, object arg)
        {
            ExitGroupRequest request = new ExitGroupRequest(Id, peer, arg);
            return false;
        }

        public int GetQueueingCount()
        {
            return queueing.Count;
        }

        public int GetHandlingCount()
        {
            return handling.Count;
        }

        public JoinGroupRequest DequeueJoinRequest()
        {
            lock (queueing)
            {
                if (queueing.Count < 0)
                    throw new InvalidOperationException("No queueing request.");
                JoinGroupRequest request = queueing[0];
                queueing.RemoveAt(0);
                // add request into handling
                handling.Add(request);
                return request;
            }
        }

        public void ExitAll(string msg, object arg)
        {
            int queueingCount = queueing.Count;
            for(int i = 0; i< queueing.Count; i++)
            {
                queueing[i].Cancel(msg, arg);
            }
            int handlingCount = handling.Count;
            for(int i = 0; i < handling.Count; i++)
            {
                handling[i].Cancel(msg, arg);
            }
            handling.Clear();
            // remove group from peer
            int inGroupCount = peers.Count;
            peers.Clear();
            UnityEngine.Debug.Log($"Group[{Id}] exit all peers, queueing cancelled : {queueingCount}, handling cancelled : {handlingCount}, in group : {inGroupCount}");
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
                CloseSafely();
        }

        private void CloseSafely()
        {
            ExitAll("Group is closed.", null);
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