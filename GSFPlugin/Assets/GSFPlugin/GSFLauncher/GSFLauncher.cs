using BulletEngine;
using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
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

    private Game game;
    private IDebugger debugger;

    public void Awake()
    {
        debugger = new UnityDebugger();
        IPhysicEngineFactory physicEngineFactory = new BulletEngineFactory();
        game = new Game(0, physicEngineFactory, debugger);
        builder = new SimpleGameBuilder(debugger);
    }

    private void Start()
    {
        if (builder != null)
        {
            game.Initialize();
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
        GSFTask = Task.Factory.StartNew(game.Start);
    }

    private void Stop()
    {
        game.Stop();
        if (GSFTask != null) GSFTask.Wait();
    }

    private void OnDestroy()
    {
        Stop();
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
