using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.SerializableMath
{
    public static class MathExtension
    {
        public static float Clamp(float value, float min, float max)
        {
            if (max < min) ExMethods.Swap(ref max, ref min);
            if (value <= min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public static Quaternion ToQuaternion(this Matrix4x4 m)
        {
            Quaternion q = new Quaternion();
            float tr = m.m11 + m.m22 + m.m33;
            float s;
            if (tr > 0)
            {
                s = (float)Math.Sqrt(tr + 1) * 2;
                q.w = 0.25f * s;
                q.x = (m.m32 - m.m23) / s;
                q.y = (m.m13 - m.m31) / s;
                q.z = (m.m21 - m.m12) / s;
            }
            else if ((m.m11 > m.m22) && (m.m11 > m.m33))
            {
                s = (float)Math.Sqrt(1 + m.m11 - m.m22 - m.m33) * 2;
                q.w = (m.m32 - m.m23) / s;
                q.x = 0.25f * s;
                q.y = (m.m12 + m.m21) / s;
                q.z = (m.m13 + m.m31) / s;
            }
            else if (m.m22 > m.m33)
            {
                s = (float)Math.Sqrt(1 + m.m22 - m.m11 - m.m33) * 2;
                q.w = (m.m13 - m.m31) / s;
                q.x = (m.m12 + m.m21) / s;
                q.y = 0.25f * s;
                q.z = (m.m23 + m.m32) / s;
            }
            else
            {
                s = (float)Math.Sqrt(1 + m.m33 - m.m11 - m.m22) * 2;
                q.w = (m.m21 - m.m12) / s;
                q.x = (m.m13 + m.m31) / s;
                q.y = (m.m23 + m.m32) / s;
                q.z = 0.25f * s;
            }
            return q;
        }

        public static Matrix4x4 ToMatrix(this Quaternion q)
        {
            Matrix4x4 m = new Matrix4x4();
            float sqw = q.w * q.w;
            float sqx = q.x * q.x;
            float sqy = q.y * q.y;
            float sqz = q.z * q.z;

            float invs = 1 / (sqx + sqy + sqz + sqw);
            m.m11 = (sqx - sqy - sqz + sqw) * invs;
            m.m22 = (-sqx + sqy - sqz + sqw) * invs;
            m.m33 = (-sqx - sqy + sqz + sqw) * invs;

            float tmp1 = q.x * q.y;
            float tmp2 = q.z * q.w;
            m.m21 = 2 * (tmp1 + tmp2) * invs;
            m.m12 = 2 * (tmp1 - tmp2) * invs;

            tmp1 = q.x * q.z;
            tmp2 = q.y * q.w;
            m.m31 = 2 * (tmp1 - tmp2) * invs;
            m.m13 = 2 * (tmp1 + tmp2) * invs;

            tmp1 = q.y * q.z;
            tmp2 = q.x * q.w;
            m.m32 = 2 * (tmp1 + tmp2) * invs;
            m.m23 = 2 * (tmp1 - tmp2) * invs;

            m.m44 = 1;
            return m;
        }
    }
}
