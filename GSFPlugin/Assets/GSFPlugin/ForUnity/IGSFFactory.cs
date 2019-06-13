using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameSystem.GameCore.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IGSFFactory
{
    GameObject Create(GSFPacket packet);

}


