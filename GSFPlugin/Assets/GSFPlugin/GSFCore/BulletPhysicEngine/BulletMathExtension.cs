using BulletSharp.Math;
using System;

namespace BulletEngine
{

    public static class BulletMathExtension
    {
        const float RadianCoefficient = (float)Math.PI / 180f;

        public static Vector3 Truncate(Vector3 v, float f)
        {
            var n = f;
            n = f / v.Length;
            n = n < 1 ? n : 1;
            return v * n;
        }

        public static Vector3 Normalized(this Vector3 v)
        {
            v.Normalize();
            return v;
        }

        public static Vector3 Negated(this Vector3 v)
        {
            v.Negate();
            return v;
        }

        public static Vector3 RotateOnXY(Vector3 origin, float degree)
        {
            double angle = degree * RadianCoefficient;
            double cos = Math.Cos(angle), sin = Math.Sin(angle);
            float x, y, z;
            x = (float)(origin.X * cos - origin.Z * sin);
            y = (float)(origin.X * sin + origin.Z * cos);
            z = origin.Z;
            return new Vector3(x, y, z);
        }

        public static Vector3 RotateOnYZ(Vector3 origin, float degree)
        {
            double angle = degree * RadianCoefficient;
            double cos = Math.Cos(angle), sin = Math.Sin(angle);
            float x, y, z;
            x = origin.X;
            y = (float)(origin.X * cos - origin.Z * sin);
            z = (float)(origin.X * sin + origin.Z * cos);
            return new Vector3(x, y, z);
        }

        public static Vector3 RotateOnXZ(Vector3 origin, float degree)
        {
            double angle = degree * RadianCoefficient;
            double cos = Math.Cos(angle), sin = Math.Sin(angle);
            float x, y, z;
            x = (float)(origin.X * cos - origin.Z * sin);
            y = origin.Y;
            z = (float)(origin.X * sin + origin.Z * cos);
            return new Vector3(x, y, z);
        }

        public static Vector3 ProjectOnXY(this Vector3 origin)
        {
            origin.Z = 0;
            return origin;
        }

        public static Vector3 ProjectOnYZ(this Vector3 origin)
        {
            origin.X = 0;
            return origin;
        }

        public static Vector3 ProjectOnXZ(this Vector3 origin)
        {
            origin.Y = 0;
            return origin;
        }

        /// <summary>
        /// Calculate dot product between (x1, y1) and (x2, y2)
        /// </summary>
        /// <returns>dot product</returns>
        public static float Dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        /// <summary>
        /// Calculate cross product between (x1, y1) and (x2, y2)
        /// </summary>
        /// <returns>cross product</returns>
        public static float Cross(float x1, float y1, float x2, float y2)
        {
            return x1 * y2 - y1 * x2;
        }

        /// <summary>
        /// Calculate angle (degree) between (x1, y1) and (x2, y2) in [0, 180]
        /// </summary>
        /// <returns>angle (degree)</returns>
        public static float Angle(float x1, float y1, float x2, float y2)
        {
            double dist = Math.Sqrt((x1 * x1 + y1 * y1) * (x2 * x2 + y2 * y2));
            if (dist == 0)
                return 0;
            return (float)(Math.Acos(Dot(x1, y1, x2, y2) / dist) / RadianCoefficient);
        }

        /// <summary>
        /// Calculate angle (degree) between (x1, y1) and (x2, y2) in (0, 180] or (-180, 0]
        /// </summary>
        /// <returns>signed angle (degree)</returns>
        public static float SignedAngle(float x1, float y1, float x2, float y2)
        {
            return Cross(x1, y1, x2, y2) >= 0 ? Angle(x1, y1, x2, y2) : -Angle(x1, y1, x2, y2);
        }

        /// <summary>
        /// Calcuate square magnitude of vector
        /// </summary>
        /// <param name="v">vector</param>
        /// <returns>square magnitude</returns>
        public static float SqrMagnitude(this Vector3 v)
        {
            return v.LengthSquared;
        }

        public static Quaternion ToQuaternion(this Vector3 eulerDegree)
        {
            float heading = eulerDegree.Y * RadianCoefficient;
            float attitude = eulerDegree.Z * RadianCoefficient;
            float bank = eulerDegree.X * RadianCoefficient;
            Quaternion q = new Quaternion();
            double c1 = Math.Cos(heading / 2);
            double s1 = Math.Sin(heading / 2);
            double c2 = Math.Cos(attitude / 2);
            double s2 = Math.Sin(attitude / 2);
            double c3 = Math.Cos(bank / 2);
            double s3 = Math.Sin(bank / 2);
            double c1c2 = c1 * c2;
            double s1s2 = s1 * s2;
            q.W = (float)(c1c2 * c3 - s1s2 * s3);
            q.X = (float)(c1c2 * s3 + s1s2 * c3);
            q.Y = (float)(s1 * c2 * c3 + c1 * s2 * s3);
            q.Z = (float)(c1 * s2 * c3 - s1 * c2 * s3);
            return q;
        }

