using GameSystem.GameCore.Physics;

namespace GameSystem.GameCore.Components
{
    public class ConeCollider : Collider, IConeShape
    {
        private OnceSetValue<float, float> coneSize = new OnceSetValue<float, float>(0.5f, 1f);
        public float Radius { get { return coneSize.Value1; } }
        public float Height { get { return coneSize.Value2; } }

        public override void Start()
        {
            colProxy = Manager.PhysicEngine.CreateConeCollision(this, Layer, Mask);
            coneSize.Lock();
            base.Start();
        }

        public void SetSize(float radius, float height)
        {
            coneSize.Set(radius, height);
        }
    }
}
