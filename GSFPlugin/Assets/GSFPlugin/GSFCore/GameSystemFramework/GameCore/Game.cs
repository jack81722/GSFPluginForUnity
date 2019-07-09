﻿using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameSystem.GameCore
{
    public class Game : IPeerGroup
    {
        private static IdentityPool idPool = new IdentityPool();
        public int GameId { get; private set; }
        public string GameName { get; private set; }
        public int GroupId { get { return peerGroup.GroupId; } }
        public int OperationCode { get { return peerGroup.OperationCode; } }
        public Scene mainScene;
        SceneBuilder sceneBuilder;

        LogicLooper looper;

        public IDebugger Debugger;

        public GameStatus status;
        
        private PhysicEngineProxy physicEngine;
        private GameSourceManager gameSourceManager;
        public PeerGroup peerGroup;

        public int MaxPlayerCount = 20;
        public int PlayerCount { get; }

        public delegate void ReceiveGamePacketHandler(IPeer peer, object packet);
        public ReceiveGamePacketHandler OnReceiveGamePacket;

        public Game(string name, IDebugger debugger)
        {
            GameId = idPool.NewID();
            GameName = name;
            Debugger = debugger;
            physicEngine = new BulletEngine.BulletPhysicEngine(debugger);
            gameSourceManager = new GameSourceManager(this, physicEngine, debugger);
            mainScene = new Scene(gameSourceManager, debugger);
            peerGroup = new PeerGroup(new FormmaterSerializer());
            peerGroup.OperationCode = SimpleGameMetrics.OperationCode.Game;

            looper = new LogicLooper(60f);
            looper.OnUpdated += GameLoop;

            status = GameStatus.WaitToInitialize;
        }

        public async Task Initialize()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            sceneBuilder = new SimpleGameBuilder(Debugger);
            sceneBuilder.Build(mainScene);

            peerGroup.OnGroupReceiveEvent += ReceiveGamePacket;
            tcs.SetResult(null);
            await tcs.Task;

            status = GameStatus.WaitToStart;
        }

        public int GetGameID()
        {
            return GameId;
        }

        public string GetGameName()
        {
            return GameName;
        }

        public void Start()
        {
            looper.Start();
            status = GameStatus.Running;
        }

        public void Close()
        {   
            looper.Close();
            // let all peers in group exit
            peerGroup.ExitAll("Game is closed.", null);
            idPool.RecycleID(GameId);
            status = GameStatus.Closed;
        }

        private void GameLoop(TimeSpan deltaTime)
        {
            // update physic objects
            physicEngine.Update(deltaTime);
            // update game sources 
            gameSourceManager.Update(deltaTime);
            // receive network packet and execute receive events
            peerGroup.Poll();
        }

        #region Join request methods
        public List<JoinGroupRequest> GetJoinRequests(int count)
        {
            int i = 0;
            List<JoinGroupRequest> reqs = new List<JoinGroupRequest>();
            while (i < count && peerGroup.GetQueueingCount() > 0)
            {
                reqs.Add(peerGroup.DequeueJoinRequest());
            }
            return reqs;
        }

        public List<JoinGroupRequest> GetJoinRequestList()
        {
            List<JoinGroupRequest> reqs = new List<JoinGroupRequest>();
            while (peerGroup.GetQueueingCount() > 0)
                reqs.Add(peerGroup.DequeueJoinRequest());
            return reqs;
        }

        public QueueStatus GetQueueStatus()
        {
            // connected_peers + handling_peers + queueing_peers
            if (peerGroup.GetPeerList().Count + peerGroup.GetHandlingCount() + peerGroup.GetQueueingCount() >= MaxPlayerCount)
                return QueueStatus.Crowded;
            else if (peerGroup.GetPeerList().Count >= MaxPlayerCount)
                return QueueStatus.Full;
            else
                return QueueStatus.Smooth;
        }
        #endregion

        public void Send(int peerID, object obj, Reliability reliability)
        {
            Task task = peerGroup.SendAsync(peerID, obj, reliability);
        }

        public void Broadcast(object obj, Reliability reliability)
        {
            Task task = peerGroup.BroadcastAsync(obj, reliability);
        }

        private void ReceiveGamePacket(IPeer peer, object obj, Reliability reliability)
        {
            OnReceiveGamePacket.Invoke(peer, obj);
        }

        #region IPeerGroup methods
        public void AddEvent(IPeer peer, object data, Reliability reliability)
        {
            peerGroup.AddEvent(peer, data, reliability);
        }

        public Task<JoinGroupResponse> JoinAsync(IPeer peer, object arg)
        {
            return peerGroup.JoinAsync(peer, arg);
        }

        public IPeer GetPeer(int peerID)
        {
            return peerGroup.GetPeer(peerID);
        }

        public bool TryGetPeer(int peerID, out IPeer peer)
        {
            return peerGroup.TryGetPeer(peerID, out peer);
        }

        public List<IPeer> FindAllPeers(Predicate<IPeer> predicate)
        {
            return peerGroup.FindAllPeers(predicate);
        }
        #endregion
    }

    public enum QueueStatus
    {
        Smooth,     // means amount of queuing and in group are not over maximum
        Crowded,    // means amount of queuing and in group are over maximum
        Full        // means group is full
    }

    public enum GameStatus
    {
        WaitToInitialize,
        WaitToStart,
        Running,
        Closed
    }
}
