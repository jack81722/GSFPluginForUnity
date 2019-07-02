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
        groups = new Dictionary<int, PeerGroup>() { { group.GroupId, group } };
    }

    protected override void OnPeerJoinResponse(IPeer peer, JoinGroupResponse response)
    {
        //object[] packet = new object[] { SimpleGameMetrics.OperationCode.Group_JoinResponse, response };
        GenericPacket packet = new GenericPacket();
        packet.InstCode = SimpleGameMetrics.OperationCode.Group_JoinResponse;
        packet.Data = response;
        peer.Send(serializer.Serialize(packet), Reliability.ReliableOrder);
    }

    protected override void OnReceivePacket(IPeer peer, object obj, Reliability reliability)
    {
        GenericPacket packet = obj as GenericPacket;
        if(packet != null)
        {  
            if(groups.TryGetValue(packet.InstCode, out PeerGroup group))
            {
                group.AddEvent(peer, packet.Data, reliability);
            }
        }
        //object[] packet = (object[])obj;
        //switch ((int)packet[0])
        //{
        //    case SimpleGameMetrics.SwitchCode.Lobby_Log:
        //        Debug.Log($"Client said : \"{packet[1]}\"");
        //        peer.Send(serializer.Serialize(new object[] { SimpleGameMetrics.OperationCode.Chat, "Hi" }), Reliability.ReliableOrder);
        //        break;
        //    case SimpleGameMetrics.SwitchCode.Lobby_JoinGame:
        //        JoinGame(peer, packet[1]);
        //        break;
        //    case SimpleGameMetrics.SwitchCode.Lobby_StartGame:
        //        StartGame();
        //        break;
        //    case SimpleGameMetrics.SwitchCode.Lobby_GameControl:
        //        ReceiveGamePacket(peer, packet[1], reliability);
        //        break;
        //}
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
