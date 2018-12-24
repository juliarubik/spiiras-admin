using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelButton : MonoBehaviour {
    public Text text;
    LabelsController labelsController;

    public void SetText(string newText)
    {
        text.text = newText;
    }

    public void SetLabelsController(LabelsController newLabelsController)
    {
        labelsController = newLabelsController;
    }

    public void OnClick()
    {
        labelsController.SetSelectedLabel(this);
    }

    public void Select()
    {
        text.color = new Color(0.204f, 0.663f, 0.537f, 1.0f);
    }

    public void UnSelect()
    {
        text.color = Color.white;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
