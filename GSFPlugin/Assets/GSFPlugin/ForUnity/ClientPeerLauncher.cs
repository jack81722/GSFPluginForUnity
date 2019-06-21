using GameSystem.GameCore.Network;
using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ClientPeerLauncher : MonoBehaviour
{
    Peer peer;
    ISerializer serializer;

    public string serverIp = "127.0.0.1";
    public int serverPort = 8888;
    public string connectKey = "Test";

    private void Start()
    {
        peer = new ClientPeer();
        peer.OnPeerReceiveEvent += OnReceiveDgram;
        serializer = new FormmaterSerializer();
    }

    public void Connect(string ip, int port, string key)
    {   
        ((ClientPeer)peer).Connect(ip, port, key);
    }

    public void ClickToConnect()
    {
        Connect(serverIp, serverPort, connectKey);
    }

    public void ClickToJoinGame()
    {
        byte[] dgram = serializer.Serialize(new object[] { 0, "Join game." });
        peer.Send(dgram, Reliability.ReliableOrder);
    }

    public void ClickToStartGame()
    {
        byte[] dgram = serializer.Serialize(new object[] { 1, "Start game." });
        peer.Send(dgram, Reliability.ReliableOrder);
    }

    public void ClickToSayHello()
    {
        byte[] dgram = serializer.Serialize(new object[] { -1, "Hello" });
        peer.Send(dgram, Reliability.ReliableOrder);
    }

    public void OnReceiveDgram(Peer peer, byte[] dgram, Reliability reliability)
    {
        try
        {
            object obj = serializer.Deserialize(dgram);
            object[] packet = obj as object[];
            if (packet != null)
            {
                switch ((int)packet[0])
                {
                    case 0:
                        Debug.Log(packet[1]);
                        break;
                }
            }
        }
        catch
        {

        }
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
        if(peer != null)
            peer.Disconnect();
    }

}
