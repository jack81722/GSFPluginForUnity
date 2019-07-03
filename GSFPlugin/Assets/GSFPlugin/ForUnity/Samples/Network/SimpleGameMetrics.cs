using System.Collections;
using System.Collections.Generic;


public static class SimpleGameMetrics
{
    public static class OperationCode
    {
        public const int Group = 0;

        public const int Lobby = 10;

        public const int Game = 20;
    }

    public static class GroupSwitchCode
    {
        public const int JoinReq = 0;
        public const int JoinRes = 1;
    }

    public static class ClientLobbySwitchCode
    {
        public const int JoinGame = 0;
        public const int StartGame = 1;
    }

    /// <summary>
    /// Switch code for client send
    /// </summary>
    public static class ClientGameSwitchCode
    {
        public const int Control = 0;
    }

    public static class ServerLobbySwitchCode
    {
        public const int JoinGameID = 0;
    }

    /// <summary>
    /// Switch code for server send
    /// </summary>
    public static class ServerGameSwitchCode
    {
        public const int BoxInfo = 0;
    }
}
