using GameSystem.GameCore;
using System.Collections;
using System.Collections.Generic;

public class PlayerReceiver : Component
{
    public override void Start()
    {
        // Accept players 
        // Create instances of players
        // Send player information
        var player = GetPlayerToken();
        player.Accpet();
    }


}

