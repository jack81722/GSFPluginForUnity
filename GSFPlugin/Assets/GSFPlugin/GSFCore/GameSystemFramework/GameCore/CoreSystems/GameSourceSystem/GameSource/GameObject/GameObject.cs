using GameSystem.GameCore.Network;
using GameSystem.GameCore.SerializableMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore
{
    public sealed class GameObject : GameSource
    {
        private ComponentContainer container;

        #region Active properties
        /// <summary>
        /// Set game object state (On/Off)
        /// </summary>
        public bool isActive
        {
            get { return curActive; }
            set
            {
                newActive = value;
                Manager.ChangeExecuteState(this);
            }
        }
        private bool curActive = true;
        private bool newActive;
        public void SetActive(bool active)
        {
            isActive = active;
        }
        #endregion

        public sealed override bool executing { get { return isActive; } }

        // Cannot be called straightly ...
        public GameObject()
        {
            container = new ComponentContainer(this);
        }

        #region Component methods
        /// <summary>
        /// Add new component with type T
        /// </summary>
        /// <typeparam name="T">component type</typeparam>
        public T AddComponent<T>() where T : Component, new()
        {
            return container.AddComponent<T>(Manager);
        }

        /// <summary>
        /// Find component with specific type
        /// </summary>
        public T GetComponent<T>() where T : Component
        {
            return container.GetComponent<T>();
        }

        /// <summary>
        /// Find components with specific type
        /// </summary>
        public Component[] GetComponents<T>() where T : Component
        {
            return container.GetComponents<T>();
        }

        public Component[] GetAllComponents()
        {
            return container.GetAllComponents();
        }
        #endregion

        public sealed override void OnEndOfFrame()
        {
            // refresh adding/removing components
            container.Refresh();
            // change state
            curActive = newActive;
        }

        #region Destroy methods
        /// <summary>
        /// Destroy game object
        /// </summary>
        public static void Destroy(GameObject gameObject)
        {   
            gameObject.container.Clear();
            gameObject.Manager.RemoveGameSource(gameObject);
        }

        /// <summary>
        /// Destroy component
        /// </summary>
        public static void Destroy(Component component)
        {
            if(component.Manager != null)
                component.Manager.RemoveGameSource(component);
            if(component.gameObject != null)
                component.gameObject.container.RemoveComponent(component);
        }
        #endregion

        #region Instantiate method
        public static GameObject Instantiate(GameObject prefab)
        {
            GameObject go = prefab.Manager.Create<GameObject>();
            prefab.container.CopyTo(ref go.container);
            go.Name = $"{prefab.Name} (Clone)";
            go.isActive = prefab.isActive;
            return go;
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position)
        {
            GameObject go = prefab.Manager.Create<GameObject>();
            prefab.container.CopyTo(ref go.container);
            go.Name = $"{prefab.Name} (Clone)";
            go.isActive = prefab.isActive;
            go.transform.position = position;
            return go;
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject go = prefab.Manager.Create<GameObject>();
            prefab.container.CopyTo(ref go.container);
            go.Name = $"{prefab.Name} (Clone)";
            go.isActive = prefab.isActive;
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            GameObject go = prefab.Manager.Create<GameObject>();
            prefab.container.CopyTo(ref go.container);
            go.Name = $"{prefab.Name} (Clone)";
            go.isActive = prefab.isActive;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.scale = scale;
            return go;
        }
        #endregion
    }
}
