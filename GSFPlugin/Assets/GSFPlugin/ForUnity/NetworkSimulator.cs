using AdvancedGeneric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class NetworkSimulator
{
    private static NetworkSimulator _instance;
    public static NetworkSimulator instance
    {
        get
        {
            if (_instance == null)
                _instance = new NetworkSimulator();
            return instance;
        }
    }

    private int serialPeerID;
    public AutoSortList<PeerSimulator> peers;

    public NetworkSimulator()
    {
        serialPeerID = 0;
        peers = new AutoSortList<PeerSimulator>(ComparePeer);
    }

    public static int NewID()
    {
        return instance.serialPeerID++;
    }

    public static PeerSimulator CreatePeer()
    {
        return new PeerSimulator();
    }

    public static void Register(PeerSimulator peer)
    {
        instance.peers.TryAdd(peer);
    }

    public static PeerSimulator Find(int id)
    {
        return instance.peers.Find(id, CompareIDAndPeer);
    }

    private static int ComparePeer(PeerSimulator x, PeerSimulator y)
    {
        return x.peerID.CompareTo(y.peerID);
    }

    private static int CompareIDAndPeer(int id, PeerSimulator peer)
    {
        return id.CompareTo(peer.peerID);
    }
}

public class PeerSimulator
{
    public int peerID;
    public Queue<PeerSimulator> acceptQueue;
    public Queue<object> dataQueue;
    public PeerSimulator remote;

    public delegate void ReceiveHandler(object obj);
    public event ReceiveHandler OnReceive;

    public PeerSimulator()
    {
        peerID = NetworkSimulator.NewID();
        acceptQueue = new Queue<PeerSimulator>();
        dataQueue = new Queue<object>();
        NetworkSimulator.Register(this);
    }

    public void Connect(int id)
    {
        PeerSimulator peer;
        if ((peer = NetworkSimulator.Find(id)) != null)
        {
            peer.accept(this);
        }
        throw new System.InvalidOperationException("Cannot find peer.");
    }

    private void accept(PeerSimulator peer)
    {
        lock(acceptQueue)
            acceptQueue.Enqueue(peer);
    }

    public PeerSimulator Accept()
    {
        PeerSimulator peer;
        while (true)
        {
            lock (acceptQueue)
            {
                if (acceptQueue.Count > 0)
                {
                    peer = acceptQueue.Dequeue();
                    break;
                }
            }
            Thread.Sleep(100);
        }
        return peer;
    }

    public void Send(object obj)
    {
        remote.Receive(obj);
    }

    private void Receive(object obj)
    {
        dataQueue.Enqueue(obj);
    }

    public void Poll()
    {
        while(dataQueue.Count > 0)
        {
            try
            {
                OnReceive.Invoke(dataQueue.Dequeue());
            }
            catch (Exception e)
            {

            }
        }
    }
}
