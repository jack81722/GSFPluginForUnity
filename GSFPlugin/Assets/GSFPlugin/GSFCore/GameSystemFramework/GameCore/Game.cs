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
        GameInformation gameInfo;
        public Scene mainScene;
        SceneBuilder sceneBuilder;

        public float TargetFPS = 60f;
        Task loopTask;


        public IDebugger Debugger;
        private IPhysicEngineFactory PhysicEngineFactory;

        bool running;
        PhysicEngineProxy physicEngine;
        GameSourceManager gameSourceManager;
        public PeerGroup peerGroup;

        public Game(int gameID, IPhysicEngineFactory physicEngineFactory, IDebugger debugger)
        {
            gameInfo = new GameInformation(gameID, this);
            Debugger = debugger;
            PhysicEngineFactory = physicEngineFactory;
            physicEngine = PhysicEngineFactory.Create(Debugger);
            gameSourceManager = new GameSourceManager(this, physicEngine, debugger);
            mainScene = new Scene(gameSourceManager, debugger);
            peerGroup = new PeerGroup(new FormmaterSerializer());
        }

        public void Initialize()
        {
            sceneBuilder = new SimpleGameBuilder(Debugger);
            sceneBuilder.Build(mainScene);
        }

        public void Start()
        {
            UnityEngine.Debug.Log("Start Game");
            loopTask = Task.Factory.StartNew(GameLoop);
        }

        public void Stop()
        {
            running = false;
        }

        public void Close()
        {
            running = false;
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
                float TargetSecond = 1f / gameInfo.TargetFPS;
                if (deltaTime.TotalSeconds < TargetSecond)
                {
                    Thread.Sleep((int)(TargetSecond - deltaTime.TotalSeconds) * 1000);
                }
                last_time = curr_time;
            }
        }

        public GameInformation GetGameInfo()
        {
            return gameInfo;
        }

        public void Join(IPeer peer)
        {            
            peerGroup.AddPeer(peer);
        }

        public void Send(int peerID, object obj, Reliability reliability)
        {
            peerGroup.Send(peerID, obj, reliability);
        }

        public void Broadcast(object obj, Reliability reliability)
        {
            peerGroup.Broadcast(obj, reliability);
        }
    }

    public class GameInformation
    {
        public int GameID;
        private Game _game;
        public Game game { get; }

        public float TargetFPS;

        public GameInformation(int id, Game game)
        {
            GameID = id;
            _game = game;
            TargetFPS = 30;
        }

        public void SetFPS(float fps)
        {
            TargetFPS = fps;
        }
    }
}
