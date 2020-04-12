using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerSnapshot
{
    public long timestamp;
    public int lastDatagramNumber;

    public ServerGameObject player;
    public List<ServerGameObject> otherPlayers;

    public override string ToString()
    {
        return "PlayerSnapshot: lastDatagramNumber=" + lastDatagramNumber;
    }
}
