using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelsEditController : MonoBehaviour {

    public enum LabelAction { ChooseLocation, NA};
    public static LabelAction action = LabelAction.NA;
    public NewLabelCreator newLabelCreator;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        switch (action)
        {
            case LabelAction.NA:
                {
                    
                }
                break;
            case LabelAction.ChooseLocation:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.name.Contains("Floor"))
                            {
                                // pointsInputsController.SetValuePointB(hit.point.ToString());
                                newLabelCreator.SetLocation(hit.point);
                                action = LabelAction.NA;
                            }
                        }
                    }
                }
                break;
        }
    }
}
