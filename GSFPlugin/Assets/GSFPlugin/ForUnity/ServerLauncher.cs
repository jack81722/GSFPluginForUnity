using BulletEngine;
using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System.Threading.Tasks;

public class ServerLauncher : UnityEngine.MonoBehaviour
{
    /// <summary>
    /// Boolean of turn On/Off debug mode
    /// </summary>
    public bool debugMode = true;

    private IDebugger debugger;

    public int Port = 8888;
    public string ConnectKey = "Test";
    public int MaxPeers = 10;

    private Server server;

    public void Awake()
    {   
        debugger = new UnityDebugger();
        server = new SimpleServer(new FormmaterSerializer());
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Launch();
    }

    public void Launch()
    {   
        Log("Launching server ... ");
        server.ConnectKey = ConnectKey;
        server.MaxPeers = MaxPeers;
        server.Start(Port);
    }

    private void Stop()
    {
        server.Close();
    }

    private void OnDestroy()
    {
        Stop();
    }

    private void Log(object obj)
    {
        debugger.Log(obj);
    }

    private void LogError(object obj)
    {
        debugger.LogError(obj);
    }

    private void LogWarning(object obj)
    {
        debugger.LogWarning(obj);
    }
}
