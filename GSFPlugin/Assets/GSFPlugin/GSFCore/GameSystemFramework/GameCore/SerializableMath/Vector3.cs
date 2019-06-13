using GameSystem.GameCore.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.SerializableMath
{
    [Serializable]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public static readonly Vector3 Zero = new Vector3(0);
        public static readonly Vector3 One = new Vector3(1);

        public Vector3(float value)
        {
            x = y = z = value;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Dot(Vector3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public void Cross(Vector3 v)
        {
            x = y * v.z - z * v.y;
            y = z * v.x - x * v.z;
            z = x * v.y - y * v.z;
        }

        public void Plus(Vector3 v)
        {
            x += v.x;
            y += v.y;
            z += v.z;
        }

        public void Sub(Vector3 v)
        {
            x -= v.x;
            y -= v.y;
            z -= v.z;
        }

        public void Scale(float f)
        {
            x *= f;
            y *= f;
            z *= f;
        }

        public void Divide(float f)
        {
            x /= f;
            y /= f;
            z /= f;
        }

        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.z);
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            v1.Plus(v2);
            return v1;
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            v1.Sub(v2);
            return v1;
        }

        public static Vector3 operator *(Vector3 v, float f)
        {
            v.Scale(f);
            return v;
        }

        public static Vector3 operator /(Vector3 v, float f)
        {
            v.Divide(f);
            return v;
        }

        public static Vector3 operator *(float f, Vector3 v)
        {
            v.Scale(f);
            return v;
        }

        public static Vector3 operator /(float f, Vector3 v)
        {
            v.Divide(f);
            return v;
        }

        public override string ToString()
        {
            return string.Format("({0:0.00}, {1:0.00}, {2:0.00})", x, y, z);
        }
    }
}
