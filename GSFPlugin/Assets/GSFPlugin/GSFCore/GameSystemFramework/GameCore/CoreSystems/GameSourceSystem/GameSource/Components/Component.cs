using System;
using System.Collections.Generic;
using System.Text;
using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;

namespace GameSystem.GameCore
{
    public class Component : GameSource
    {
        #region Enable properties
        private bool curEnable = true;
        private bool newEnable;
        public bool enable
        {
            get { return curEnable; }
            set
            {
                newEnable = value;
                Manager.ChangeExecuteState(this);
            }
        }

        public void SetEnable(bool isEnable)
        {
            enable = isEnable;
        }
        #endregion

        public sealed override bool executing { get { return (gameObject != null ? gameObject.isActive : false) && enable; } }

        public GameObject gameObject { get; set; }
        private uint GoInstID { get { return gameObject.GetInstanceID(); } }
        public sealed override string Name { get { return gameObject.Name; } set { gameObject.Name = value; } }
        public sealed override string Tag { get { return gameObject.Tag; } set { gameObject.Tag = value; } }
        public sealed override int Layer { get { return gameObject.Layer; } set { gameObject.Layer = value; } }

        public Component() { }

        public sealed override void OnEndOfFrame()
        {
            curEnable = newEnable;
        }

        /// <summary>
        /// Find component by specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        #region Create/Instantiate methods
        /// <summary>
        /// Create empty game object
        /// </summary>
        public GameObject CreateGameObject()
        {
            return Manager.Create<GameObject>();
        }

        public static GameObject Instantiate(GameObject prefab)
        {
            return GameObject.Instantiate(prefab);
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position)
        {
            return GameObject.Instantiate(prefab, position);
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return GameObject.Instantiate(prefab, position, rotation);
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return GameObject.Instantiate(prefab, position, rotation, scale);
        }
        #endregion

        #region Destroy methods
        public static void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }

        public static void Destroy(Component component)
        {
            GameObject.Destroy(component);
        }
        #endregion
    }
}
