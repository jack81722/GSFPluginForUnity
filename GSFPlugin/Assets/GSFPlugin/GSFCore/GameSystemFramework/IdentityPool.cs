using System.Collections;
using System.Collections.Generic;

namespace GameSystem
{
    public class IdentityPool
    {
        private int serialId;
        private Queue<int> idPool;

        public IdentityPool()
        {
            serialId = 0;
            idPool = new Queue<int>();
        }

        public int NewID()
        {
            lock (idPool)
            {
                if (idPool.Count > 0)
                    return idPool.Dequeue();
            }
            return serialId++;
        }

        public void RecycleID(int id)
        {
            lock (idPool)
            {
                if (!idPool.Contains(id))
                    idPool.Enqueue(id);
            }
        }
    }
}