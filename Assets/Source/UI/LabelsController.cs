using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class LabelsController : MonoBehaviour {
    public PointsInputsController pointsInputsController;
    public GameObject prefab;
	// Use this for initialization
	void Start () {

	}

    private void ClearLabelsButtonsList()
    {
        foreach(Transform obj in transform)
        {
            Button button = GetComponent<Button>();
            if(button != null)
            {
                Destroy(button.gameObject);
            }
        }
    }

    public void LoadList()
    {
        Debug.Log("loading labels");
        if (!Utils.isOnline())
        {            
            Debug.Log("MainScreen: Нет подключения к интернету!");
            return;
        }
        
        WebClient webClient = new WebClient(Utils.REQUEST_URL, WebClient.RequestType.GET);
        
        // Поток в котором выполняется запрос на сервер

        StartCoroutine(webClient.requestJSON(null, (bool success, string responseString) =>
        {
            // Загрузка списка всех меток
            if (success)
            {
                LabelsList.self.clear();
                ClearLabelsButtonsList();

                JSONObject response = new JSONObject(responseString);
                Debug.Log("QRSAdmin: WebClient response " + response);

                if (response == null || response.list == null)
                {
                    Debug.Log("MainScreen: response || response.list == null");
                    return;
                }

                for (int i = 0; i < response.list.Count; i++)
                {
                    Debug.Log(i);
                    JSONObject item = response.list[i];

                    if (item != null)
                    {
                        string name = Regex.Unescape(item.GetField(Utils.JSON_NAME).str);
                        LabelsList.self.update(name, item);
                        GameObject newButton = GameObject.Instantiate(prefab);
                        newButton.GetComponent<LabelButton>().SetText(name);
                        newButton.transform.SetParent(transform);
                    }
                    else
                    {
                        Debug.Log("MainScreen: response.list item == null ");
                    }
                }

                if (LabelsList.self.size() == 0)
                {
                    Debug.Log("MainScreen: Ошибка загрузки списка...");
                }
            }
        }));
    }

    public void SetPoint(string name)
    {
        switch (ShowPathScreen.action)
        {
            case ShowPathScreen.LabelAction.ADD_POINT_A:
                {
                    pointsInputsController.SetValuePointA(name);
                    pointsInputsController.navMeshController.SetSource(getLocationByName(name));
                    
                }
                break;
            case ShowPathScreen.LabelAction.ADD_POINT_B:
                {
                    pointsInputsController.SetValuePointB(name);
                    pointsInputsController.navMeshController.SetDestination(getLocationByName(name));
                }
                break;
            default:
                {
                    pointsInputsController.SetValuePointA(name);
                    pointsInputsController.navMeshController.SetSource(getLocationByName(name));
                }
                break;
        }
    }

    private Vector3 getLocationByName(string name)
    {
        JSONObject tmpObj = LabelsList.self.getLabel(name);
        if(tmpObj != null)
        {
            return Utils.stringToVector3(tmpObj.GetField(Utils.JSON_LOCATION).str);
        }
        else
        {
            return Vector3.zero;
        }
        
       
    }
    private void removeLabel()
    {
        /*
        if (!Utils.isOnline())
        {
            
            Debug.Log("MainScreen: Нет подключения к интернету!");
            return;
        }

        
        

        string currentLabel = PlayerPrefs.GetString(Utils.PREF_CURRENT_LABEL);

        JSONObject obj = LabelsList.self.getLabel(currentLabel);

        if (obj == null)
        {
            Debug.Log("QRSAdmin: removeLabel JSONObject == null");
            return;
        }

        labelStatus.text = "Удаление метки " + currentLabel;
        Debug.Log("MainScreen: Удаление метки... " + obj.ToString());

        // Удаление метки осуществляется по идентификатору
        if (!obj.HasField(Utils.JSON_ID))
        {
            Debug.Log("MainScreen: Ошибка. Неизвестный идентификатор метки");
            return;
        }

        long id = obj.GetField(Utils.JSON_ID).i;
        string url = Utils.REQUEST_URL + id.ToString() + "/";

        WebClient webClient = new WebClient(url, WebClient.RequestType.DELETE);

        // Поток в котором выполняется запрос на сервер
        StartCoroutine(webClient.requestJSON(obj, (bool success, string responseString) =>
        {
            // Обработка ответа сервера
            if (success)
            {
                string message = Utils.NA;
                JSONObject response = new JSONObject(responseString);

                if (response == null)
                {
                    Debug.Log("MainScreen: response == null ");
                    enableUI();
                    loadLabelsList();
                    return;
                }

                if (response.HasField(Utils.JSON_MESSAGE))
                {
                    message = response.GetField(Utils.JSON_MESSAGE).str;
                }

                if (response.HasField(Utils.JSON_SUCCESS))
                {
                    if (response.GetField(Utils.JSON_SUCCESS).b)
                    {
                        labelStatus.text = "Метка успешно удалена!";
                        Debug.Log("MainScreen: Метка успешно удалена! " + response);
                    }
                    else
                    {
                        labelStatus.text = "Не удалось удалить метку: " + message;
                        Debug.Log("MainScreen: Не удалось удалить метку: " + response);
                    }
                }
                else
                {
                    labelStatus.text = "Некорректный ответ сервера " + message;
                    Debug.Log("MainScreen: Некорректный ответ " + response);
                }
            }
            else
            {
                labelStatus.text = "Ошибка удаления метки...";
                Debug.Log("MainScreen: Ошибка удаления метки...");
            }

            enableUI();
            loadLabelsList();
        }));
        */
    }
}
