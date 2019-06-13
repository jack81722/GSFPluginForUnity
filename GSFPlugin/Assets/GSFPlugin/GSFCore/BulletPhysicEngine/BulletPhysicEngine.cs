using BulletSharp;
using GameSystem;
using GameSystem.GameCore;
using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Physics;
using System;

namespace BulletEngine
{
    public class BulletPhysicEngine : PhysicEngineProxy
    {
        private CollisionConfiguration configuration;
        private Dispatcher dispatcher;
        private BroadphaseInterface broadphase;
        private DiscreteDynamicsWorld world;

        public BulletPhysicEngine(IDebugger debugger) : base(debugger)
        {
            configuration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(configuration);
            broadphase = new DbvtBroadphase();
            world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, configuration);
        }

        public override void Update(TimeSpan deltaTime)
        {
            float second = (float)deltaTime.TotalSeconds;
            world.StepSimulation(second);

            // on collision events
            int numManifolds = world.Dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold manifold = world.Dispatcher.GetManifoldByIndexInternal(i);
                CollisionProxy colA = (CollisionProxy)manifold.Body0.UserObject;
                CollisionProxy colB = (CollisionProxy)manifold.Body1.UserObject;

                // check if both are not null
                if (colA != null && colB != null)
                {
                    // execute colA -> colB event
                    try
                    {
                        colA.ExecuteCollision(colA, colB);
                    }
                    catch (Exception e)
                    {
                        LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                    }
                    // execute colB -> colA event
                    try
                    {
                        colB.ExecuteCollision(colB, colA);
                    }
                    catch (Exception e)
                    {
                        LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                    }
                }
            }
        }

        #region Add/Remove collision methods
        public override void AddCollision(CollisionProxy colProxy, int layer = 1, int mask = -1)
        {
            world.AddCollisionObject((CollisionObject) colProxy.CollisionObject, (short)layer, (short)mask);
        }

        public override void RemoveCollision(CollisionProxy colProxy)
        {
            world.RemoveCollisionObject((CollisionObject) colProxy.CollisionObject);
        }
        #endregion

        #region Create collision methods
        public override CollisionProxy CreateBoxCollision(IBoxShape shape, int layer = 1, int mask = -1)
        {
            CollisionObject co = new CollisionObject();
            co.CollisionShape = new BoxShape(shape.HalfSize.ToBullet());
            world.AddCollisionObject(co, (short)layer, (short)mask);
            return new BulletCollision(co, null);
        }

        public override CollisionProxy CreateSphereCollision(ISphereShape shape, int layer = 1, int mask = -1)
        {
            CollisionObject co = new CollisionObject();
            co.CollisionShape = new SphereShape(shape.Radius);
            world.AddCollisionObject(co, (short)layer, (short)mask);
            return new BulletCollision(co, null);
        }

        public override CollisionProxy CreateCapsuleCollision(ICapsuleShape shape, int layer = 1, int mask = -1)
        {
            CollisionObject co = new CollisionObject();
            co.CollisionShape = new CapsuleShape(shape.Radius, shape.Height);
            world.AddCollisionObject(co, (short)layer, (short)mask);
            return new BulletCollision(co, null);
        }

        public override CollisionProxy CreateConeCollision(IConeShape shape, int layer = 1, int mask = -1)
        {
            CollisionObject co = new CollisionObject();
            co.CollisionShape = new ConeShape(shape.Radius, shape.Height);
            world.AddCollisionObject(co, (short)layer, (short)mask);
            return new BulletCollision(co, null);
        }

        public override CollisionProxy CreateOtherCollision(int shapeType, int layer = 1, int mask = -1, params object[] shapeArgs)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Raycast methods
        public override bool Raycast(GameSystem.GameCore.SerializableMath.Vector3 startPoint, GameSystem.GameCore.SerializableMath.Vector3 endPoint, out GameSystem.GameCore.SerializableMath.Vector3 hitPoint, out CollisionProxy hitObject, int mask = -1)
        {
            // transfer serializable math position to bullet math position
            BulletSharp.Math.Vector3 start = startPoint.ToBullet(), end = endPoint.ToBullet();
            var callback = new ClosestRayResultCallback(ref start, ref end);
            // set group mask filter
            callback.CollisionFilterMask = (short)mask;
            world.RayTest(start, end, callback);
            if (callback.HasHit)
            {
                hitPoint = callback.HitPointWorld.ToSerializable();
                hitObject = (CollisionProxy)callback.CollisionObject.UserObject;
            }
            else
            {
                hitPoint = GameSystem.GameCore.SerializableMath.Vector3.Zero;
                hitObject = null;
            }
            return callback.HasHit;
        }

        public override bool Raycast(GameSystem.GameCore.SerializableMath.Vector3 startPoint, GameSystem.GameCore.SerializableMath.Vector3 endPoint, out GameSystem.GameCore.SerializableMath.Vector3[] hitPoint, out CollisionProxy[] hitObject, int mask = -1)
        {
            // transfer serializable math position to bullet math position
            BulletSharp.Math.Vector3 start = startPoint.ToBullet(), end = endPoint.ToBullet();
            var callback = new AllHitsRayResultCallback(start, end);
            // set group mask filter
            callback.CollisionFilterMask = (short)mask;
            world.RayTest(start, end, callback);
            if (callback.HasHit)
            {
                hitPoint = callback.HitPointWorld.ToSerializableArray();
                hitObject = callback.CollisionObjects.SelectToArray(colObj => (CollisionProxy) colObj.UserObject);
            }
            else
            {
                hitPoint = new GameSystem.GameCore.SerializableMath.Vector3[0];
                hitObject = new CollisionProxy[0];
            }
            return callback.HasHit;
        }
        #endregion

        #region Setting methods
        // To do setting methods (Example : gravity, fixedtime ... etc.)
        #endregion
    }
}
