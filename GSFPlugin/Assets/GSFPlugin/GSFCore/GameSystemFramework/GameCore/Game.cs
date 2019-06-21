using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameSystem.GameCore
{
    public class Game
    {
        private static IdentityPool idPool = new IdentityPool();
        public int Id { get; private set; }
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

        public Game(PhysicEngineProxy physicEngine, IDebugger debugger)
        {
            Id = idPool.NewID();
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
            return Id;
        }

        public void Start()
        {
            UnityEngine.Debug.Log("Start Game");
            loopTask = Task.Run(GameLoop);
        }

        public void Stop()
        {
            running = false;
        }

        public void Close()
        {
            running = false;
            if(loopTask != null)
                loopTask.Wait();
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
                if (deltaTime.TotalSeconds < TargetSecond)
                {
                    Thread.Sleep((int)(TargetSecond - deltaTime.TotalSeconds) * 1000);
                }
                last_time = curr_time;
            }
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

        public List<JoinGroupRequest> GetJoinPeerList()
        {
            List<JoinGroupRequest> reqs = new List<JoinGroupRequest>();
            while (peerGroup.GetQueueingCount() > 0)
                reqs.Add(peerGroup.DequeueJoinRequest());
            return reqs;
        }

        public QueueStatus GetQueueStatus()
        {
            if (peerGroup.GetPeerList().Count + GetJoinPeerList().Count >= MaxPlayerCount)
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
    }
}
