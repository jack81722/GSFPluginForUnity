using GameSystem.GameCore.SerializableMath;
using ExCollection;
using System;
using System.Collections.Generic;
using System.Text;
using GameSystem.GameCore.Network;

namespace GameSystem.GameCore
{
    public class GameSource
    {
        #region GSManager properties
        /// <summary>
        /// Manager of this game source
        /// </summary>
        private OnceSetValue<GameSourceManager> _manager;
        public GameSourceManager Manager
        {
            get { return _manager; }
            set { _manager.Value = value; }
        }

        /// <summary>
        /// SID of this game source
        /// </summary>
        private OnceSetValue<uint> _sid;
        public uint SID
        {
            get { return _sid; }
            set { _sid.Value = value; }
        }
        #endregion

        #region Name/Tag/Layer properties
        /// <summary>
        /// Name of game source
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Tag of game source
        /// </summary>
        public virtual string Tag { get; set; } = DefaultTag;
        protected const string DefaultTag = "Default";

        /// <summary>
        /// Layer of game source
        /// </summary>
        public virtual int Layer { get; set; } = DefaultLayer;
        protected const int DefaultLayer = 1;
        #endregion

        /// <summary>
        /// Boolean of game source to executing
        /// </summary>
        public virtual bool executing { get; }

        /// <summary>
        /// Transform of position, rotation, scale
        /// </summary>
        public virtual Transform transform { get; set; }

        /// <summary>
        /// Current game delta time of one frame
        /// </summary>
        public TimeSpan DeltaTime { get { return Manager.DeltaTime; } }

        public Game.ReceiveGamePacketHandler OnReceiveGamePacket { get { return Manager.OnReceiveGamePacket; } set { Manager.OnReceiveGamePacket = value; } }

        #region Constructor
        public GameSource()
        {
            _manager = new OnceSetValue<GameSourceManager>();
            _sid = new OnceSetValue<uint>();
            transform = new Transform();
        }
        #endregion

        #region Update phases
        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        public virtual void OnDestroy() { }
        #endregion

        #region End of frame
        public virtual void OnEndOfFrame() { }
        #endregion

        public static void Destroy(GameSource gs)
        {
            gs.Manager.RemoveGameSource(gs);
        }

        public static int CompareSID(GameSource gs1, GameSource gs2)
        {
            return gs1.SID.CompareTo(gs2.SID);
        }

        public uint GetInstanceID()
        {
            return SID;
        }

        public void Send(int peerID, object obj, Reliability reliability)
        {
            Manager.Send(peerID, obj, reliability);
        }

        public void Broadcast(object obj, Reliability reliability)
        {
            Manager.Broadcast(obj, reliability);
        }

        public JoinGroupRequest[] GetJoinRequests()
        {
            return Manager.GetJoinRequests();
        }

        public int GetGameID()
        {
            return Manager.GetGameID();
        }

        public void CloseGame()
        {
            Manager.CloseGame();
        }

        #region Log methods
        public void Log(object obj)
        {
            Manager.Log(obj);
        }

        public void LogError(object obj)
        {
            Manager.LogError(obj);
        }

        public void LogWarning(object obj)
        {
            Manager.LogWarning(obj);
        }
        #endregion
    }
}
