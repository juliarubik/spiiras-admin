using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewLabelCreator : MonoBehaviour {
    public InputField nameField;
    public InputField descriptionField;
    public InputField locationField;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetLocation(Vector3 location)
    {
        locationField.text = location.ToString();
    }

    public void CreateLabel()
    {
        if(nameField.text == "" || locationField.text == "")
        {
            Debug.Log("Заполните обязательне поля");
            return;
        }
        JSONObject labelJSON = LabelsList.self.initLabel(nameField.text, locationField.text, descriptionField.text);


        if (!Utils.isOnline())
        {
            Debug.Log("QRSAdmin: Нет подключения к интернету!");
            return;
        }

        WebClient webClient = new WebClient(Utils.REQUEST_URL, WebClient.RequestType.POST);

        StartCoroutine(webClient.requestJSON(labelJSON, (bool success, string responseString) =>
        {
            // Обработка ответа сервера
            if (success)
            {
                string message = Utils.NA;
                JSONObject response = new JSONObject(responseString);

                if (response == null)
                {
                    Debug.Log("QRSAdmin: response == null ");
                    //enableUI();
                    // loadLabelsList();
                    return;
                }
                Debug.Log(response);

                if (response.HasField(Utils.JSON_MESSAGE))
                {
                    message = response.GetField(Utils.JSON_MESSAGE).str;
                }

                if (response.HasField(Utils.JSON_SUCCESS))
                {
                    if (response.GetField(Utils.JSON_SUCCESS).b)
                    {
                        //labelStatus.text = "Метка " + name + " успешно создана!";
                        Debug.Log("QRSAdmin: Метка " + response + " успешно создана!");
                    }
                    else
                    {
                        //labelStatus.text = "Не удалось создать метку: " + message;
                        Debug.Log("QRSAdmin: Не удалось создать метку: " + response);
                    }
                }
                else
                {
                    //labelStatus.text = "Некорректный ответ сервера " + message;
                    Debug.Log("QRSAdmin: Некорректный ответ " + response);
                }
            }
            else
            {
                //labelStatus.text = "Ошибка создания метки " + name;
                Debug.Log("QRSAdmin: Ошибка создания метки " + name);
            }
        }));

    }
}
