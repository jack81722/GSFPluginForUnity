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

    private Lobby lobby;

    public SimpleServer(ISerializer serializer) : base(serializer)
    {
        debugger = new UnityDebugger();
        lobby = new Lobby(serializer);
        groups = new Dictionary<int, IPeerGroup>() { { group.GroupId, group }, { lobby.GroupId, lobby } };
    }

    protected override void OnPeerConnected(IPeer peer)
    {
        // join lobby after connected
        lobby.JoinAsync(peer, null)
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

    }

    private void Looper_OnUpdated(TimeSpan deltaTime)
    {
        // authenticate
        while(group.GetQueueingCount() > 0)
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
    public void GetGameList()
    {
        // return all game information (id, name, ... etc.)
    }
    #endregion
}

public class LogicLooper
{
    private bool running;
    private bool isClosed;
    private Task loopTask;

    public float TargetFPS = 30f;
    
    public delegate void OnUpdatedEventHandler(TimeSpan deltaTime);
    public delegate void OnCloseEventHandler();

    public OnUpdatedEventHandler OnUpdated;
    public OnCloseEventHandler OnClose;

    public void Start()
    {
        loopTask = Task.Run(Loop);
    }

    public void Stop()
    {
        running = false;
    }

    private void Loop()
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
                OnUpdated.Invoke(deltaTime);
            }
            // correct time into fps
            float TargetSecond = 1f / TargetFPS;
            int delayTime = (int)(TargetSecond - deltaTime.TotalSeconds) * 1000;
            // force release thread 5 ms
            if (delayTime > 5)
                Thread.Sleep(delayTime);
            else
                Thread.Sleep(5);
            last_time = curr_time;
        }
        if (isClosed)
            Task.Run(CloseSafely);
    }

    public void Close()
    {
        if (isClosed)
            return;
        if (!running)
            CloseSafely();
        running = false;    // set loop stopped
        isClosed = true;
    }

    private void CloseSafely()
    {
        OnClose.Invoke();
    }
}