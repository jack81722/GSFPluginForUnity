using System;
using GameSystem.GameCore.Physics;

namespace GameSystem.GameCore
{
    /// <summary>
    /// Basic collider component
    /// </summary>
    public abstract class Collider : Component
    {
        protected CollisionProxy colProxy;
        public event OnCollisionHandler OnCollisionEvent;

        #region Mask properties
        private bool lockMask = false;
        private int _mask = -1;
        public int Mask
        {
            get { return _mask; }
            set
            {
                if (!lockMask)
                    _mask = value;
                else
                    throw new InvalidOperationException("Mask was been locked.");
            }
        }
        #endregion

        public override void Start()
        {
            colProxy.collider = this;
            colProxy.SetTransform(transform.matrix);
            colProxy.CollisionEvent += ColProxy_CollisionEvent;
            lockMask = true;
        }

        public override void OnDestroy()
        {
            // remove collision proxy from physic engine
            Manager.PhysicEngine.RemoveCollision(colProxy);
        }

        public override void LateUpdate()
        {
            colProxy.SetTransform(transform.matrix);
        }

        private void ColProxy_CollisionEvent(CollisionProxy colA, CollisionProxy colB)
        {
            OnCollisionEvent.Invoke(colA.collider, colB.collider);
        }
    }

    public delegate void OnCollisionHandler(Collider self, Collider other);

}
