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
    VirtualServer server;

    private IDebugger debugger;

    public void Awake()
    {
        debugger = new UnityDebugger();
        IPhysicEngineFactory physicEngineFactory = new BulletEngineFactory();
        server = new VirtualServer(debugger);
        builder = new SimpleGameBuilder(debugger);
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
        GSFTask = Task.Factory.StartNew(() => server.Start(8888));
    }

    private void Stop()
    {
        server.Close();
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
