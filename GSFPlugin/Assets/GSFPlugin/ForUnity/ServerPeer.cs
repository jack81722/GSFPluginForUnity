using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ServerPeer
{
    EventBasedNetListener listener;
    NetManager netManager;

    Task receiveTask;
    CancellationTokenSource receiveTcs;

    public int MaxPeers = 1000;
    public string ConnectKey = "";

    protected PeerGroup group;
    protected ISerializer serializer;

    public ServerPeer(ISerializer serializer)
    {
        this.serializer = serializer;
        group = new PeerGroup(serializer);
        listener = new EventBasedNetListener();
        netManager = new NetManager(listener);
        group.OnGroupReceiveEvent += OnReceivePacket;
        receiveTcs = new CancellationTokenSource();
    }

    protected virtual bool OnPeerConnect(Peer peer)
    {
        return true;
    }

    protected virtual void OnPeerJoinResponse(Peer peer, JoinGroupResponse response)
    {
        byte[] dgram = serializer.Serialize(response);
        peer.Send(dgram, Reliability.ReliableOrder);
    }

    protected abstract void OnReceivePacket(Peer peer, object obj, Reliability reliability);

    public void Start(int port)
    {   
        // initialize listener events
        listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
        listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
        listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
        // start listen
        netManager.Start(port);
        // start receive loop
        receiveTask = Task.Run(ReceiveLoop, receiveTcs.Token);
        Debug.Log($"Start Server[Port:{port}]");
    }

    private async Task ReceiveLoop()
    {
        while (!receiveTcs.IsCancellationRequested)
        {
            netManager.PollEvents();
            HandleGroupJoinReqeust();
            await Task.Delay(15);
        }
    }

    private void Listener_PeerConnectedEvent(NetPeer peer)
    {
        RUDPPeer newPeer = new RUDPPeer(peer);
        peer.Tag = newPeer;
        Task.Run(() => group.JoinAsync(newPeer, null));
    }

    private JoinGroupResponse Group_OnPeerJoinRequest(JoinGroupRequest request)
    {
        JoinGroupResponse response;
        if (OnPeerConnect(request.Peer))
            response = new JoinGroupResponse(group.Id, JoinGroupResponse.ResultType.Accepted, "");
        else
            response = new JoinGroupResponse(group.Id, JoinGroupResponse.ResultType.Rejected, "");
        Debug.Log($"Peer[{request.Peer.Id}] is {response.type}");
        return response;
    }

    private void HandleGroupJoinReqeust()
    {
        while (group.GetQueueingCount() > 0)
            Group_OnPeerJoinRequest(group.DequeueJoinRequest());
    }

    private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte[] dgram = new byte[reader.AvailableBytes];
        reader.GetBytes(dgram, dgram.Length);
        Peer p = (Peer)peer.Tag;
        OnReceivePacket(p, serializer.Deserialize(dgram), (Reliability)deliveryMethod);
    }

    private void Listener_ConnectionRequestEvent(ConnectionRequest request)
    {
        if (group.GetPeerList().Count < MaxPeers)
            request.AcceptIfKey(ConnectKey);
        else
            request.Reject();
    }

    public Peer GetPeer(int peerId)
    {
        return group.GetPeer(peerId);
    }

    public bool TryGetPeer(int peerId, out Peer peer)
    {
        return group.TryGetPeer(peerId, out peer);
    }

    public void Close()
    {
        receiveTcs.Cancel();
        receiveTask.Wait();
        group.Close();
        netManager.DisconnectAll();
        Debug.Log($"Receive task : {receiveTask.Status}");
    }
}

public class MyServerPeer : ServerPeer
{   
    IDebugger debugger;
    Dictionary<int, Game> games;
    Dictionary<int, PeerGroup> groups;

    public MyServerPeer(ISerializer serializer) : base(serializer)
    {
        debugger = new UnityDebugger();
        games = new Dictionary<int, Game>();
        groups = new Dictionary<int, PeerGroup>() { { group.Id, group } };
    }

    protected override void OnReceivePacket(Peer peer, object obj, Reliability reliability)
    {
        object[] packet = (object[])obj;
        switch ((int)packet[0])
        {
            case -1:
                Debug.Log(packet[1]);
                peer.Send(serializer.Serialize("123"), Reliability.ReliableOrder);
                break;
            case 0:
                JoinGame(peer, packet[1]);
                break;
            case 1:
                StartGame();
                break;
        }
    }

    private void JoinGame(Peer peer, object arg)
    {
        Debug.Log(arg);
        if (games.Count <= 0)
        {
            Game game = new Game(new BulletEngine.BulletPhysicEngine(debugger), debugger);
            Task.Run(game.Initialize);
            games.Add(game.Id, game);
        }
        // search compatible game for player
        foreach(var g in games.Values)
        {
            Debug.Log(g.GetQueueStatus());
            if(g.GetQueueStatus() == QueueStatus.Smooth)
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
        foreach(var g in games.Values)
        {
            g.Start();
        }
    }
}