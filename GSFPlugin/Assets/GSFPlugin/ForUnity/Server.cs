﻿using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameSystem.GameCore.Network
{
    public abstract class Server
    {
        private EventBasedNetListener listener;
        private NetManager netManager;

        private Task receiveTask;
        private CancellationTokenSource receiveTcs;

        /// <summary>
        /// Boolean of serverr is running
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Server port
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Maximum peer number to accept
        /// </summary>
        private int maxPeers = 1000;
        public int MaxPeers
        {
            get { return maxPeers; }
            set
            {
                if (Running)
                    throw new InvalidOperationException("Cannot change max peer while running.");
                maxPeers = value;
            }
        }

        /// <summary>
        /// Connect key to accept
        /// </summary>
        private string connectKey = "";
        public string ConnectKey
        {
            get { return connectKey; }
            set
            {
                if (Running)
                    throw new InvalidOperationException("Cannot change connect key while running.");
                connectKey = value;
            }
        }

        protected PeerGroup group;
        protected ISerializer serializer;

        public Server(ISerializer serializer)
        {
            this.serializer = serializer;
            group = new PeerGroup(serializer);
            listener = new EventBasedNetListener();
            netManager = new NetManager(listener);
            group.OnGroupReceiveEvent += OnReceivePacket;
            receiveTcs = new CancellationTokenSource();
        }

        protected virtual bool OnPeerConnect(IPeer peer)
        {
            return true;
        }

        protected virtual void OnPeerJoinResponse(IPeer peer, JoinGroupResponse response)
        {
            byte[] dgram = serializer.Serialize(response);
            peer.Send(dgram, Reliability.ReliableOrder);
        }

        protected abstract void OnReceivePacket(IPeer peer, object packet, Reliability reliability);

        public void Start(int port)
        {
            Running = true;
            // initialize listener events
            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            // start listen
            Port = port;
            netManager.Start(port);
            // start receive loop
            receiveTask = Task.Run(ReceiveLoop, receiveTcs.Token);
            Debug.Log($"Start Server[Port:{port}]");
        }

        private async Task ReceiveLoop()
        {
            while (!receiveTcs.IsCancellationRequested)
            {
                netManager.PollEvents();
                HandleGroupJoinReqeust();
                await Task.Delay(15);
            }
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            RUDPPeer newPeer = new RUDPPeer(peer);
            peer.Tag = newPeer;
            Task.Run(() => group.JoinAsync(newPeer, null));
        }

        private JoinGroupResponse Group_OnPeerJoinRequest(JoinGroupRequest request)
        {
            JoinGroupResponse response;
            if (OnPeerConnect(request.Peer))
                response = new JoinGroupResponse(group.Id, JoinGroupResponse.ResultType.Accepted, "");
            else
                response = new JoinGroupResponse(group.Id, JoinGroupResponse.ResultType.Rejected, "");
            Debug.Log($"Peer[{request.Peer.Id}] is {response.type}");
            return response;
        }

        private void HandleGroupJoinReqeust()
        {
            while (group.GetQueueingCount() > 0)
                Group_OnPeerJoinRequest(group.DequeueJoinRequest());
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte[] dgram = new byte[reader.AvailableBytes];
            reader.GetBytes(dgram, dgram.Length);
            IPeer p = (IPeer)peer.Tag;
            OnReceivePacket(p, serializer.Deserialize(dgram), (Reliability)deliveryMethod);
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            if (group.GetPeerList().Count < MaxPeers)
                request.AcceptIfKey(ConnectKey);
            else
                request.Reject();
        }

        public IPeer GetPeer(int peerId)
        {
            return group.GetPeer(peerId);
        }

        public bool TryGetPeer(int peerId, out IPeer peer)
        {
            return group.TryGetPeer(peerId, out peer);
        }

        public void Close()
        {
            Running = false;
            receiveTcs.Cancel();
            receiveTask.Wait();
            group.Close();
            netManager.DisconnectAll();
            Debug.Log($"Server receive task : {receiveTask.Status}");
        }
    }
}