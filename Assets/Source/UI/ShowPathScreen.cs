using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowPathScreen : MonoBehaviour {
    public NavMeshController navMeshController;

    public PointsInputsController pointsInputsController;
    public enum LabelAction { NA, ADD_POINT_A, ADD_POINT_B };
    public static LabelAction action = LabelAction.NA;
    
    void Start () {
    }

    void Update()
    {
        switch (action)
        {
            case LabelAction.ADD_POINT_A:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.name.Contains("Floor"))
                            {
                                navMeshController.SetSource(hit.point);
                                pointsInputsController.SetValuePointA(hit.point.ToString());
                                action = LabelAction.ADD_POINT_B;
                            }                            
                        }
                    }
                }
                break;
            case LabelAction.ADD_POINT_B:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject.name.Contains("Floor"))
                            {
                                navMeshController.SetDestination(hit.point);
                                pointsInputsController.SetValuePointB(hit.point.ToString());
                                action = LabelAction.NA;
                            }
                        }
                    }
                }
                break;
        }
    }
    }
