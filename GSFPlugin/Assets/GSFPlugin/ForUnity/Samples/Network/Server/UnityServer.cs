using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityServer : IServerLaunch
{
    private Server server;
    private IDebugger debugger;

    public bool isRunning { get { return server != null && server.isRunning; } }
    public int Port { get; set; }
    public string ConnectKey { get; set; }
    public int MaxPeers { get; set; }

    public UnityServer(IDebugger debugger)
    {
        this.debugger = debugger;
    }

    public void Reset()
    {
        server = new SimpleServer(FormmaterSerializer.GetInstance());
    }

    public void Start()
    {
        debugger.Log("Launching server ... ");
        server.ConnectKey = ConnectKey;
        server.MaxPeers = MaxPeers;
        server.Start(Port);
    }

    public void Stop()
    {
        if (server != null)
            server.Close();
    }
}
