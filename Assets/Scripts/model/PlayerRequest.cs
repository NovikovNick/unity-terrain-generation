using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerRequest
{
    public int sequenceNumber;
    public int acknowledgmentNumber;
    public float timeDelta;

    public Vector3 direction;
    public bool isRunning;
    public float magnitude;
    public Vector3 loadedChunck;

    public override string ToString()
    {
        return "PlayerRequest: sequenceNumber=" + sequenceNumber + "; acknowledgmentNumber=" + acknowledgmentNumber;
    }
}
