using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class ProcessServer : IServerLaunch
{
    public string ServerPath;
    private Process serverProcess;

    public int Port { get; set; }
    public string ConnectKey { get; set; }
    public int MaxPeers { get; set; }

    public bool isRunning { get; private set; }

    public DataReceivedEventHandler OnReceiveOutput;
    public DataReceivedEventHandler OnReceiveError;

    public void Reset()
    {
        ServerPath = "D:\\Work Space\\Visual Studio 2017\\SimpleGameServer\\SimpleGameServer";
        if (Directory.Exists(ServerPath))
        {
            serverProcess = new Process();
            serverProcess.StartInfo.FileName = "dotnet";
            serverProcess.StartInfo.Arguments = $"run --project \"{ServerPath}\"";
            serverProcess.StartInfo.CreateNoWindow = true;
            serverProcess.StartInfo.UseShellExecute = false;
            serverProcess.StartInfo.RedirectStandardError = true;
            serverProcess.StartInfo.RedirectStandardOutput = true;
            serverProcess.OutputDataReceived += ServerProcess_OutputDataReceived;
            serverProcess.ErrorDataReceived += ServerProcess_ErrorDataReceived;
        }
    }

    private void ServerProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if(OnReceiveOutput != null)
            OnReceiveOutput.Invoke(sender, e);
    }

    private void ServerProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (OnReceiveError != null)
            OnReceiveError.Invoke(sender, e);
    }

    public void Start()
    {   
        isRunning = serverProcess.Start();
        serverProcess.BeginErrorReadLine();
        serverProcess.BeginOutputReadLine();
    }

    public void Stop()
    {
        if(!serverProcess.HasExited)
            serverProcess.Kill();
        serverProcess.CancelErrorRead();
        serverProcess.CancelOutputRead();
    }

}
