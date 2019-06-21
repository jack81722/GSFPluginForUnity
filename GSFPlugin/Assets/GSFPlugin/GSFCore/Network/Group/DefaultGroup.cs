using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class DefaultGroup : PeerGroup
    {
        public DefaultGroup(ISerializer serializer) : base(serializer)
        {
            OnGroupReceiveEvent += HandlePacketEvent;
        }

        protected virtual void HandlePacketEvent(Peer peer, object data, Reliability reliability)
        {
            GroupCommand command = (GroupCommand)data;
            switch (command.Code)
            {
                case 0:         // Join group
                    int groupId = (int)command.Args[0];
                    if(PeerGroupManager.TryGetGroup(groupId, out PeerGroup group))
                    {
                        Task.Run(() => group.JoinAsync(peer, command.Args[1]));
                    }
                    break;
                default:
                    break;
            }
        }
    }

    [Serializable]
    public class GroupCommand
    {
        public int Code;
        public object[] Args;
    }
}