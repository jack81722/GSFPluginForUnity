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
        public int MaxPeer;
        public string ConnectKey;

        int port;

        private bool isReceiving;
        Task recvTask;

        EventBasedNetListener listener;
        NetManager serverNetManager;
        PeerGroup peerGroups;

        Game game;

        ISerializer serializer;
        IDebugger debugger;

        public VirtualServer(IDebugger debugger)
        {
            listener = new EventBasedNetListener();
            serverNetManager = new NetManager(listener);
            peerGroups = new PeerGroup(new FormmaterSerializer());
            this.debugger = debugger;
        }

        public Game CreateGame(IPhysicEngineFactory phyEnginFactory)
        {
            game = new Game(0, phyEnginFactory, debugger);
            return game;
        }

        public void Start(int port)
        {
            debugger.Log("Start Server.");
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            serverNetManager.Start(port);
            recvTask = Task.Factory.StartNew(KeepReceive);
        }

        private void Listener_PeerConnectedEvent(NetPeer peer)
        {
            peerGroups.AddPeer(new RUDPPeer(peer));
        }

        private async void KeepReceive()
        {
            isReceiving = true;
            while (isReceiving)
            {
                //debugger.Log("Receive.");
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
            peerGroups.GetPeer(peer.Id).AddPacketEvent(dgram);
        }

        private void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            debugger.Log("Connection Request Event.");
            if (serverNetManager.PeersCount < MaxPeer)
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
}