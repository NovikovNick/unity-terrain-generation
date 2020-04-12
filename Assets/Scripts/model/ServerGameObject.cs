﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerGameObject
{
    public Vector3 position;
    public Vector3 direction;

    public override string ToString()
    {
        return "ServerGameObject: position=" + position;
    }
}
