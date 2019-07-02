using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public interface IPeerGroup
    {
        int GroupId { get; }
        int OperationCode { get; }

        void AddEvent(IPeer peer, object data, Reliability reliability);

        Task<JoinGroupResponse> JoinAsync(IPeer peer, object arg);

        IPeer GetPeer(int peerID);

        bool TryGetPeer(int peerID, out IPeer peer);

        List<IPeer> FindAllPeers(Predicate<IPeer> predicate);

        void Close();
    }
}