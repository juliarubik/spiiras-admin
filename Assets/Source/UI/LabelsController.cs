using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class LabelsController : MonoBehaviour {
    // public PointsInputsController pointsInputsController;
    public GameObject buttonPrefab;
    public GameObject markerPrefab;
    public GameObject content;
    public LevelsController levelsController;
    private float prevActiveLevelPositionY;
    private LabelButton selectedLabelButton;

    private GameObject markersStore;
    // Use this for initialization
    void Start () {
        prevActiveLevelPositionY = LevelButton.activeLevelPositionY;
        markersStore = new GameObject("Markers");
        LoadList();
        ShowOnlyActiveMarkers();
    }

    public void SetSelectedLabel(LabelButton newSelectedLabelButton)
    {
        if(selectedLabelButton != null)
        {
            selectedLabelButton.UnSelect();
        }
        selectedLabelButton = newSelectedLabelButton;
        selectedLabelButton.Select();
        Debug.Log(selectedLabelButton.text.text);
    }

    private void ClearLabelsButtonsList()
    {
        foreach(Transform obj in content.transform)
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
        ClearMarkersList();
        
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
                    JSONObject item = response.list[i];

                    if (item != null)
                    {
                        string name = Regex.Unescape(item.GetField(Utils.JSON_NAME).str);
                        LabelsList.self.update(name, item);
                        GameObject newButton = GameObject.Instantiate(buttonPrefab);
                        GameObject newLabel = GameObject.Instantiate(markerPrefab);
                        newLabel.transform.position = Utils.stringToVector3(item.GetField(Utils.JSON_LOCATION).str);
                        newLabel.transform.parent = markersStore.transform;
                        newLabel.GetComponent<Label>().SetName(name);
                        newButton.GetComponent<LabelButton>().SetText(name);
                        newButton.GetComponent<LabelButton>().SetLabelsController(this);
                        newButton.transform.SetParent(content.transform);
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
                else
                {
                    ShowOnlyActiveMarkers();
                }
            }
        }));
    }

    public void SetPoint(string name)
    {
        /*
        Debug.Log(ShowPathScreen.action);
        switch (ShowPathScreen.action)
        {
            case ShowPathScreen.LabelAction.ADD_POINT_A:
                {
                    pointsInputsController.SetValuePointA(name);
                    pointsInputsController.navMeshController.SetSource(getLocationByName(name));
                    ShowPathScreen.action = ShowPathScreen.LabelAction.ADD_POINT_B;

                }
                break;
            case ShowPathScreen.LabelAction.ADD_POINT_B:
                {
                    pointsInputsController.SetValuePointB(name);
                    pointsInputsController.navMeshController.SetDestination(getLocationByName(name));
                    ShowPathScreen.action = ShowPathScreen.LabelAction.NA;
                }
                break;
            default:
                {
                    pointsInputsController.SetValuePointA(name);
                    pointsInputsController.navMeshController.SetSource(getLocationByName(name));
                    ShowPathScreen.action = ShowPathScreen.LabelAction.ADD_POINT_B;
                }
                break;
        }
        */
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

    public void ShowOnlyActiveMarkers()
    {
        foreach (Transform marker in markersStore.transform)
        {
            marker.gameObject.SetActive(marker.gameObject.transform.position.y < levelsController.getActiveLevelPosition().y + 1f);
        }
    }

    private void ClearMarkersList()
    {
        foreach (Transform marker in markersStore.transform)
        {
            Destroy(marker.gameObject);
        }
    }

    public void RemoveLabel()
    {
        if (!Utils.isOnline())
        {
            Debug.Log("MainScreen: Нет подключения к интернету!");
            return;
        }

        JSONObject obj = LabelsList.self.getLabel(selectedLabelButton.text.text);

        if (obj == null)
        {
            Debug.Log("QRSAdmin: removeLabel JSONObject == null");
            return;
        }

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
                        
                        Debug.Log("MainScreen: Метка успешно удалена! " + response);
                    }
                    else
                    {
                       
                        Debug.Log("MainScreen: Не удалось удалить метку: " + response);
                    }
                }
                else
                {
                    
                    Debug.Log("MainScreen: Некорректный ответ " + response);
                }
                LoadList();
            }
            else
            {              
                Debug.Log("MainScreen: Ошибка удаления метки...");
            }
        }));
    }

    private void Update()
    {

        if (prevActiveLevelPositionY != levelsController.getActiveLevelPosition().y)
        {
            ShowOnlyActiveMarkers();
            prevActiveLevelPositionY = levelsController.getActiveLevelPosition().y;
        }

    }
}
