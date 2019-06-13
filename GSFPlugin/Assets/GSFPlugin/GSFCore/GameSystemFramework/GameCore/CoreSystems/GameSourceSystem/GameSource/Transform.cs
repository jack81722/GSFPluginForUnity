using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore
{
    public class Transform 
    {
        public Matrix4x4 matrix { get { return pos_mat * scale_mat * rot_mat; } }

        private Matrix4x4 pos_mat;
        private Matrix4x4 rot_mat;
        private Matrix4x4 scale_mat;

        public Vector3 position
        {
            get
            {
                return pos_mat.Origin;
            }
            set
            {  
                pos_mat.Origin = value;
            }
        }
        public Quaternion rotation
        {
            get
            {
                return rot_mat.ToQuaternion();
            }
            set
            {
                rot_mat = value.ToMatrix();
            }
        }
        public Vector3 scale
        {
            get
            {
                return new Vector3(scale_mat.m11, scale_mat.m22, scale_mat.m33);
            }
            set
            {
                scale_mat.m11 = value.x;
                scale_mat.m22 = value.y;
                scale_mat.m33 = value.z;
                scale_mat.m44 = 1;
            }
        }

        public Transform()
        {
            position = new Vector3(0);
            rotation = Quaternion.identity;
            scale = new Vector3(1);
        }

        public Transform(Vector3 position)
        {
            this.position = position;
            rotation = Quaternion.identity;
            scale = new Vector3(1);
        }

        public Transform(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
            scale = new Vector3(1);
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

}