        public static Vector3 ToEuler(this Quaternion q)
        {
            Vector3 e;
            float sqw = q.W * q.W;
            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;
            float unit = sqx + sqy + sqz + sqw;
            float test = q.X * q.Y + q.Z * q.W;
            if (test > 0.499f * unit)
            {
                e.Y = 2 * (float)Math.Atan2(q.X, q.W);
                e.Z = (float)Math.PI / 2;
                e.X = 0;
                return e;
            }
            if (test < -0.499f * unit)
            {
                e.Y = -2 * (float)Math.Atan2(q.X, q.W);
                e.Z = -(float)Math.PI / 2;
                e.X = 0;
                return e;
            }
            e.Y = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);
            e.Z = (float)Math.Asin(2 * test / unit);
            e.X = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);
            return e;
        }

        public static Vector3 ToEulerDegree(this Quaternion q)
        {
            Vector3 e;
            float sqw = q.W * q.W;
            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;
            float unit = sqx + sqy + sqz + sqw;
            float test = q.X * q.Y + q.Z * q.W;
            if (test > 0.499f * unit)
            {
                e.Y = 2 * (float)Math.Atan2(q.X, q.W) / RadianCoefficient;
                e.Z = (float)Math.PI / 2 / RadianCoefficient;
                e.X = 0;
                return e;
            }
            if (test < -0.499f * unit)
            {
                e.Y = -2 * (float)Math.Atan2(q.X, q.W) / RadianCoefficient;
                e.Z = -(float)Math.PI / 2 / RadianCoefficient;
                e.X = 0;
                return e;
            }
            e.Y = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw) / RadianCoefficient;
            e.Z = (float)Math.Asin(2 * test / unit) / RadianCoefficient;
            e.X = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw) / RadianCoefficient;
            return e;
        }

        public static Quaternion ToQuaternion(this Matrix m)
        {
            Quaternion q = new Quaternion();
            float tr = m.M11 + m.M22 + m.M33;
            float s;
            if (tr > 0)
            {
                s = (float)Math.Sqrt(tr + 1) * 2;
                q.W = 0.25f * s;
                q.X = (m.M32 - m.M23) / s;
                q.Y = (m.M13 - m.M31) / s;
                q.Z = (m.M21 - m.M12) / s;
            }
            else if ((m.M11 > m.M22) && (m.M11 > m.M33))
            {
                s = (float)Math.Sqrt(1 + m.M11 - m.M22 - m.M33) * 2;
                q.W = (m.M32 - m.M23) / s;
                q.X = 0.25f * s;
                q.Y = (m.M12 + m.M21) / s;
                q.Z = (m.M13 + m.M31) / s;
            }
            else if (m.M22 > m.M33)
            {
                s = (float)Math.Sqrt(1 + m.M22 - m.M11 - m.M33) * 2;
                q.W = (m.M13 - m.M31) / s;
                q.X = (m.M12 + m.M21) / s;
                q.Y = 0.25f * s;
                q.Z = (m.M23 + m.M32) / s;
            }
            else
            {
                s = (float)Math.Sqrt(1 + m.M33 - m.M11 - m.M22) * 2;
                q.W = (m.M21 - m.M12) / s;
                q.X = (m.M13 + m.M31) / s;
                q.Y = (m.M23 + m.M32) / s;
                q.Z = 0.25f * s;
            }
            return q;
        }

        public static Matrix ToMatrix(this Quaternion q)
        {
            Matrix m = new Matrix();
            float sqw = q.W * q.W;
            float sqx = q.X * q.X;
            float sqy = q.Y * q.Y;
            float sqz = q.Z * q.Z;

            float invs = 1 / (sqx + sqy + sqz + sqw);
            m.M11 = (sqx - sqy - sqz + sqw) * invs;
            m.M22 = (-sqx + sqy - sqz + sqw) * invs;
            m.M33 = (-sqx - sqy + sqz + sqw) * invs;

            float tmp1 = q.X * q.Y;
            float tmp2 = q.Z * q.W;
            m.M21 = 2 * (tmp1 + tmp2) * invs;
            m.M12 = 2 * (tmp1 - tmp2) * invs;

            tmp1 = q.X * q.Z;
            tmp2 = q.Y * q.W;
            m.M31 = 2 * (tmp1 - tmp2) * invs;
            m.M13 = 2 * (tmp1 + tmp2) * invs;

            tmp1 = q.Y * q.Z;
            tmp2 = q.X * q.W;
            m.M32 = 2 * (tmp1 + tmp2) * invs;
            m.M23 = 2 * (tmp1 - tmp2) * invs;

            m.M44 = 1;
            return m;
        }

        public static string ToStr(this Vector3 v)
        {
            return string.Format("({0:0.00}, {1:0.00}, {2:0.00})", v.X, v.Y, v.Z);
        }
    }
}
