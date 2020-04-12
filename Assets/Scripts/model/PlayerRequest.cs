using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerRequest
{
    public long timestamp;
    public float timeDelta;
    public int datagramNumber;

    public Vector3 direction;
    public bool isRunning;
    public float magnitude;

    public override string ToString()
    {
        return "PlayerRequest: datagramNumber=" + datagramNumber + ", " + timeDelta.ToString() + ", " + direction.x.ToString() + "-" + direction.y.ToString() + "-" + direction.z.ToString();
    }
}
