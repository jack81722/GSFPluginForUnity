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

    public interface IPeer
    {
        int Id { get; }
        void Send(byte[] bytes, Reliability reliability);
        void Disconnect();
    }

    public delegate void OnReceiveDgramHandler(IPeer peer, byte[] dgram, Reliability reliability);
    public delegate void OnReceivePacketHandler(IPeer peer, object packet, Reliability reliability);
}