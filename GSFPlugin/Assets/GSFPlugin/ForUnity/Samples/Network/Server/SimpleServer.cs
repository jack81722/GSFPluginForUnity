using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;

public class SimpleServer : Server
{
    IDebugger debugger;
    private Dictionary<int, IPeerGroup> groups;

    private Game game;

    public SimpleServer(ISerializer serializer) : base(serializer)
    {
        debugger = UnityDebugger.instance;
        game = new Game("Simple Game", debugger);
        groups = new Dictionary<int, IPeerGroup>() { { group.GroupId, group }, { game.GroupId, game } };
    }

    protected override void OnPeerConnected(IPeer peer)
    {
        try
        {
            // join game after connected
            if (game != null)
            {
                if (game.status == GameStatus.WaitToInitialize)
                {
                    debugger.Log("Start initialize game.");
                    game.Initialize().Wait();
                    debugger.Log("End initialize game.");
                }
                if (game.status == GameStatus.WaitToStart)
                {
                    debugger.Log("Start game.");
                    game.Start();
                }
            }
            debugger.Log("Join game.");
            game.JoinAsync(peer, null)
                .ContinueWith((res) => OnPeerJoinResponse(peer, res.Result));
        }
        catch (Exception e)
        {
            debugger.LogError(e);
        }
    }

    protected override void OnPeerJoinResponse(IPeer peer, JoinGroupResponse response)
    {
        GenericPacket packet = new GenericPacket();
        packet.InstCode = SimpleGameMetrics.OperationCode.Group;
        packet.Data = response;
        peer.Send(serializer.Serialize(packet), Reliability.ReliableOrder);
        debugger.Log($"Send join response : peer[{peer.Id}] join group[{response.groupId}]");
    }

    protected override void OnReceivePacket(IPeer peer, object obj, Reliability reliability)
    {
        GenericPacket packet = obj as GenericPacket;
        if(packet != null)
        {  
            if(groups.TryGetValue(packet.InstCode, out IPeerGroup group))
            {
                group.AddEvent(peer, packet.Data, reliability);
            }
        }
    }

    public void AddGroup(PeerGroup group)
    {
        if (groups.ContainsKey(group.GroupId))
        {
            groups.Add(group.GroupId, group);
        }
    }

    protected override void OnServerClose()
    {  
        foreach(var group in groups.Values)
        {
            group.Close();
        }
        groups.Clear();
    }
}



