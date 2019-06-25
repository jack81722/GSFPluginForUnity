using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleServer : Server
{
    IDebugger debugger;
    private Dictionary<int, Game> games;
    private Dictionary<int, PeerGroup> groups;

    public SimpleServer(ISerializer serializer) : base(serializer)
    {
        debugger = new UnityDebugger();
        games = new Dictionary<int, Game>();
        groups = new Dictionary<int, PeerGroup>() { { group.Id, group } };
    }

    protected override void OnReceivePacket(IPeer peer, object obj, Reliability reliability)
    {
        object[] packet = (object[])obj;
        switch ((int)packet[0])
        {
            case -1:
                Debug.Log($"Client said : \"{packet[1]}\"");
                peer.Send(serializer.Serialize(new object[] { -1, "Hi" }), Reliability.ReliableOrder);
                break;
            case 0:
                JoinGame(peer, packet[1]);
                break;
            case 1:
                StartGame();
                break;
        }
    }

    private void JoinGame(IPeer peer, object arg)
    {
        if (games.Count <= 0)
        {
            // if no valid game existed, then create new game
            Game game = new Game(new BulletEngine.BulletPhysicEngine(debugger), debugger);
            Task.Run(game.Initialize);
            games.Add(game.Id, game);
        }
        // search compatible game for player
        foreach (var g in games.Values)
        {
            Debug.Log($"Game[{g.Id}] status : {g.GetQueueStatus()}");
            if (g.GetQueueStatus() == QueueStatus.Smooth)
            {
                Task.Run(() => g.peerGroup.JoinAsync(peer, arg)).
                    ContinueWith(
                    (t) => {
                        peer.Send(serializer.Serialize(new object[] { 0, t.Result }), Reliability.ReliableOrder);
                        return t.Result;
                    });
                break;
            }
        }
        
    }

    private void StartGame()
    {
        foreach (var g in games.Values)
        {
            g.Start();
        }
    }

    protected override void OnServerClose()
    {
        List<Task> closingTasks = new List<Task>();
        foreach(var g in games.Values)
        {
            g.Close();
        }
        Task.WaitAll(closingTasks.ToArray());
        games.Clear();
        groups.Clear();
    }
}
