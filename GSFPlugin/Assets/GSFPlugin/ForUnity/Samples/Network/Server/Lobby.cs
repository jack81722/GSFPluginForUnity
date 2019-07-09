using GameSystem.GameCore;
using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Lobby : IPeerGroup
{
    private PeerGroup group;
    private LogicLooper looper;

    private Dictionary<int, Game> games;

    #region IPeerGroup properties
    public int GroupId { get { return group.GroupId; } }
    public int OperationCode { get { return group.OperationCode; } }
    #endregion

    public Lobby(ISerializer serializer)
    {
        group = new PeerGroup(serializer);
        group.OperationCode = SimpleGameMetrics.OperationCode.Lobby;
        group.OnGroupReceiveEvent += Group_OnReceiverPacket;
        looper = new LogicLooper();
        looper.OnUpdated += Looper_OnUpdated;
        looper.Start();

        games = new Dictionary<int, Game>();
    }

    private void Group_OnReceiverPacket(IPeer peer, object data, Reliability reliability)
    {
        int switchCode = 0;
        
        switch (switchCode)
        {
            case 1:    // Create Game
                CreateGame();
                break;
            // Join Game

            // Get Game List
        }
    }

    private void Looper_OnUpdated(TimeSpan deltaTime)
    {
        // authenticate
        while (group.GetQueueingCount() > 0)
        {
            var req = group.DequeueJoinRequest();
            req.Accept(null);
            UnityDebugger.instance.Log($"Accepted : GroupId = {req.GroupId}, OperationCode = {req.OperationCode}");
        }
        group.Poll();
    }

    #region IPeerGroup methods
    public void AddEvent(IPeer peer, object data, Reliability reliability)
    {
        group.AddEvent(peer, data, reliability);
    }

    public void Close()
    {
        looper.Close();
        foreach (var g in games.Values)
        {
            g.Close();
        }
        games.Clear();
        group.Close();
    }

    public List<IPeer> FindAllPeers(Predicate<IPeer> predicate)
    {
        return group.FindAllPeers(predicate);
    }

    public IPeer GetPeer(int peerID)
    {
        return group.GetPeer(peerID);
    }

    public Task<JoinGroupResponse> JoinAsync(IPeer peer, object arg)
    {
        return group.JoinAsync(peer, arg);
    }

    public bool TryGetPeer(int peerID, out IPeer peer)
    {
        return group.TryGetPeer(peerID, out peer);
    }
    #endregion

    #region Game room methods
    public void CreateGame(string name = "Default Name")
    {
        Game game = new Game(name, UnityDebugger.instance);
        games.Add(game.GameId, game);
    }

    public List<GameInformation> GetGameList()
    {
        List<GameInformation> gameInfoList = new List<GameInformation>();
        foreach(var game in games.Values)
        {
            gameInfoList.Add(new GameInformation(game.GameId, game.GetGameName()));
        }
        return gameInfoList;
    }
    #endregion
}