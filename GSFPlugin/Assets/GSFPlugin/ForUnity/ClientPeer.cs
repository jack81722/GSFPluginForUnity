using GameSystem.GameCore.Debugger;
using GameSystem.GameCore.Network;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    public class ClientPeer : IPeer
    {
        protected EventBasedNetListener listener;
        protected NetManager netManager;
        protected NetPeer peer;
        protected ISerializer serializer;
        protected IDebugger debugger;

        #region Peer information
        public string DestinationIP
        {
            get { return peer.EndPoint.Address.ToString(); }
        }
        public int Port
        {
            get { return peer.EndPoint.Port; }
        }
        #endregion

        public delegate void ClientReceiveHandler(object packet, Reliability reliability);
        public ClientReceiveHandler OnClientReceivePacket;

        #region IPeer properties
        public int Id { get { return peer.Id; } }
        public bool isConnected {
            get
            {
                return peer != null && peer.ConnectionState == ConnectionState.Connected;
            }
        }
        public object UserObject { get; set; }
        public PeerDisconnectedHandler OnPeerDisconnected { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        #endregion

        public ClientPeer(ISerializer serializer, IDebugger debugger) : base()
        {
            this.serializer = serializer;
            this.debugger = debugger;
            listener = new EventBasedNetListener();
            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;
            netManager = new NetManager(listener);
            netManager.Start();
        }

        private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte[] dgram = new byte[reader.AvailableBytes];
            reader.GetBytes(dgram, dgram.Length);
            object packet = serializer.Deserialize(dgram);
            OnReceivePacket(packet, (Reliability)deliveryMethod);
        }

        protected virtual void OnReceivePacket(object packet, Reliability reliability)
        {
            try
            {
                OnClientReceivePacket.Invoke(packet, reliability);
            }
            catch
            {
                // log exception ...
            }
        }

        public void Connect(string ipAddr, int port, string connectKey)
        {
            debugger.Log($"Connect to [{ipAddr}:{port}, \"{connectKey}\"]");
            peer = netManager.Connect(ipAddr, port, connectKey);
            peer.Tag = this;
        }

        public void Send(object packet, Reliability reliability)
        {
            byte[] dgram = serializer.Serialize(packet);
            peer.Send(dgram, (DeliveryMethod)reliability);
        }

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }

        public void Poll()
        {
            netManager.PollEvents();
        }

        public void Disconnect()
        {
            netManager.DisconnectAll();
        }

        public void TrackGroup(IPeerGroup group)
        {
            throw new System.NotImplementedException();
        }

        public void UntrackGroup(IPeerGroup group)
        {
            throw new System.NotImplementedException();
        }
    }
}