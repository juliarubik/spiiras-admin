using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Label : MonoBehaviour {
    public string labelName;
	// Use this for initialization
	void Start () {
	}

    public void SetName(string newName)
    {
        labelName = newName;
    }

    public string GetName()
    {
        return labelName;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
