﻿using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class RUDPPeer : IPeer
    {
        private NetPeer peer;

        public RUDPPeer(NetPeer peer)
        {
            this.peer = peer;
        }

        public int Id { get { return peer.Id; } }

        public object UserObject { get; set; }

        public void Disconnect()
        {   
            peer.Disconnect();
        }

        public void Send(byte[] bytes, Reliability reliability)
        {
            peer.Send(bytes, (DeliveryMethod)reliability);
        }
    }
}