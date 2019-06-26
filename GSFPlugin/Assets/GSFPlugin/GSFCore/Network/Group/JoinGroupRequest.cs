using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public class JoinGroupRequest
    {
        public int GroupId { get; private set; }
        public IPeer Peer { get; private set; }
        public object Arg { get; private set; }

        private TaskCompletionSource<JoinGroupResponse> tcs;
        public Task<JoinGroupResponse> Task { get { return tcs.Task; } }

        public JoinGroupRequest(int groupId, IPeer peer, object arg)
        {
            GroupId = groupId;
            Peer = peer;
            Arg = arg;
            tcs = new TaskCompletionSource<JoinGroupResponse>();
        }

        public IPeer Accept(object obj)
        {
            tcs.SetResult(new JoinGroupResponse(GroupId, JoinGroupResponse.ResultType.Accepted, "", obj));
            return Peer;
        }

        public void Reject(string msg = "", object obj = null)
        {
            tcs.SetResult(new JoinGroupResponse(GroupId, JoinGroupResponse.ResultType.Rejected, msg, obj));
        }

        public void Cancel(string msg = "", object obj = null)
        {
            tcs.SetResult(new JoinGroupResponse(GroupId, JoinGroupResponse.ResultType.Cancelled, msg, obj));
        }
    }
}