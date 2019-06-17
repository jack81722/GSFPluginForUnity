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
        OnReceiveHandler OnRecvEvent { get; set; }

        Queue<PacketEvent> events { get; set; }

        void Send(byte[] bytes, Reliability reliability);
        void Recv(byte[] bytes, Reliability reliability);
        void Poll();
        void Disconnect();
    }

    public delegate void OnReceiveHandler(byte[] dgram, Reliability reliability);
}