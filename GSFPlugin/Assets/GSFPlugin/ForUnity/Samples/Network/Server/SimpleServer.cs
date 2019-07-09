using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class SimpleServer : Server
{
    IDebugger debugger;
    private Dictionary<int, IPeerGroup> groups;

    //private Lobby lobby;
    private Game game;

    public SimpleServer(ISerializer serializer) : base(serializer)
    {
        debugger = new UnityDebugger();
        game = new Game("Simple Game", debugger);
        //lobby = new Lobby(serializer);
        groups = new Dictionary<int, IPeerGroup>() { { group.GroupId, group }, { game.GroupId, game } };
    }

    protected override void OnPeerConnected(IPeer peer)
    {
        // join lobby after connected
        //lobby.JoinAsync(peer, null)
        //    .ContinueWith((res) => OnPeerJoinResponse(peer, res.Result));
        if (game != null)
        {
            if (game.status == GameStatus.WaitToInitialize)
            {
                debugger.Log("Start initialize game");
                game.Initialize().Wait();
                debugger.Log("End initialize game");
            }
            if (game.status == GameStatus.WaitToStart)
            {
                debugger.Log("Start game");
                game.Start();
            }
        }
        game.JoinAsync(peer, null)
            .ContinueWith((res) => OnPeerJoinResponse(peer, res.Result));
    }

    protected override void OnPeerJoinResponse(IPeer peer, JoinGroupResponse response)
    {
        GenericPacket packet = new GenericPacket();
        packet.InstCode = SimpleGameMetrics.OperationCode.Group;
        packet.Data = response;
        peer.Send(serializer.Serialize(packet), Reliability.ReliableOrder);
    }

    protected override void OnReceivePacket(IPeer peer, object obj, Reliability reliability)
    {
        //debugger.Log(obj.GetType());
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



