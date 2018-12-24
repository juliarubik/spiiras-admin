using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PointsInputsController : MonoBehaviour {
    public InputField inputPointA;
    public InputField inputPointB;
    public NavMeshController navMeshController;
    // Use this for initialization
    void Start () {
		
	}

    public void SetValuePointA(string value)
    {
        inputPointA.text = value;
    }

    public void SetValuePointB(string value)
    {
        inputPointB.text = value;
    }

    public void ClearFields()
    {
        inputPointA.text = "";
        inputPointB.text = "";
        navMeshController.ResetPath();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
