using GameSystem.GameCore.Debugger;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem.GameCore
{
    public abstract class SceneBuilder
    {
        private IDebugger debugger;
        private Scene scene;

        public SceneBuilder(IDebugger debugger)
        {   
            this.debugger = debugger;
        }

        public void Build(Scene scene)
        {
            this.scene = scene;
            Building();
        }

        protected abstract void Building();

        protected GameObject CreateGameObject(string name = "GameObject")
        {
            var go = scene.GSManager.Create<GameObject>();
            go.Name = name;
            return go;
        }

        protected GameObject Instantiate(GameObject prefab)
        {
            return GameObject.Instantiate(prefab);
        }

        public void Log(object obj)
        {
            debugger.Log(obj);
        }

        public void LogError(object obj)
        {
            debugger.LogError(obj);
        }

        public void LogWarning(object obj)
        {
            debugger.LogWarning(obj);
        }
    }
}
