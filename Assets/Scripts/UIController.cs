using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    [SerializeField] private RoutePopup routePopup;

    [SerializeField] private BuildingPopup buildingPopup;

    [SerializeField] private LabelsPopup labelsPopup;

    void Start()
    {
        routePopup.Close(); //Закрываем всплывающее окно в момент начала игры.
        buildingPopup.Close();
        labelsPopup.Close();
    }

    public void OnOpenRoute()
    {
        routePopup.Open(); //метод всплывающего окна.
    }

    public void OnOpenBuilding()
    {
        buildingPopup.Open();
    }

    public void OnOpenLabels()
    {
        labelsPopup.Open();
    }



}
