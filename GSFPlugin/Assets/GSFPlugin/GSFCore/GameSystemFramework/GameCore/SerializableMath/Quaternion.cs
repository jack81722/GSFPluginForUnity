using GameSystem.GameCore.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.SerializableMath
{
    [Serializable]
    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vector3 axis, float angle)
        {
            w = (float)Math.Cos(0.5f * angle);
            float s = (float)Math.Sin(0.5f * angle);
            x = axis.x * s;
            y = axis.y * s;
            z = axis.z * s;
        }

        public static readonly Quaternion identity = new Quaternion(0, 0, 0, 1);

        public void Plus(Quaternion q)
        {
            x += q.x;
            y += q.x;
            z += q.z;
            w += q.w;
        }

        public void Sub(Quaternion q)
        {
            x -= q.x;
            y -= q.y;
            z -= q.z;
            w -= q.w;
        }

        public void Mul(float f)
        {
            x *= f;
            y *= f;
            z *= f;
            w *= f;
        }

        public void Normalize()
        {
            float len = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
            x /= len;
            y /= len;
            z /= len;
            w /= len;
        }

        public void Invert()
        {
            x *= -1;
            y *= -1;
            z *= -1;
        }

        public float Dot(Quaternion q)
        {
            return x * q.x + y * q.y + z * q.z + w * q.w;
        }

        public void Project(Quaternion onto)
        {
            float dot = Dot(onto);
            x = dot * onto.x;
            y = dot * onto.y;
            z = dot * onto.z;
            w = dot * onto.w;
        }

        public void Product(Quaternion rhs)
        {
            Vector3 vThis = new Vector3(x, y, z);
            Vector3 vRhs = new Vector3(rhs.x, rhs.y, rhs.z);
            w = w * rhs.w - vThis.Dot(vRhs);

            Vector3 newV =
                rhs.w * vThis + w * vRhs + Vector3.Cross(vThis, vRhs);
            x = newV.x;
            y = newV.y;
            z = newV.z;
        }

        public Vector3 ToEuler(Quaternion q)
        {
            Vector3 e;
            float sqw = q.w * q.w;
            float sqx = q.x * q.x;
            float sqy = q.y * q.y;
            float sqz = q.z * q.z;
            float unit = sqx + sqy + sqz + sqw;
            float test = q.x * q.y + q.z * q.w;
            if (test > 0.499f * unit)
            {
                e.y = 2 * (float)Math.Atan2(q.x, q.w);
                e.z = (float)Math.PI / 2;
                e.x = 0;
                return e;
            }
            if (test < -0.499f * unit)
            {
                e.y = -2 * (float)Math.Atan2(q.x, q.w);
                e.z = -(float)Math.PI / 2;
                e.x = 0;
                return e;
            }
            e.y = (float)Math.Atan2(2 * q.y * q.w - 2 * q.x * q.z, sqx - sqy - sqz + sqw);
            e.z = (float)Math.Asin(2 * test / unit);
            e.x = (float)Math.Atan2(2 * q.x * q.w - 2 * q.y * q.z, -sqx + sqy - sqz + sqw);
            return e;
        }

        public Vector3 ToEulerDegree(Quaternion q)
        {
            float toRadian = (float)(Math.PI / 180.0);

            Vector3 e;
            float sqw = q.w * q.w;
            float sqx = q.x * q.x;
            float sqy = q.y * q.y;
            float sqz = q.z * q.z;
            float unit = sqx + sqy + sqz + sqw;
            float test = q.x * q.y + q.z * q.w;
            if (test > 0.499f * unit)
            {
                e.y = 2 * (float)Math.Atan2(q.x, q.w) / toRadian;
                e.z = (float)Math.PI / 2 / toRadian;
                e.x = 0;
                return e;
            }
            if (test < -0.499f * unit)
            {
                e.y = -2 * (float)Math.Atan2(q.x, q.w) / toRadian;
                e.z = -(float)Math.PI / 2 / toRadian;
                e.x = 0;
                return e;
            }
            e.y = (float)Math.Atan2(2 * q.y * q.w - 2 * q.x * q.z, sqx - sqy - sqz + sqw) / toRadian;
            e.z = (float)Math.Asin(2 * test / unit) / toRadian;
            e.x = (float)Math.Atan2(2 * q.x * q.w - 2 * q.y * q.z, -sqx + sqy - sqz + sqw) / toRadian;
            return e;
        }

        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            q1.Plus(q2);
            return q1;
        }

        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            q1.Sub(q2);
            return q1;
        }

        public static Quaternion operator *(Quaternion q, float f)
        {
            q.Mul(f);
            return q;
        }

        public static Quaternion operator *(float f, Quaternion q)
        {
            q.Mul(f);
            return q;
        }

        public static Quaternion operator *(Quaternion q, double f)
        {
            q.Mul((float)f);
            return q;
        }

        public static Quaternion operator *(double f, Quaternion q)
        {
            q.Mul((float)f);
            return q;
        }

        public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
        {
            float omega = (float)Math.Acos(MathExtension.Clamp(from.Dot(to), -1f, 1f));
            float sin_inv = 1f / (float)Math.Sin(omega);

            return Math.Sin((1f - t) * omega) * sin_inv * from + Math.Sin(t * omega) * sin_inv * to;
        }

        public static Quaternion Euler(Vector3 euler)
        {
            float heading = euler.y;
            float attitude = euler.z;
            float bank = euler.x;
            Quaternion q = new Quaternion();
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            q.w = (float)(c1c2 * c3 - s1s2 * s3);
            q.x = (float)(c1c2 * s3 + s1s2 * c3);
            q.y = (float)(s1 * c2 * c3 + c1 * s2 * s3);
            q.z = (float)(c1 * s2 * c3 - s1 * c2 * s3);
            return q;
        }

        public static Quaternion EulerDegree(Vector3 eulerDegree)
        {
            float toRadian = (float)(Math.PI / 180.0);

            float heading = eulerDegree.y * toRadian;
            float attitude = eulerDegree.z * toRadian;
            float bank = eulerDegree.x * toRadian;
            Quaternion q = new Quaternion();
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            q.w = (float)(c1c2 * c3 - s1s2 * s3);
            q.x = (float)(c1c2 * s3 + s1s2 * c3);
            q.y = (float)(s1 * c2 * c3 + c1 * s2 * s3);
            q.z = (float)(c1 * s2 * c3 - s1 * c2 * s3);
            return q;
        }
    }
}
