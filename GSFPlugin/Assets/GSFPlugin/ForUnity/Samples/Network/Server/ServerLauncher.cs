using BulletEngine;
using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System.Text;
using System.Threading.Tasks;

public class ServerLauncher : UnityEngine.MonoBehaviour
{
    public bool StartOnAwake = true;
    public bool StopOnDestroy = true;

    private IDebugger debugger = UnityDebugger.GetInstance();

    public int Port;
    public string ConnectKey;
    public int MaxPeers;

    IServerLaunch server;
    public bool isRunning { get { return server != null && server.isRunning; } }

    public void Awake()
    {
#if UNITY_EDITOR
        if (StartOnAwake && !isRunning)
        {
            ResetServer();
            Launch();
        }
        DontDestroyOnLoad(gameObject);
#endif
    }

    public void Launch()
    {
        server.Port = Port;
        server.ConnectKey = ConnectKey;
        server.MaxPeers = MaxPeers;
        server.Start();
    }

    public void Stop()
    {
        if (server != null)
            server.Stop();
    }

    public void ResetServer()
    {
        if (server == null)
        {
            //server = new UnityServer(debugger);
            ProcessServer ps = new ProcessServer();
            ps.OnReceiveOutput += (sender, args) =>
            {
                debugger.Log(args.Data);
            };
            server = ps;
        }
        server.Reset();
    }

    private void OnDestroy()
    {
        if (StopOnDestroy && isRunning)
        {
            debugger.Log("Stop on destroy");
            Stop();
        }
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




public interface IServerLaunch
{
    int Port { get; set; }
    string ConnectKey { get; set; }
    int MaxPeers { get; set; }

    bool isRunning { get; }
    void Start();
    void Stop();
    void Reset();
}
