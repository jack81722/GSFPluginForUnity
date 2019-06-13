using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GameSystem.GameCore
{
    public class Game
    {
        GameInformation gameInfo;
        public Scene mainScene;
        SceneBuilder sceneBuilder;

        public float TargetFPS = 60f;

        public IDebugger Debugger;
        private IPhysicEngineFactory PhysicEngineFactory;

        bool running;
        
        PhysicEngineProxy physicEngine;
        GameSourceManager gameSourceManager;

        public Game(int gameID, IPhysicEngineFactory physicEngineFactory, IDebugger debugger)
        {
            gameInfo = new GameInformation(gameID, this);
            Debugger = debugger;
            PhysicEngineFactory = physicEngineFactory;
            physicEngine = PhysicEngineFactory.Create(Debugger);
            gameSourceManager = new GameSourceManager(gameInfo, physicEngine, debugger);
            mainScene = new Scene(gameSourceManager, debugger);
        }

        public void Initialize()
        {
            sceneBuilder = new SimpleGameBuilder(Debugger);
            sceneBuilder.Build(mainScene);
        }

        public void Start()
        {
            GameLoop();
        }

        public void Stop()
        {
            running = false;
        }

        public void Close()
        {
            running = false;
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
                    physicEngine.Update(deltaTime);
                    gameSourceManager.Update(deltaTime);
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

        #region Player System
        public class PlayerTokenManager
        {
            private int serialNum = 0;

            public int NewPID()
            {
                return serialNum++;
            }

            private List<PlayerToken> logList;

            public PlayerToken RegisterPlayer(IPeer peer, object data)
            {
                // how to avoid duplicate register?
                if (!logList.Exists(t => t.Data.Equals(data)))
                {
                    PlayerToken token = new PlayerToken(this, NewPID(), peer, data);
                    logList.Add(token);
                    return token;
                }
                throw new InvalidOperationException("Cannot register player unspecified data.");
            }

            List<PlayerToken> waitingPlayerList;
            List<PlayerToken> playingPlayerList;
            public void PlayerJoin(IPeer peer, object data)
            {
                PlayerToken token = RegisterPlayer(peer, data);
                waitingPlayerList.Add(token);
            }

            public PlayerToken GetWaitingPlayer()
            {
                if (waitingPlayerList.Count < 0)
                    throw new InvalidOperationException("No waiting player in line.");
                return waitingPlayerList[0];
            }

            public PlayerToken[] GetWaitingPlayers()
            {
                return waitingPlayerList.ToArray();
            }

            public PlayerToken[] GetWaitingPlayer(int count)
            {
                return waitingPlayerList.GetRange(0, count).ToArray();
            }

            public void Accept(PlayerToken token)
            {
                if (waitingPlayerList.Contains(token))
                {
                    waitingPlayerList.Remove(token);
                    playingPlayerList.Add(token);
                }
            }

            public void Reject(PlayerToken token)
            {
                waitingPlayerList.Remove(token);
            }

            public void RejectAll()
            {
                waitingPlayerList.Clear();
            }
        }

        public class PlayerToken
        {
            PlayerTokenManager manager;
            public int PID { get; private set; }
            public IPeer Peer { get; private set; }
            public object Data;

            public PlayerToken(PlayerTokenManager manager, int pid, IPeer peer, object data)
            {
                this.manager = manager;
                PID = pid;
                Peer = peer;
                Data = data;
            }

            public void Accpet()
            {
                manager.Accept(this);
                // send join success signal
            }

            public void Reject()
            {
                manager.Reject(this);
                // send join fail signal
            }
        }

        PlayerTokenManager playerTokenMgr;
        public void PlayerJoin(IPeer peer, object data)
        {
            playerTokenMgr.PlayerJoin(peer, data);
        }

        public PlayerToken GetWaitingPlayer()
        {
            return playerTokenMgr.GetWaitingPlayer();
        }
        #endregion
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
