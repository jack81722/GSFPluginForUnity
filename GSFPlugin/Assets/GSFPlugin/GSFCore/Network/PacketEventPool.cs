using System.Collections;
using System.Collections.Generic;

namespace GameSystem.GameCore.Network
{
    public class PacketEventPool
    {
        private Queue<PacketEvent> events;
        public int Count { get { return events.Count; } }

        public PacketEventPool()
        {
            events = new Queue<PacketEvent>();
        }

        public void Enqueue(IPeer peer, object obj, Reliability reliability)
        {
            lock(events)
                events.Enqueue(new PacketEvent(peer, obj, reliability));
        }

        public PacketEvent Dequeue()
        {
            lock(events)
                return events.Dequeue();
        }

        public PacketEvent[] DequeueAll()
        {   
            PacketEvent[] all;
            lock (events)
                all = events.ToArray();
            events.Clear();
            return all;
        }

        public void Clear()
        {
            lock (events)
                events.Clear();
        }

        private static Queue<PacketEvent> stack = new Queue<PacketEvent>();
        public static PacketEvent Create(IPeer peer, object data, Reliability reliability)
        {
            PacketEvent packet;
            if (stack.Count > 0)
            {
                packet = stack.Dequeue();
                packet.Set(peer, data, reliability);
            }
            else
            {
                packet = new PacketEvent(peer, data, reliability);
            }
            return packet;
        }

        public static void Recycle(PacketEvent packet)
        {
            if (!stack.Contains(packet))
                stack.Enqueue(packet);
        }
    }

    public class PacketEvent
    {   
        private IPeer peer;
        private object data;
        private Reliability reliability;

        public PacketEvent(IPeer peer, object data, Reliability reliability)
        {
            this.peer = peer;
            this.data = data;
            this.reliability = reliability;
        }

        public IPeer GetPeer()
        {
            return peer;
        }

        public object GetData()
        {
            return data;
        }

        public Reliability GetReliability()
        {
            return reliability;
        }

        public void Set(IPeer peer, object data, Reliability reliability)
        {
            this.peer = peer;
            this.data = data;
            this.reliability = reliability;
        }

        public void Recycle()
        {
            PacketEventPool.Recycle(this);
        }
    }
}