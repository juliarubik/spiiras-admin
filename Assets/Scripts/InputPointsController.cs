using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputPointsController : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        LabelsEditController.action = LabelsEditController.LabelAction.ChooseLocation;
    }
}
