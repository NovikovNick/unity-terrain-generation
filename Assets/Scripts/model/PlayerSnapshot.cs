using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerSnapshot
{
    public long timestamp;
    public int sequenceNumber;
    public int acknowledgmentNumber;

    public ServerGameObject player;
    public List<Player> otherPlayers;
    public List<TerrainChunk> terrainChunks; 

    public override string ToString()
    {
        return "PlayerSnapshot: sequenceNumber=" + sequenceNumber + "; acknowledgmentNumber=" + acknowledgmentNumber;
    }
}
