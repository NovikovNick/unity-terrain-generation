using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerSnapshot
{

    public int lastDatagramNumber;

    public byte playerId;
    public Vector3 position;
    public Vector3 direction;
}
