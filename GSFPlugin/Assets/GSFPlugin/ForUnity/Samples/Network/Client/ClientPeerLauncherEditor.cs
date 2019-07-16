#if UNITY_EDITOR
using GameSystem.GameCore.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ClientPeerLauncher))]
public class ClientPeerLauncherEditor : Editor
{  
    ClientPeer peer;
    SerializedProperty ipProp;
    SerializedProperty portProp;
    SerializedProperty keyProp;

    private void OnEnable()
    {
        ipProp = serializedObject.FindProperty("serverIp");
        portProp = serializedObject.FindProperty("serverPort");
        keyProp = serializedObject.FindProperty("connectKey");
        peer = typeof(ClientPeerLauncher).GetProperty("peer").GetValue((ClientPeerLauncher)serializedObject.targetObject) as ClientPeer;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        bool isConnected = peer != null ? peer.isConnected : false;
        // lock input after connected to server
        if (!isConnected)
        {
            ipProp.stringValue = EditorGUILayout.TextField("Server IP", ipProp.stringValue);
            portProp.intValue = EditorGUILayout.IntField("Server Port", portProp.intValue);
            keyProp.stringValue = EditorGUILayout.TextField("Connect Key", keyProp.stringValue);
        }
        else
        {
            EditorGUILayout.TextField("Server IP", peer.DestinationIP);
            EditorGUILayout.IntField("Server Port", peer.Port);
            EditorGUILayout.TextField("Connect Key", keyProp.stringValue);
        }
        EditorGUILayout.Toggle("IsConnected", isConnected);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif