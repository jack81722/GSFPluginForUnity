using System.Collections;
using System.Collections.Generic;

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

    public interface IPeer
    {
        int Id { get; }
        void Send(byte[] bytes, Reliability reliability);
        void AddPacketEvent(byte[] dgram);
        void Poll();
    }

    
}