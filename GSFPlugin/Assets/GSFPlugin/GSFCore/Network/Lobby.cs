using GameSystem.GameCore.Debugger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class Lobby : Room
    {
        Dictionary<int, Game> gameDict;

        public Lobby(ISerializer serializer, IDebugger debugger) : base(serializer, debugger)
        {
            gameDict = new Dictionary<int, Game>();
        }

        protected override void LoopLogic()
        {
            debugger.Log("Running");
        }

        protected override void ReceiveLogic(Peer peer, object obj, Reliability reliability)
        {

        }

        public Game CreateGame()
        {
            Game game = new Game(new BulletEngine.BulletPhysicEngine(debugger), debugger);
            gameDict.Add(game.Id, game);
            return game;
        }

    }
}