using GameSystem.GameCore.Physics;
using GameSystem.GameCore.Debugger;

namespace BulletEngine
{
    public class BulletEngineFactory : IPhysicEngineFactory
    {
        public PhysicEngineProxy Create(IDebugger debugger)
        {
            return new BulletPhysicEngine(debugger);
        }
    }
}
