using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;

public class ClientGroupAgentManager
{
    private static object instLock = new object();
    private static ClientGroupAgentManager _inst;
    public static ClientGroupAgentManager instance
    {
        get
        {
            lock (instLock)
            {
                if (_inst == null)
                {
                    lock (instLock)
                    {
                        _inst = new ClientGroupAgentManager();
                    }
                }
            }
            return _inst;
        }
    }

    private List<GroupAgent> agents;

    public ClientGroupAgentManager()
    {
        agents = new List<GroupAgent>();
    }

    public void OnJoinedGroup(JoinGroupResponse response)
    {
        if(!agents.Exists(agent => agent.GroupId == response.groupId && agent.OperationCode == response.operationCode))
        {
            GroupAgent agent = new GroupAgent(response.groupId, response.operationCode);
            agents.Add(agent);
        }
    }

    public void AddReceiver(IPacketReceiver receiver)
    {
        var agent = agents.Find(a => a.OperationCode == receiver.OperationCode);
        if (agent != null)
            agent.RegisterReceiver(receiver);
    }

    private class GroupAgent
    {
        public int OperationCode { get; private set; }
        public int GroupId { get; private set; }

        public List<IPacketReceiver> receivers;

        public GroupAgent(int groupId, int opCode)
        {
            GroupId = groupId;
            OperationCode = opCode;
            receivers = new List<IPacketReceiver>();
        }

        public void RegisterReceiver(IPacketReceiver receiver)
        {
            if(receivers.Contains(receiver))
                receivers.Add(receiver);
        }

        public bool UnregisterReceiver(IPacketReceiver receiver)
        {
            return receivers.Remove(receiver);
        }
    }

}