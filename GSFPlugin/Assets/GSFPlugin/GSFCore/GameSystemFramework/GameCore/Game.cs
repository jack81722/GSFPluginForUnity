using GameSystem.GameCore.Debugger;
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
        public int GroupId { get { return peerGroup.GroupId; } }
        public int OperationCode { get { return peerGroup.OperationCode; } }
        public Scene mainScene;
        SceneBuilder sceneBuilder;

        public float TargetFPS = 60f;
        private bool running;
        private Task loopTask;

        public IDebugger Debugger;
        
        private PhysicEngineProxy physicEngine;
        private GameSourceManager gameSourceManager;
        public PeerGroup peerGroup;

        public int MaxPlayerCount = 20;
        public int PlayerCount { get; }

        /// <summary>
        /// Boolean of game is cloesed
        /// </summary>
        private bool isClosed;

        public delegate void ReceiveGamePacketHandler(IPeer peer, object packet);
        public ReceiveGamePacketHandler OnReceiveGamePacket;

        public Game(PhysicEngineProxy physicEngine, IDebugger debugger)
        {
            GameId = idPool.NewID();
            Debugger = debugger;
            this.physicEngine = physicEngine;
            gameSourceManager = new GameSourceManager(this, physicEngine, debugger);
            mainScene = new Scene(gameSourceManager, debugger);
            peerGroup = new PeerGroup(new FormmaterSerializer());
        }

        public async Task Initialize()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            sceneBuilder = new SimpleGameBuilder(Debugger);
            sceneBuilder.Build(mainScene);
            tcs.SetResult(null);
            await tcs.Task;
        }

        public int GetGameID()
        {
            return GameId;
        }

        public void Start()
        {
            peerGroup.OnGroupReceiveEvent += ReceiveGamePacket;
            loopTask = Task.Run(GameLoop);
            UnityEngine.Debug.Log($"Start Game[{GameId}]");
        }

        public void Close()
        {
            if (isClosed)
                return;
            if (!running)
                CloseSafely();
            running = false;    // set loop stopped
            isClosed = true;
        }

        private void CloseSafely()
        {
            // let all peers in group exit
            peerGroup.ExitAll("Game is closed.", null);
            Debugger.Log($"Close game[{GameId}].");
        }

        private void GameLoop()
        {
            DateTime curr_time = DateTime.UtcNow;
            DateTime last_time = curr_time;
            running = true;
            while (running)
            {
                curr_time = DateTime.UtcNow;
                TimeSpan deltaTime;
                // caculate time span between current and last time
                if ((deltaTime = curr_time - last_time).TotalMilliseconds > 0)
                {
                    // update physic objects
                    physicEngine.Update(deltaTime);
                    // update game sources 
                    gameSourceManager.Update(deltaTime);
                    // receive network packet and execute receive events
                    peerGroup.Poll();
                }
                // correct time into fps
                float TargetSecond = 1f / TargetFPS;
                int delayTime = (int)(TargetSecond - deltaTime.TotalSeconds) * 1000;
                // force release thread 5 ms
                if (delayTime > 5)
                    Thread.Sleep(delayTime);
                else
                    Thread.Sleep(5);
                last_time = curr_time;
            }
            if (isClosed)
                Task.Run(CloseSafely);
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
}
