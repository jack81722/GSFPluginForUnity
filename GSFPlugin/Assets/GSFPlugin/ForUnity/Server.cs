using GameSystem.GameCore;
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
    /// <summary>
    /// Base server class using RUDP protocol
    /// </summary>
    public abstract class Server
    {
        private EventBasedNetListener listener;
        private NetManager netManager;

        private Task receiveTask;
        private CancellationTokenSource receiveTcs;

        #region Server properties
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
        #endregion

        /// <summary>
        /// Default group in server, it will service all peer
        /// </summary>
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

        #region Protected user-defined events
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

        protected virtual void OnServerClose() { }
        #endregion

        #region Private listener events
        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            RUDPPeer newPeer = new RUDPPeer(peer);
            peer.Tag = newPeer;
            Task.Run(() => group.JoinAsync(newPeer, null));
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
        #endregion

        #region Server main logic
        private async Task ReceiveLoop()
        {
            while (!receiveTcs.IsCancellationRequested)
            {
                netManager.PollEvents();
                HandleGroupJoinReqeust();
                await Task.Delay(15);
            }
        }

        private void Group_OnPeerJoinRequest(JoinGroupRequest request)
        {
            if (OnPeerConnect(request.Peer))
                request.Accept(null);
            else
                request.Reject("", null);
        }

        private void HandleGroupJoinReqeust()
        {
            while (group.GetQueueingCount() > 0)
                Group_OnPeerJoinRequest(group.DequeueJoinRequest());
        }
        #endregion

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
            Debug.Log("User-defined logic is closing ...");
            OnServerClose();            // close all user-defined logic
            Debug.Log("User-defined logic is closed.");
            receiveTcs.Cancel();        // stop receiving
            receiveTask.Wait();         // wait receiving task stop
            group.Close();              // close default group
            netManager.DisconnectAll(); 
            Debug.Log($"Server receive task : {receiveTask.Status}");
        }
    }
}