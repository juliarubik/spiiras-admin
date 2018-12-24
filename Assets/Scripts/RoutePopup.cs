using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutePopup : MonoBehaviour {

    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnSubmitName(string name)  //Этот метод срабатывает в момент начала ввода данных в текстовое поле.
    { 
        //Debug.Log(name);
    }
}
