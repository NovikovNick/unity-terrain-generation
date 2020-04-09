using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerRequest
{
    public long timestamp;
    public int datagramNumber;
    public byte playerId;
    public Vector3 direction;
    public bool isRunning;
    public float magnitude;
}
