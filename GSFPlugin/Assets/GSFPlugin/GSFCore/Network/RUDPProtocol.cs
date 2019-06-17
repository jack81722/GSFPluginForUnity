using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LiteNetLib;

namespace GameSystem.GameCore.Network
{
    public class RUDPProtocol
    {
        EventBasedNetListener listener;
        NetManager netManager;
        Task recvTask;

        public RUDPProtocol()
        {
            listener = new EventBasedNetListener();
            netManager = new NetManager(listener);
        }

        public void Start()
        {
            netManager.Start();
        }

        public void Start(int port)
        {
            netManager.Start(port);
        }

        public IPeer Connect(string ip, int port, string key)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return Connect(endPoint, key);
        }

        public IPeer Connect(IPEndPoint destination, string key)
        {   
            NetPeer netPeer = netManager.Connect(destination, key);
            RUDPPeer rudpPeer = new RUDPPeer(netPeer);
            listener.NetworkReceiveEvent += (peer, reader, method) => RecvEvent(rudpPeer, reader, method);
            recvTask = Task.Factory.StartNew(RecvProcess);
            return rudpPeer;
        }

        public void RecvEvent(RUDPPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte[] dgram = new byte[reader.AvailableBytes];
            reader.GetBytes(dgram, dgram.Length);
            peer.Recv(dgram, (Reliability)deliveryMethod);
            reader.Recycle();
        }

        bool isReceiving = false;
        private async void RecvProcess()
        {
            isReceiving = true;
            while (isReceiving)
            {
                netManager.PollEvents();
                await Task.Delay(15);
            }
        }

        public void Close()
        {
            isReceiving = false;
            recvTask.Wait();
        }
    }
}