using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class ServerPeer : IPeer, IGroupablePeer
    {
        private NetPeer peer;
        private List<IPeerGroup> groups;

        public ServerPeer(NetPeer peer)
        {
            this.peer = peer;
            groups = new List<IPeerGroup>();
        }

        public int Id { get { return peer.Id; } }

        public bool isConnected { get { return peer.ConnectionState == ConnectionState.Connected; } }

        public object UserObject { get; set; }

        IPeer IGroupablePeer.peer
        {
            get { return this; }
        }

        public void Disconnect()
        {   
            peer.Disconnect();
        }

        public void LeaveGroup(IPeerGroup group)
        {
            group.Exit(this);
        }

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }

        public void TrackGroup(IPeerGroup group)
        {
            groups.Remove(group);
        }
    }
}