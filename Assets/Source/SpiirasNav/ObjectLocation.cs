﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/edemskiy/spiiras-nav-unity
public class ObjectLocation 
{
    public string time;
    public float x;
    public float z;
    public string userID;

    public ObjectLocation(string time, float x, float z, string userID) 
	{
        this.x = x;
        this.z = z;
        this.time = time;
        this.userID = userID;
    }

    public string toJSON() 
	{
        return JsonUtility.ToJson(this);
    }
}
