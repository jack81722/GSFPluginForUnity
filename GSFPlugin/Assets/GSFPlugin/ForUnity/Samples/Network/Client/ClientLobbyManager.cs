using GameSystem;
using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ClientLobbyManager : MonoBehaviour, IPacketReceiver
{
    private ClientPeer peer;

    public int OperationCode { get { return SimpleGameMetrics.OperationCode.Lobby; } }

    private void Start()
    {
        peer = FindObjectOfType<ClientPeerLauncher>().peer;
    }

    public void Receive(object packet)
    {
        // Get Game Information List

        
    }

    public void SendRequest(int switchCode, object data)
    {   
        peer.Send(new object[] { switchCode, data }, Reliability.ReliableOrder);
        // how to wait response
        // 1. what will happen while time out?
        // 2. what event will execute after receive response
    }
}

public class RequestHandler
{
    static ISerializer serializer;
    static IPeer peer;

    DateTime startTime;
    int timeout;

    public int num;
    public Status status { get; private set; }
    public Task<object> task { get { return tcs.Task; } }

    private TaskCompletionSource<object> tcs;
    private CancellationTokenSource cts;

    public RequestHandler(int timeout)
    {
        startTime = DateTime.Now;
        this.timeout = timeout;
        tcs = new TaskCompletionSource<object>();
        cts = new CancellationTokenSource(timeout);
    }

    public static async Task<TResponse> Require<TResponse>(object request)
    {
        Packet packet = new Packet();
        peer.Send(serializer.Serialize(packet), Reliability.ReliableOrder);
        RequestHandler handler = new RequestHandler(5000);
        _handlers.Add(handler.num, handler);
        StartWaiting();
        await handler.task;
        return (TResponse)handler.task.Result;
    }

    private static void StartWaiting()
    {
        if (!updating)
        {
            Task.Run(UpdateLoop);
            updating = true;
        }
    }

    private static void UpdateLoop()
    {
        while (_handlers.Count > 0)
        {
            UpdateHandlers();
            Thread.Sleep(30);
        }
        updating = false;
    }

    private static void UpdateHandlers()
    {
        DateTime time = DateTime.Now;
        List<int> removeNums = new List<int>();
        foreach(var handler in _handlers.Values)
        {
            handler.SetTime(time);
            if (handler.status != Status.Waiting)
                removeNums.Add(handler.num);
        }
        for(int i = 0; i < removeNums.Count; i++)
        {
            _handlers.Remove(removeNums[i]);
        }
    }

    static bool updating;
    static Dictionary<int, RequestHandler> _handlers = new Dictionary<int, RequestHandler>();

    private void SetTime(DateTime time)
    {
        if ((time - startTime).TotalMilliseconds > timeout)
        {
            status = Status.Timedout;
        }
        if (cts.IsCancellationRequested)
        {
            status = Status.Cancelled;
        }
    }
    
    public void Cancel()
    {
        cts.Cancel();
    }

    public enum Status
    {
        Cancelled,
        Timedout,
        Completed,
        Waiting
    }

    public class Packet
    {
        public int num;
        public bool req, res;
        public object data;
    }

}
