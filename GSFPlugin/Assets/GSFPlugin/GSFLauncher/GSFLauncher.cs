using BulletEngine;
using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.Physics;
using System.Threading.Tasks;

public class GSFLauncher : UnityEngine.MonoBehaviour
{
    /// <summary>
    /// Boolean of turn On/Off debug mode
    /// </summary>
    public bool debugMode = true;

    protected Task GSFTask;
    protected SceneBuilder builder;
    //VirtualServer server;

    private IDebugger debugger;

    public int serverPort = 8888;
    public string connectKey = "Test";
    public int maxPeers = 10;

    ServerPeer serverPeer;

    public void Awake()
    {
        debugger = new UnityDebugger();
        //server = new VirtualServer(debugger);
        //server.ConnectKey = connectKey;
        //server.MaxPeers = maxPeers;
        
        builder = new SimpleGameBuilder(debugger);
        serverPeer = new MyServerPeer(new FormmaterSerializer());
        serverPeer.MaxPeers = maxPeers;
        serverPeer.ConnectKey = connectKey;
    }

    private void Start()
    {
        if (builder != null)
        {
            if (debugMode)
            {
                var simulator = FindObjectOfType<Simulator>();
                
            }
            Launch();
        }
    }

    private void Launch()
    {
        Log("Launching...");
        //server.Start(8888);
        //GSFTask = Task.Factory.StartNew(() => server.Start(serverPort));
        serverPeer.Start(8888);
    }

    public void StartGame()
    {
        //server.StartGame();
    }

    private void Stop()
    {
        //server.Close();
        serverPeer.Close();
        if (GSFTask != null) GSFTask.Wait();
    }

    private void OnDestroy()
    {
        Stop();
        if (GSFTask != null)
            Log("Close GSF Task : " + GSFTask.Status);
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
