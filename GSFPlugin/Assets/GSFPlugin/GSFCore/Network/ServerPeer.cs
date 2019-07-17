using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class ServerPeer : IPeer
    {
        private NetPeer peer;
        private List<IPeerGroup> groups;

        public PeerDisconnectedHandler OnPeerDisconnected { get; set; }

        public ServerPeer(NetPeer peer)
        {
            this.peer = peer;
            groups = new List<IPeerGroup>();
        }

        public int Id { get { return peer.Id; } }

        public bool isConnected { get { return peer.ConnectionState == ConnectionState.Connected; } }

        public object UserObject { get; set; }
        

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }

        public void Disconnect()
        {   
            var groupArray = groups.ToArray();
            foreach(var group in groupArray)
            {
                UntrackGroup(group);
            }
            // positively disconnect only if peer is connected
            if(isConnected)
                peer.Disconnect();

            if (OnPeerDisconnected != null)
                OnPeerDisconnected.Invoke(this);
        }

        public void UntrackGroup(IPeerGroup group)
        {
            if (group.ContainsPeer(Id))
            {
                group.Exit(this, null);
            }
            lock(groups)
                groups.Remove(group);
        }

        public void TrackGroup(IPeerGroup group)
        {
            if (!group.ContainsPeer(Id))
            {
                throw new InvalidOperationException("Peer must join group before tracking.");
            }
            lock (groups)
            {
                if (!groups.Contains(group))
                    groups.Add(group);
            }
        }
    }
}