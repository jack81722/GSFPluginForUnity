﻿using GameSystem.GameCore.Network;
using LiteNetLib;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ClientPeerLauncher : MonoBehaviour
{
    public ClientPeer peer { get; private set; }

    public string serverIp = "127.0.0.1";
    public int serverPort = 8888;
    public string connectKey = "Test";

    private void Awake()
    {
        // New client peer before start
        peer = new SimpleClientPeer(new FormmaterSerializer());
    }

    private void Start()
    {   
        IEnumerable<IPacketReceiver> receivers = FindObjectsOfType<MonoBehaviour>().OfType<IPacketReceiver>();
        foreach(var receiver in receivers)
        {
            ((SimpleClientPeer)peer).AddReceiver(receiver);
            Debug.Log($"Add receiver : {receiver}");
        }
        ((SimpleClientPeer)peer).AddAction(-1, OnReceiveMessage);
    }

    public void Connect(string ip, int port, string key)
    {   
        peer.Connect(ip, port, key);
    }

    public void ClickToConnect()
    {
        Connect(serverIp, serverPort, connectKey);
    }

    public void ClickToJoinGame()
    {
        peer.Send(new object[] { 0, "Join game." }, Reliability.ReliableOrder);
    }

    public void ClickToStartGame()
    {
        Debug.Log("Send start game.");
        peer.Send(new object[] { 1, "Start game." }, Reliability.ReliableOrder);
    }

    public void ClickToSayHello()
    {
        peer.Send(new object[] { -1, "Hello" }, Reliability.ReliableOrder);
    }

    public void OnReceiveMessage(object obj)
    {
        string msg = (string)obj;
        Debug.Log($"Server said : \"{msg}\"");
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
