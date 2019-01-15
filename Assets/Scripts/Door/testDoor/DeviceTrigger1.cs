using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceTrigger1 : MonoBehaviour
{
    [SerializeField] private GameObject[] targets; //Список целевых объектов, которые будет активировать данный триггер.

    void OnTriggerEnter(Collider other)  //Метод OnTriggerEnter() вызывается при попадании объекта в зону триггера.
    { 
        foreach (GameObject target in targets)
        {
            target.SendMessage("Activate");
        }
    }

    void OnTriggerExit(Collider other)
    { 
        foreach (GameObject target in targets)  //в то время как метод OnTriggerExit() вызывается при выходе объекта из зоны триггера.
        {
            target.SendMessage("Deactivate");
        }
    }
}
