using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public enum Reliability
    {
        ReliableOrder = 3,
        ReliableSequence = 4,
        ReliableUnorder = 1,
        Sequence = 2,
        Unreliable = 0
    }

    public abstract class Peer
    {
        public virtual int Id { get; protected set; }
        public virtual bool isConnecting { get; protected set; }

        public OnReceiveHandler OnPeerReceiveEvent;

        protected Queue<PacketEvent> events;


        public Peer()
        {
            events = new Queue<PacketEvent>();
        }

        public abstract void Send(byte[] bytes, Reliability reliability);
        
        public virtual void Poll()
        {
            
        }

        public abstract void Disconnect();
    }

    public delegate void OnReceiveHandler(Peer peer, byte[] dgram, Reliability reliability);
}