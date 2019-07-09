using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameInformation
{
    public int GameId;
    public string GameName;

    public GameInformation(int id, string name)
    {
        GameId = id;
        GameName = name;
    }
}
