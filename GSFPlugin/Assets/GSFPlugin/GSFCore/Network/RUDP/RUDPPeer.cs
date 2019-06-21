using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class RUDPPeer : Peer
    {
        private NetPeer peer;

        public RUDPPeer(NetPeer peer)
        {
            this.peer = peer;
        }

        public override int Id { get { return peer.Id; } }
        public override bool isConnecting { get { return peer.ConnectionState != ConnectionState.Disconnected; } }

        public override void Disconnect()
        {   
            peer.Disconnect();
        }

        public override void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }
    }
}