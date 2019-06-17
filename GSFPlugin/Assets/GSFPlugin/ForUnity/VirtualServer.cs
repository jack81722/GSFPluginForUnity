using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class VirtualServer
    {
        public int MaxPeers = 10;
        public string ConnectKey = "Test";

        int port = 8888;

        private bool isReceiving;
        Task recvTask;

        PeerManager peerManager;

        EventBasedNetListener listener;
        NetManager serverNetManager;
        PeerGroup peerGroups;

        Game game;

        ISerializer serializer;
        IDebugger debugger;

        public VirtualServer(IDebugger debugger)
        {
            peerGroups = new PeerGroup(new FormmaterSerializer());
            this.debugger = debugger;
            peerManager = new PeerManager();
            listener = new EventBasedNetListener();
            serverNetManager = new NetManager(listener);
        }

        public Game CreateGame(IPhysicEngineFactory phyEnginFactory)
        {
            game = new Game(0, phyEnginFactory, debugger);
            game.Initialize();
            return game;
        }

        public void StartGame()
        {
            game.Start();
        }

        public void Start(int port)
        {
            debugger.Log("Start Server.");
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            serverNetManager.Start(port);
            recvTask = Task.Factory.StartNew(KeepReceive);

            CreateGame(new BulletEngine.BulletEngineFactory());
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            IPeer newPeer = new RUDPPeer(peer);
            PeerState state = new PeerState(newPeer);
            peerManager.AddPeerState(state);
            UnityEngine.Debug.Log("Set Group");
            state.SetGroup(game.peerGroup);
        }

        private async void KeepReceive()
        {
            isReceiving = true;
            while (isReceiving)
            {
                serverNetManager.PollEvents();
                await Task.Delay(15);
            }
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            debugger.Log("Receive Event.");
            byte[] dgram = new byte[reader.AvailableBytes];
            reader.GetBytes(dgram, dgram.Length);
            reader.Recycle();
            PeerState state = peerManager.GetPeerState(peer.Id);
            state.TransEvent(dgram, (Reliability)deliveryMethod);
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            debugger.Log("Connection Request Event.");
            if (serverNetManager.PeersCount < MaxPeers)
                request.AcceptIfKey(ConnectKey);
            else
                request.Reject();
        }

        public void Close()
        {
            if(game != null)
                game.Close();
            isReceiving = false;
            recvTask.Wait();
            serverNetManager.DisconnectAll();
        }
    }

    public class PeerManager
    {
        Dictionary<int, PeerState> peerStates;
        
        public PeerManager()
        {
            peerStates = new Dictionary<int, PeerState>();
        }

        public void AddPeerState(PeerState state)
        {
            if (!peerStates.ContainsKey(state.GetPeerID()))
            {
                peerStates.Add(state.GetPeerID(), state);
            }
        }

        public PeerState GetPeerState(int peerID)
        {
            return peerStates[peerID];
        }
    }

    public class PeerState
    {
        IPeer peer;
        PeerGroup group;

        public PeerState(IPeer peer)
        {
            this.peer = peer;
            group = null;
        }

        public int GetPeerID()
        {
            return peer.Id;
        }

        public IPeer GetPeer()
        {
            return peer;
        }

        public void SetGroup(PeerGroup group)
        {
            if (group == null)
                return;
            // if peer has have group, exit current group
            if(this.group != null)
            {
                this.group.RemovePeer(peer.Id);
            }
            if (this.group != group)
            {
                // set new group and join
                this.group = group;
                group.AddPeer(peer);
            }
        }

        public PeerGroup GetGroup()
        {
            return group;
        }

        public void TransEvent(byte[] dgram, Reliability reliability)
        {
            if(group != null)
                group.AddEvent(peer, dgram, reliability);
            else
            {
                peer.Recv(dgram, reliability);
            }
        }
    }
}