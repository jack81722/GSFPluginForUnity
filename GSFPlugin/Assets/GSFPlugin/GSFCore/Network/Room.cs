using GameSystem.GameCore.Debugger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameSystem.GameCore.Network
{
    public abstract class Room 
    {
        private float LoopPeriod = 0;
        private CancellationTokenSource LoopTcs;
        protected PeerGroup group;

        protected TimeSpan TotalTime;
        protected TimeSpan DeltaTime;

        protected ISerializer serializer;
        protected IDebugger debugger;

        public Room(ISerializer serializer, IDebugger debugger)
        {
            this.serializer = serializer;
            this.debugger = debugger;
            group = new PeerGroup(serializer);
            group.OnGroupReceiveEvent += MainReceiveLogic;
            //group.OnPeerJoinRequest += ReceiveJoinRequest;
            LoopTcs = new CancellationTokenSource();
            events = new Queue<PacketEvent>();
        }

        /// <summary>
        /// Start room process
        /// </summary>
        /// <param name="period">frame update period (millionsecond)</param>
        public void Start(int period = 15)
        {
            LoopPeriod = period;
            Task task = Task.Run(MainLoop);
        }

        private void MainLoop()
        {
            DateTime curr_time = DateTime.UtcNow;
            DateTime last_time = curr_time;
            while (!LoopTcs.IsCancellationRequested)
            {
                curr_time = DateTime.UtcNow;
                // caculate time span between current and last time
                if ((DeltaTime = curr_time - last_time).TotalMilliseconds > 0)
                {
                    TotalTime += DeltaTime;
                    LoopLogic();

                    while (events.Count > 0)
                    {
                        var e = events.Dequeue();
                        ReceiveLogic(e.GetPeer(), e.GetData(), e.GetReliability());
                    }
                    HandleJoinRequests();
                }
                // correct time into fps
                if (DeltaTime.TotalMilliseconds < LoopPeriod)
                {
                    Thread.Sleep((int)(LoopPeriod - DeltaTime.TotalMilliseconds));
                }
                last_time = curr_time;
            }
        }

        protected abstract void LoopLogic();

        Queue<PacketEvent> events;
        private void MainReceiveLogic(Peer peer, object obj, Reliability reliability)
        {
            if (LoopPeriod == 0)
            {
                ReceiveLogic(peer, obj, reliability);
                return;
            }
            events.Enqueue(new PacketEvent(peer, obj, reliability));
        }

        protected abstract void ReceiveLogic(Peer peer, object obj, Reliability reliability);

        #region Join request methods
        private void HandleJoinRequests()
        {
            while (group.GetQueueingCount() > 0)
                OnJoinRequestEvent(group.DequeueJoinRequest());
        }

        protected virtual void OnJoinRequestEvent(JoinGroupRequest request)
        {
            debugger.Log("handle request");
            request.Accept(null);
            GroupedPacket packet = new GroupedPacket();
            packet.groupId = 0;
            packet.data = request.Task.Result;
            debugger.Log(request.Task.Result);
            request.Peer.Send(serializer.Serialize(packet), Reliability.ReliableOrder);
        }

        public async Task<JoinGroupResponse> Join(Peer peer, object arg)
        {
            return await group.JoinAsync(peer, arg);
        }
        #endregion

        public List<Peer> GetPeerList()
        {
            return group.GetPeerList();
        }

        public void Close()
        {
            LoopTcs.Cancel();
        }
    }
}