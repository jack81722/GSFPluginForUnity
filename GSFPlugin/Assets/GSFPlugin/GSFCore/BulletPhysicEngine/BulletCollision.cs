using BulletSharp;
using GameSystem.GameCore;
using GameSystem.GameCore.Physics;
using GameSystem.GameCore.SerializableMath;

namespace BulletEngine
{
    public class BulletCollision : CollisionProxy
    {
        /// <summary>
        /// Collision object of bullet engine
        /// </summary>
        public CollisionObject colObj;
        public override object CollisionObject { get { return colObj; } set { colObj = (CollisionObject)value; } }

        public BulletCollision (CollisionObject colObj, Collider collider) : base(collider)
        {
            this.colObj = colObj;
            colObj.UserObject = this;
        }

        public override void SetTransform(Matrix4x4 matrix)
        {
            colObj.WorldTransform = matrix.ToBullet();
        }
    }
}
