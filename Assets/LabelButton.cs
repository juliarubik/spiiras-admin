using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelButton : MonoBehaviour {
    public Text text;

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void OnClick()
    {
        GetComponentInParent<LabelsController>().SetPoint(text.text);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
