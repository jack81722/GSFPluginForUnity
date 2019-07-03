using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ServerLauncher))]
public class ServerLauncherEditor : Editor
{
    private ServerLauncher launcher;
    SerializedProperty startProp;
    SerializedProperty stopProp;
    SerializedProperty portProp;
    SerializedProperty keyProp;
    SerializedProperty maxProp;

    public void OnEnable()
    {
        startProp = serializedObject.FindProperty("StartOnAwake");
        stopProp = serializedObject.FindProperty("StopOnDestroy");
        portProp = serializedObject.FindProperty("Port");
        keyProp = serializedObject.FindProperty("ConnectKey");
        maxProp = serializedObject.FindProperty("MaxPeers");
        launcher = (ServerLauncher)serializedObject.targetObject;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(startProp);
        EditorGUILayout.PropertyField(stopProp);
        bool isRunning = launcher != null ? launcher.isRunning : false;

        // lock input after server start
        if (!isRunning)
        {   
            portProp.intValue = Mathf.Clamp(EditorGUILayout.IntField("Port", portProp.intValue), 0, ushort.MaxValue);
            keyProp.stringValue = EditorGUILayout.TextField("Connect Key", keyProp.stringValue);
            maxProp.intValue = Mathf.Clamp(EditorGUILayout.IntField("Max Peers", maxProp.intValue), 0, int.MaxValue);
        }
        else
        {
            EditorGUILayout.IntField("Port", portProp.intValue);
            EditorGUILayout.TextField("Connect Key", keyProp.stringValue);
            EditorGUILayout.IntField("Max Peers", maxProp.intValue);
        }
        EditorGUILayout.Toggle("IsRunning", isRunning);
        if(!isRunning && GUILayout.Button("Start Server"))
        {
            launcher.ResetServer();
            launcher.Launch();
        }
        if (isRunning && GUILayout.Button("Stop Server"))
        {
            launcher.Stop();
        }

        serializedObject.ApplyModifiedProperties();
        //SceneView.RepaintAll();
    }
}
