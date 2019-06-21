using GameSystem.GameCore.Physics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ExCollection;
using System.Reflection;
using GameSystem.GameCore.Debugger;
using AdvancedGeneric;
using GameSystem.GameCore.Network;

namespace GameSystem.GameCore
{
    public class GameSourceManager : IGameModule
    {   
        /// <summary>
        /// All game sources
        /// </summary>
        private AutoSortList<GameSource> GSList;
        private AutoSortList<GameSourceAdapter> GSAdapterList;
        private List<IGSVisitor> visitors;

        /// <summary>
        /// Source water id
        /// </summary>
        private uint serialSID;

        public bool updating { get; private set; }
        public TimeSpan DeltaTime { get; private set; }

        #region Temp of add/remove game sources
        // temp of added/removed game source which would execute start/destroy phase
        private List<GameSource> added;
        private List<GameSource> removed;

        // temp of adding/removing game sources for adding/removing 
        private List<GameSource> temp_adding;
        private List<GameSource> temp_removing;
        #endregion

        // temp of state refreshing game sources
        private List<GameSource> temp_stateUpdating;

        public PhysicEngineProxy PhysicEngine { get; private set; }

        private IDebugger Debugger;
        private Game game;

        #region Constructor
        public GameSourceManager(Game game, PhysicEngineProxy physicEngine, IDebugger debugger)
        {
            serialSID = 0;
            //GSList = new KeyedList<uint, GameSource>();
            GSList = new AutoSortList<GameSource>(GameSource.CompareSID);
            GSAdapterList = new AutoSortList<GameSourceAdapter>(GameSourceAdapter.CompareSID);
            visitors = new List<IGSVisitor>();
            
            added = new List<GameSource>();
            removed = new List<GameSource>();

            temp_adding = new List<GameSource>();
            temp_removing = new List<GameSource>();

            temp_stateUpdating = new List<GameSource>();

            PhysicEngine = physicEngine;
            Debugger = debugger;
            this.game = game;
        }
        #endregion

        /// <summary>
        /// Get primary SID
        /// </summary>
        public uint NewSID()
        {
            // what will happen if overflow
            uint id = serialSID++;
            //while (GSList.ContainsKey(id))
            //    id = serialSID++;
            return id;
        }

        #region IGameModule methods
        public void Initialize()
        {
            
        }

        public void Update(TimeSpan deltaTime)
        {
            DeltaTime = deltaTime;
            updating = true;
            // start phase (only process non-started game source)
            _startPhase();

            // update phase
            _updatePhase();

            // late update phase
            _lateupdatePhase();
            updating = false;

            // refresh game source every end of frame
            RefreshGS();

            // execute OnDestroy method after destroy
            _onDestroyPhase();
        }
        #endregion

        #region Phase methods
        private void _startPhase()
        {
            for (int i = 0; i < added.Count; i++)
            {
                try
                {
                    added[i].Start();
                }
                catch (Exception e)
                {
                    Debugger.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                }
            }
            
            added.Clear();
        }

        private void _updatePhase()
        {
            for (int i = 0; i < GSList.Count; i++)
            {
                // need try/catch ...
                try
                {
                    if (GSList[i].executing)
                        GSList[i].Update();
                }
                catch (Exception e)
                {
                    Debugger.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                }
            }
        }

        private void _lateupdatePhase()
        {
            for (int i = 0; i < GSList.Count; i++)
            {
                try
                {
                    if (GSList[i].executing)
                        GSList[i].LateUpdate();
                }
                catch (Exception e)
                {
                    Debugger.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                }
            }
        }

        private void _onDestroyPhase()
        {
            for (int i = 0; i < removed.Count; i++)
            {
                try
                {
                    removed[i].OnDestroy();
                }
                catch (Exception e)
                {
                    Debugger.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                }
            }
            removed.Clear();
        }
        #endregion

        /// <summary>
        /// Execute while end of frame
        /// </summary>
        private void RefreshGS()
        {   
            // refresh adding game source
            _addGSRange(temp_adding);
            temp_adding.Clear();

            // refresh state updating game source
            for (int i = 0; i < temp_stateUpdating.Count; i++)
                temp_stateUpdating[i].OnEndOfFrame();
            temp_stateUpdating.Clear();

            // refresh removing game source
            _removeGSRange(temp_removing);
            temp_removing.Clear();

            
            for(int i = 0; i < visitors.Count; i++)
            {
                //Log("Send to visitor count = " + GSAdapterList.Count);
                visitors[i].GetGSList(GSAdapterList.ToList());
            }
        }

        public void ChangeExecuteState(GameSource source)
        {
            temp_stateUpdating.Add(source);
        }

        #region Add/Remove GS
        /// <summary>
        /// Add gamer source
        /// </summary>
        /// <param name="source">game source</param>
        public void AddGameSource(GameSource source)
        {
            if (updating)
            {
                lock (temp_adding)
                    temp_adding.Add(source);
            }
            else
                _addGS(source);
        }

        private void _addGS(GameSource source)
        {
            lock (GSList)
            {
                if (!GSList.TryAdd(source)) return;
                added.Add(source);
                GSAdapterList.Add(new GameSourceAdapter(source));
            }
        }

        private void _addGSRange(List<GameSource> sources)
        {
            lock (GSList)
            {
                var newSources = GSList.UnionWith(sources);
                added.AddRange(newSources);

                GameSourceAdapter[] adapters = new GameSourceAdapter[newSources.Count];
                for(int i = 0; i < newSources.Count; i++)
                    adapters[i] = new GameSourceAdapter(newSources[i]);
                GSAdapterList.UnionWith(adapters);
            }
        }

        /// <summary>
        /// Remove game source
        /// </summary>
        /// <param name="source">game source</param>
        public void RemoveGameSource(GameSource source)
        {
            if (updating)
            {
                lock (temp_removing)
                    temp_removing.Add(source);
            }
            else
                _removeGS(source);
        }

        private void _removeGS(GameSource source)
        {
            lock (GSList)
            {
                GSList.Remove(source);
                removed.Add(source);

                GSAdapterList.Remove(source.SID, (sid, adapter) => ((uint)sid).CompareTo(adapter.SID));
            }
        }

        private void _removeGSRange(IEnumerable<GameSource> sources)
        {
            lock (GSList)
            {
                GSList.ExceptWith(sources);

                List<uint> sids = new List<uint>();
                foreach (var source in sources)
                    sids.Add(source.SID);
                var adapters = GSAdapterList.FindAll(sids.ToArray(), (sid, adapter) => sid.CompareTo(adapter.SID));
                GSAdapterList.ExceptWith(adapters);
            }
        }
        #endregion

        public T Create<T>() where T : GameSource, new()
        {
            T source = new T() { Manager = this, SID = NewSID() };
            AddGameSource(source);
            return source;
        }

        Queue<Action> endframeActions = new Queue<Action>();
        public void StartTask(Action action)
        {
            endframeActions.Enqueue(action);
        }

        public void ExecuteEndOfFrameTasks()
        {
            while (endframeActions.Count > 0)
                endframeActions.Dequeue().Invoke();
        }

        public void Clear()
        {
            lock(GSList)
                GSList.Clear();
        }

        #region Find object methods
        public T FindObjectOfType<T>() where T : GameSource
        {
            return (T)GSList.Find(gs => gs.GetType() == typeof(T));
        }

        public IEnumerable<T> FindObjectsOfType<T>() where T : GameSource
        {
            return GSList.FindAll(gs => gs.GetType() == typeof(T)).Select(gs => (T)gs);
        }
        #endregion

        #region Log methods
        public void Log(object obj)
        {
            Debugger.Log(obj);
        }

        public void LogError(object obj)
        {
            Debugger.LogError(obj);
        }

        public void LogWarning(object obj)
        {
            Debugger.LogWarning(obj);
        }
        #endregion

        #region Network methods
        public void Send(int peerID, object obj, Reliability reliability)
        {
            game.Send(peerID, obj, reliability);
        }

        public void Broadcast(object obj, Reliability reliability)
        {
            game.Broadcast(obj, reliability);
        }

        public JoinGroupRequest[] GetJoinRequests()
        {
            return game.GetJoinPeerList().ToArray();
        }
        #endregion

        public int GetGameID()
        {
            return game.GetGameID();
        }
    }
}
