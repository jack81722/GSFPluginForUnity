using GameSystem.GameCore.Physics;

namespace GameSystem.GameCore.Components
{
    public class CapsuleCollider : Collider, ICapsuleShape
    {
        private OnceSetValue<float, float> capsuleSize = new OnceSetValue<float, float>(0.5f, 1f);
        public float Radius { get { return capsuleSize.Value1; } }
        public float Height { get { return capsuleSize.Value2; } }

        public override void Start()
        {
            colProxy = Manager.PhysicEngine.CreateCapsuleCollision(this, Layer, Mask);
            capsuleSize.Lock();
            base.Start();
        }

        public void SetSize(float radius, float height)
        {
            capsuleSize.Set(radius, height);
        }
    }
}
