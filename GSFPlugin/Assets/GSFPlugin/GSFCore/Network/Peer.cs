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
        /// <summary>
        /// Identity of peer
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Boolean state of peer is connected
        /// </summary>
        bool isConnected { get; }

        /// <summary>
        /// Custom object of peer used by user
        /// </summary>
        object UserObject { get; set; }

        /// <summary>
        /// Send method of peer
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="reliability"></param>
        void Send(byte[] bytes, Reliability reliability);

        /// <summary>
        /// Disconnect method of peer
        /// </summary>
        void Disconnect();
    }

    public delegate void OnReceiveDgramHandler(IPeer peer, byte[] dgram, Reliability reliability);
    public delegate void OnReceivePacketHandler(IPeer peer, object packet, Reliability reliability);
}