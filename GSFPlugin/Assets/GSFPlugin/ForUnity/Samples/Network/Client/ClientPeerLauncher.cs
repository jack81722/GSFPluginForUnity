﻿using GameSystem.GameCore.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientPeerLauncher : MonoBehaviour
{
    public ClientPeer peer { get; private set; }

    public string serverIp = "127.0.0.1";
    public int serverPort = 8888;
    public string connectKey = "Test";

    private Dictionary<int, List<IPacketReceiver>> receivers;
    private Dictionary<int, List<Action<object>>> actions;
    private Dictionary<int, int> tokens;

    private void Awake()
    {
        // New client peer before start
        peer = new ClientPeer(new FormmaterSerializer());
        receivers = new Dictionary<int, List<IPacketReceiver>>();
        actions = new Dictionary<int, List<Action<object>>>();
        tokens = new Dictionary<int, int>();
    }

    private void Start()
    {
        AddAction(SimpleGameMetrics.OperationCode.Group_JoinResponse, ReceiveJoinGroupResponse);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        RefreshReceivers();
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        RefreshReceivers();
    }

    public void RefreshReceivers()
    {
        var recvers = FindObjectsOfType<MonoBehaviour>().OfType<IPacketReceiver>();
        foreach(var recver in recvers)
        {
            if(receivers.TryGetValue(recver.OperationCode, out List<IPacketReceiver> recvList))
            {
                if (!recvList.Contains(recver))
                {
                    recvList.Add(recver);
                }
            }
            else
            {
                receivers.Add(recver.OperationCode, new List<IPacketReceiver>() { recver });
            }
        }
    }

    public void Connect()
    {
        Connect(serverIp, serverPort, connectKey);
    }

    public void Connect(string ip, int port, string key)
    {
        serverIp = ip;
        serverPort = port;
        connectKey = key;
        peer.OnClientReceivePacket += OnClientReceivePacketEvent;
        peer.Connect(ip, port, key);
    }

    public bool TryGetGroupId(int operationCode, out int groupId)
    {
        return tokens.TryGetValue(operationCode, out groupId);
    }

    public void AddAction(int operationCode, Action<object> action)
    {
        if(actions.TryGetValue(operationCode, out List<Action<object>> actionList))
        {
            actionList.Add(action);
        }
        else
        {
            actions.Add(operationCode, new List<Action<object>>() { action });
        }
    }

    private void OnClientReceivePacketEvent(object obj, Reliability reliability)
    {
        GenericPacket packet = obj as GenericPacket;
        if(packet != null)
        {
            if (receivers.TryGetValue(packet.InstCode, out List<IPacketReceiver> receiverList))
            {
                try
                {
                    foreach (var receiver in receiverList)
                    {
                        receiver.Receive(packet.Data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + e.StackTrace);
                }
            }
            if (actions.TryGetValue(packet.InstCode, out List<Action<object>> actionList))
            {
                foreach (var action in actionList)
                {
                    try
                    {
                        action.Invoke(packet.Data);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message + e.StackTrace);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Packet is null.");
        }
    }

    private void AddGroupToken(int operationCode, int groupId)
    {
        if (tokens.TryGetValue(operationCode, out int gid))
        {
            if (gid != groupId)
                throw new InvalidOperationException("Same operation group joined.");
        }
        else
        {
            tokens.Add(operationCode, groupId);
            Debug.Log($"add new token ({operationCode},{groupId})");
            
        }

    }

    private void ReceiveJoinGroupResponse(object obj)
    {
        JoinGroupResponse response = obj as JoinGroupResponse;
        if(response != null)
        {
            AddGroupToken(response.operationCode, response.groupId);
        }
    }

    public void Send(int operationCode, object data, Reliability reliability)
    {
        if (TryGetGroupId(operationCode, out int groupId)) {
            GenericPacket packet = new GenericPacket();
            packet.InstCode = groupId;
            packet.Data = data;
            peer.Send(packet, reliability);
        }
        else
        {
            Debug.LogError("Cannot find group id.");
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