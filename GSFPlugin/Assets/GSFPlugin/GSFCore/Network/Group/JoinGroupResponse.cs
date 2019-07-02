using System;
using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    [Serializable]
    public class JoinGroupResponse
    {   
        public int groupId;
        public int operationCode;
        public ResultType type;
        public string message;
        public object UserObject;

        public JoinGroupResponse(int groupId, int operationCode, ResultType type, string msg)
        {
            this.groupId = groupId;
            this.operationCode = operationCode;
            this.type = type;
            message = msg;
        }

        public JoinGroupResponse(int groupId, int operationCode, ResultType type, string msg, object obj)
        {
            this.groupId = groupId;
            this.operationCode = operationCode;
            this.type = type;
            message = msg;
            UserObject = obj;
        }

        public override string ToString()
        {
            if (message != null && message.Length > 0)
                return string.Format("Join group[{0}] result : {1}, {2}; object : {3}", groupId, type, message, UserObject);
            return string.Format("Join group[{0}] result : {1}; object : {2}", groupId, type, UserObject);
        }

        [Serializable]
        public enum ResultType
        {
            Accepted,
            Rejected,
            HasJoined,
            InQueue,
            Handling,
            Cancelled
        }
    }
}