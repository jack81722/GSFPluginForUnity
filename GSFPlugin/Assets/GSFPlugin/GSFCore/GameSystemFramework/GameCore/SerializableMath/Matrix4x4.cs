using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore.SerializableMath
{
    [Serializable]
    public struct Matrix4x4
    {
        public float m11, m12, m13, m14,
                     m21, m22, m23, m24,
                     m31, m32, m33, m34,
                     m41, m42, m43, m44;
        public Vector3 Origin
        {
            get { return new Vector3(m41, m42, m43); }
            set
            {
                m11 = 1; m12 = 0; m13 = 0; m14 = 0;
                m21 = 0; m22 = 1; m23 = 0; m24 = 0;
                m31 = 0; m32 = 0; m33 = 1; m34 = 0;
                m41 = value.x; m42 = value.y; m43 = value.z; m44 = 1;
            }
        }

        public static readonly Matrix4x4 zero = new Matrix4x4(0);
        public static readonly Matrix4x4 identity = new Matrix4x4()
        {
            m11 = 1, m12 = 0, m13 = 0, m14 = 0,
            m21 = 0, m22 = 1, m23 = 0, m24 = 0,
            m31 = 0, m32 = 0, m33 = 1, m34 = 0,
            m41 = 0, m42 = 0, m43 = 0, m44 = 1
        };

        public Matrix4x4(float value)
        {
            m11 = m12 = m13 = m14 = m21 = m22 = m23 = m24 = m31 = m32 = m33 = m34 = m41 = m42 = m43 = m44 = value;
        }

        public Matrix4x4(float[] values)
        {
            if (values == null)
                throw new InvalidOperationException("Values cannot be null.");
            if (values.Length != 16)
                throw new InvalidOperationException("Number of matrix elements must be 16.");
            m11 = values[0]; m12 = values[0]; m13 = values[0]; m14 = values[0];
            m21 = values[0]; m22 = values[0]; m23 = values[0]; m24 = values[0];
            m31 = values[0]; m32 = values[0]; m33 = values[0]; m34 = values[0];
            m41 = values[0]; m42 = values[0]; m43 = values[0]; m44 = values[0];
        }

        public Matrix4x4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            this.m11 = m11; this.m12 = m12; this.m13 = m13; this.m14 = m14;
            this.m21 = m21; this.m22 = m22; this.m23 = m23; this.m24 = m24;
            this.m31 = m31; this.m32 = m32; this.m33 = m33; this.m34 = m34;
            this.m41 = m41; this.m42 = m42; this.m43 = m43; this.m44 = m44;
        }

        public float[] ToArray()
        {
            return new float[]
            {
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            };
        }

        public void Plus(Matrix4x4 m)
        {
            m11 = m11 + m.m11; m12 = m12 + m.m12; m13 = m13 + m.m13; m14 = m14 + m.m14;
            m21 = m21 + m.m21; m22 = m22 + m.m22; m23 = m23 + m.m23; m24 = m24 + m.m24;
            m31 = m31 + m.m31; m32 = m32 + m.m32; m33 = m33 + m.m33; m34 = m34 + m.m34;
            m41 = m41 + m.m41; m42 = m42 + m.m42; m43 = m43 + m.m43; m44 = m44 + m.m44;
        }

        public void Sub(Matrix4x4 m)
        {
            m11 = m11 - m.m11; m12 = m12 - m.m12; m13 = m13 - m.m13; m14 = m14 - m.m14;
            m21 = m21 - m.m21; m22 = m22 - m.m22; m23 = m23 - m.m23; m24 = m24 - m.m24;
            m31 = m31 - m.m31; m32 = m32 - m.m32; m33 = m33 - m.m33; m34 = m34 - m.m34;
            m41 = m41 - m.m41; m42 = m42 - m.m42; m43 = m43 - m.m43; m44 = m44 - m.m44;
        }

        public void Scale(float scale)
        {
            m11 = m11 * scale; m12 = m12 * scale; m13 = m13 * scale; m14 = m14 * scale;
            m21 = m21 * scale; m22 = m22 * scale; m23 = m23 * scale; m24 = m24 * scale;
            m31 = m31 * scale; m32 = m32 * scale; m33 = m33 * scale; m34 = m34 * scale;
            m41 = m41 * scale; m42 = m42 * scale; m43 = m43 * scale; m44 = m44 * scale;
        }

        public void Divide(float f)
        {
            m11 = m11 / f; m12 = m12 / f; m13 = m13 / f; m14 = m14 / f;
            m21 = m21 / f; m22 = m22 / f; m23 = m23 / f; m24 = m24 / f;
            m31 = m31 / f; m32 = m32 / f; m33 = m33 / f; m34 = m34 / f;
            m41 = m41 / f; m42 = m42 / f; m43 = m43 / f; m44 = m44 / f;
        }

        public void Product(Matrix4x4 m)
        {
            m11 = m11 * m.m11 + m12 * m.m21 + m13 * m.m31 + m14 * m.m41;
            m12 = m11 * m.m12 + m12 * m.m22 + m13 * m.m32 + m14 * m.m42;
            m13 = m11 * m.m13 + m12 * m.m23 + m13 * m.m33 + m14 * m.m43;
            m14 = m11 * m.m14 + m12 * m.m24 + m13 * m.m34 + m14 * m.m44;
            m21 = m21 * m.m11 + m22 * m.m21 + m23 * m.m31 + m24 * m.m41;
            m22 = m21 * m.m12 + m22 * m.m22 + m23 * m.m32 + m24 * m.m42;
            m23 = m21 * m.m13 + m22 * m.m23 + m23 * m.m33 + m24 * m.m43;
            m24 = m21 * m.m14 + m22 * m.m24 + m23 * m.m34 + m24 * m.m44;
            m31 = m31 * m.m11 + m32 * m.m21 + m33 * m.m31 + m34 * m.m41;
            m32 = m31 * m.m12 + m32 * m.m22 + m33 * m.m32 + m34 * m.m42;
            m33 = m31 * m.m13 + m32 * m.m23 + m33 * m.m33 + m34 * m.m43;
            m34 = m31 * m.m14 + m32 * m.m24 + m33 * m.m34 + m34 * m.m44;
            m41 = m41 * m.m11 + m42 * m.m21 + m43 * m.m31 + m44 * m.m41;
            m42 = m41 * m.m12 + m42 * m.m22 + m43 * m.m32 + m44 * m.m42;
            m43 = m41 * m.m13 + m42 * m.m23 + m43 * m.m33 + m44 * m.m43;
            m44 = m41 * m.m14 + m42 * m.m24 + m43 * m.m34 + m44 * m.m44;
        }

        public void Transpose()
        {
            ExMethods.Swap(ref m12, ref m21); ExMethods.Swap(ref m13, ref m31); ExMethods.Swap(ref m14, ref m41);
            ExMethods.Swap(ref m23, ref m32); ExMethods.Swap(ref m24, ref m42); ExMethods.Swap(ref m34, ref m43);
        }

        public static Matrix4x4 Translation(Vector3 v)
        {
            Matrix4x4 m = identity;
            m.m14 = v.x;
            m.m24 = v.y;
            m.m34 = v.z;
            return m;
        }

        public static Matrix4x4 Scaling(Vector3 v)
        {
            Matrix4x4 m = identity;
            m.m11 = v.x;
            m.m22 = v.y;
            m.m33 = v.z;
            return m;
        }

        public static Matrix4x4 operator +(Matrix4x4 m1, Matrix4x4 m2)
        {
            m1.Plus(m2);
            return m1;
        }

        public static Matrix4x4 operator -(Matrix4x4 m1, Matrix4x4 m2)
        {
            m1.Plus(m2 * -1);
            return m1;
        }

        public static Matrix4x4 operator *(Matrix4x4 m1, Matrix4x4 m2)
        {
            m1.Product(m2);
            return m1;
        }

        public static Matrix4x4 operator *(Matrix4x4 m, float f)
        {
            m.Scale(f);
            return m;
        }

        public static Matrix4x4 operator /(Matrix4x4 m, float f)
        {
            m.Divide(f);
            return m;
        }

        public override string ToString()
        {
            return string.Format(
                "[{0}, {1}, {2}, {3}][{4}, {5}, {6}, {7}][{8}, {9}, {10}, {11}][{12}, {13}, {14}, {15}]",
                m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);
        }
    }
}
