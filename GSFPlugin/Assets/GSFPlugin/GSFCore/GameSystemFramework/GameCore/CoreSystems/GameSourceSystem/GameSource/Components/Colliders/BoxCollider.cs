using GameSystem.GameCore.Physics;
using GameSystem.GameCore.SerializableMath;
using System;

namespace GameSystem.GameCore.Components
{
    public class BoxCollider : Collider, IBoxShape
    {
        private OnceSetValue<Vector3> halfSize = new OnceSetValue<Vector3>(new Vector3(0.5f));
        public Vector3 HalfSize { get { return halfSize; } }

        public override void Start()
        {
            if (Manager.PhysicEngine == null)
                throw new InvalidOperationException("Physic engine was not be installed.");
            colProxy = Manager.PhysicEngine.CreateBoxCollision(this, Layer, Mask);
            // size must be locked when starting
            halfSize.Lock();
            base.Start();
        }

        public void SetSize(Vector3 halfSize)
        {
            this.halfSize.Value = halfSize;
        }
    }
}
