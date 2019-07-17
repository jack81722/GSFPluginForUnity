using BulletEngine;
using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System.Threading.Tasks;

public class ServerLauncher : UnityEngine.MonoBehaviour
{
    public bool StartOnAwake = true;
    public bool StopOnDestroy = true;

    private IDebugger debugger = UnityDebugger.GetInstance();

    public int Port;
    public string ConnectKey;
    public int MaxPeers;

    private Server server;
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
        Log("Launching server ... ");
        server.ConnectKey = ConnectKey;
        server.MaxPeers = MaxPeers;
        server.Start(Port);
    }

    public void Stop()
    {
        if(server != null)
            server.Close();
    }

    public void ResetServer()
    {
        server = new SimpleServer(FormmaterSerializer.GetInstance());
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
