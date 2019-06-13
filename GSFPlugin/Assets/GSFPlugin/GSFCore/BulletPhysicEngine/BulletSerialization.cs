using System.Collections.Generic;

namespace BulletEngine
{
    /// <summary>
    /// Converter between Bullet Math and Serialization Math
    /// </summary>
    public static class BulletSerialization
    {
        /// <summary>
        /// Convert from serialized math to bullet
        /// </summary>
        public static BulletSharp.Math.Vector3 ToBullet(this GameSystem.GameCore.SerializableMath.Vector3 v)
        {
            return new BulletSharp.Math.Vector3(v.x, v.y, v.z);
        }

        /// <summary>
        /// Convert from bullet to serialized math
        /// </summary>
        public static GameSystem.GameCore.SerializableMath.Vector3 ToSerializable(this BulletSharp.Math.Vector3 v)
        {
            return new GameSystem.GameCore.SerializableMath.Vector3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Convert from serialized math array to bullet array
        /// </summary>
        public static BulletSharp.Math.Vector3[] ToBullet(this GameSystem.GameCore.SerializableMath.Vector3[] vs)
        {
            BulletSharp.Math.Vector3[] vectors = new BulletSharp.Math.Vector3[vs.Length];
            for(int i = 0; i < vs.Length; i++)
            {
                vectors[i] = vs[i].ToBullet();
            }
            return vectors;
        }

        /// <summary>
        /// Convert from bullet array to serialized math array
        /// </summary>
        public static GameSystem.GameCore.SerializableMath.Vector3[] ToSerializable(this BulletSharp.Math.Vector3[] vs)
        {   
            GameSystem.GameCore.SerializableMath.Vector3[] vectors = new GameSystem.GameCore.SerializableMath.Vector3[vs.Length];
            for (int i = 0; i < vs.Length; i++)
            {
                vectors[i] = vs[i].ToSerializable();
            }
            return vectors;
        }

        /// <summary>
        /// Convert from serialized math list to bullet list
        /// </summary>
        public static List<BulletSharp.Math.Vector3> ToBullet(this List<GameSystem.GameCore.SerializableMath.Vector3> vs)
        {
            List<BulletSharp.Math.Vector3> vectors = new List<BulletSharp.Math.Vector3>(vs.Count);
            for (int i = 0; i < vs.Count; i++)
            {
                vectors.Add(vs[i].ToBullet());
            }
            return vectors;
        }

        /// <summary>
        /// Convert of list from bullet to serialized math
        /// </summary>
        public static List<GameSystem.GameCore.SerializableMath.Vector3> ToSerializable(this List<BulletSharp.Math.Vector3> vs)
        {
            List<GameSystem.GameCore.SerializableMath.Vector3> vectors = new List<GameSystem.GameCore.SerializableMath.Vector3>(vs.Count);
            for (int i = 0; i < vs.Count; i++)
            {
                vectors.Add(vs[i].ToSerializable());
            }
            return vectors;
        }

        /// <summary>
        /// Convert from serialized math list to bullet array
        /// </summary>
        public static BulletSharp.Math.Vector3[] ToBulletArray(this List<GameSystem.GameCore.SerializableMath.Vector3> vs)
        {
            BulletSharp.Math.Vector3[] vectors = new BulletSharp.Math.Vector3[vs.Count];
            for (int i = 0; i < vs.Count; i++)
            {
                vectors[i] = vs[i].ToBullet();
            }
            return vectors;
        }

        /// <summary>
        /// Convert from bullet list to serialized math array
        /// </summary>
        public static GameSystem.GameCore.SerializableMath.Vector3[] ToSerializableArray(this List<BulletSharp.Math.Vector3> vs)
        {
            GameSystem.GameCore.SerializableMath.Vector3[] vectors = new GameSystem.GameCore.SerializableMath.Vector3[vs.Count];
            for (int i = 0; i < vs.Count; i++)
            {
                vectors[i] = vs[i].ToSerializable();
            }
            return vectors;
        }

        /// <summary>
        /// Convert from bullet matrix to serializable math matrix
        /// </summary>
        public static GameSystem.GameCore.SerializableMath.Matrix4x4 ToSerializable(this BulletSharp.Math.Matrix m)
        {   
            return new GameSystem.GameCore.SerializableMath.Matrix4x4(m.ToArray());
        }

        /// <summary>
        /// Convert from serializable math matrix to bullet matrix
        /// </summary>
        public static BulletSharp.Math.Matrix ToBullet(this GameSystem.GameCore.SerializableMath.Matrix4x4 m)
        {
            var bMatrix = new BulletSharp.Math.Matrix(
                m.m11, m.m12, m.m13, m.m14,
                m.m21, m.m22, m.m23, m.m24,
                m.m31, m.m32, m.m33, m.m34,
                m.m41, m.m42, m.m43, m.m44);
            return bMatrix;
        }
    }
}
