using GameSystem.GameCore.Debugger;
using System;
using GameSystem.GameCore.Physics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameSystem.GameCore
{
    public class Scene
    {
        int GameID;
        int SceneID;

        private bool running;
        public float TargetFps = 30;
        
        public GameSourceManager GSManager { get; private set; }

        public IDebugger Debugger { get; private set; }

        public Scene(GameSourceManager gsManager, IDebugger debugger)
        {
            Debugger = debugger;
            GSManager = gsManager;
        }

        public void Close()
        {
            GSManager.Clear();
        }
    }
}
