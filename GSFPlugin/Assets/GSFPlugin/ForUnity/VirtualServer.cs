using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using LiteNetLib;
using System;
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

        EventBasedNetListener listener;
        NetManager serverNetManager;

        Game game;
        Lobby lobby;

        ISerializer serializer;
        IDebugger debugger;

        public VirtualServer(IDebugger debugger)
        {
            serializer = new FormmaterSerializer();
            this.debugger = debugger;
            listener = new EventBasedNetListener();
            serverNetManager = new NetManager(listener);
            lobby = new Lobby(serializer, debugger);
            lobby.Start();
        }

        public Game CreateGame()
        {
            game = new Game(new BulletEngine.BulletPhysicEngine(debugger), debugger);
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
            recvTask = Task.Run(KeepReceive);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            debugger.Log("Peer connected.");
            Peer newPeer = new RUDPPeer(peer);
            peer.Tag = newPeer;

            Task.Run(() => lobby.Join(newPeer, null));
            //Game game = CreateGame();
            //Task initGameTask = Task.Run(game.Initialize);
            //Task.Run(() => game.peerGroup.JoinAsync(newPeer, null));
            //initGameTask.Wait();
        }

        private async Task KeepReceive()
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
            byte[] dgram = new byte[reader.AvailableBytes];
            reader.GetBytes(dgram, dgram.Length);
            reader.Recycle();
            Peer p = (Peer)peer.Tag;
            GroupedPacket packet = serializer.Deserialize<GroupedPacket>(dgram);
            if (PeerGroupManager.TryGetGroup(packet.groupId, out PeerGroup group))
            {
                group.AddEvent(p, packet.data, (Reliability)deliveryMethod);
            }
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
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
            lobby.Close();
        }

    }

    [Serializable]
    public class GroupedPacket
    {
        public int groupId;
        public object data;
    }

    public class PeerGroupManager
    {
        private static object instLock = new object();
        private static PeerGroupManager _inst;
        private static PeerGroupManager inst
        {
            get
            {
                if (_inst == null)
                {
                    lock (instLock)
                    {
                        if (_inst == null)
                        {
                            _inst = new PeerGroupManager();
                        }
                    }
                }
                return _inst;
            }
        }
        public Dictionary<int, PeerGroup> groups;

        public PeerGroupManager()
        {
            groups = new Dictionary<int, PeerGroup>();
        }
        
        public static void RegisterGroup(PeerGroup group)
        {
            lock (inst.groups)
            {
                if (!inst.groups.ContainsKey(group.Id))
                    inst.groups.Add(group.Id, group);
            }
        }

        public static bool UnregisterGroup(PeerGroup group)
        {
            lock(inst.groups)
                return inst.groups.Remove(group.Id);
        }

        public static bool UnregisterGroup(int groupId)
        {
            lock (inst.groups)
                return inst.groups.Remove(groupId);
        }

        public static PeerGroup GetGroup(int id)
        {
            return inst.groups[id];
        }

        public static bool TryGetGroup(int id, out PeerGroup group)
        {
            return inst.groups.TryGetValue(id, out group);
        }
    }

    public class PeerManager
    {
        Dictionary<int, Peer> peers;
        
        public PeerManager()
        {
            peers = new Dictionary<int, Peer>();
        }

        public void AddPeer(Peer peer)
        {
            if (peers.ContainsKey(peer.Id))
                return;
            peers.Add(peer.Id, peer);
        }

        public bool RemovePeer(Peer peer)
        {
            return peers.Remove(peer.Id);
        }

        public Peer GetPeer(int peerId)
        {
            return peers[peerId];
        }

        public bool TryGetPeer(int peerId, out Peer peer)
        {
            return peers.TryGetValue(peerId, out peer);
        }
    }
}