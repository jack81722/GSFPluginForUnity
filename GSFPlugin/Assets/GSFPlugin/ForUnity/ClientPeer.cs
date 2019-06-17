using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPeer : MonoBehaviour
{
    RUDPProtocol protocol;
    RUDPPeer peer;
    ISerializer serializer;

    public string serverIp = "127.0.0.1";
    public int serverPort = 8888;
    public string connectKey = "Test";

    public OnReceiveHandler OnRecvEvent;

    private void Start()
    {
        protocol = new RUDPProtocol();
        serializer = new FormmaterSerializer();
    }

    public void Connect(string ip, int port, string key)
    {
        protocol.Start();
        peer = (RUDPPeer)protocol.Connect(ip, port, key);
        peer.OnRecvEvent += OnReceiveEvent;
    }

    public void OnReceiveEvent(byte[] dgram, Reliability reliability)
    {
        OnRecvEvent(dgram, reliability);
    }

    public void ClickToConnect()
    {
        Connect(serverIp, serverPort, connectKey);
    }

    private void Update()
    {
        if (peer != null)
        {
            peer.Poll();
        }
    }

    public void OnDestroy()
    {
        peer.Disconnect();
        protocol.Close();
    }
}
