using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

// TODO: В этом окне тоже нужен таймер?
public class LabelsScreen: MonoBehaviour 
{
	public Button buttonRefresh;
	public Button buttonClose;
	public Button buttonForward;
	public Button buttonBack;
	public Button buttonPath;
    public Dropdown labelDropdownSrc, labelDropdownDst;
	public Text labelStatus;

	// Текущая метка пользователя
	private int srcIndex = 0;
	// Вторая метка, выбранная пользователем
	private int dstIndex = 0;
	private int selectedIndex = 0;

	private void Start() 
	{
		// buttonBack.onClick.AddListener(backClick);
		// buttonForward.onClick.AddListener(forwardClick);
		buttonPath.onClick.AddListener(showPathClick);
		buttonClose.onClick.AddListener(closeWindow);
		buttonRefresh.onClick.AddListener(refreshClick);

		loadList();
	}

    
	private void loadList1()
	{
		srcIndex = PlayerPrefs.GetInt(Utils.PREF_PATH_SRC);

		labelStatus.text = "Загрузка списка...";
		Debug.Log("LabelsScreen: Загрузка списка...");

		disableUI();
		labelDropdownSrc.options.Clear();
        Debug.Log(LabelsList.self.size());

		for(int i = 0; i < LabelsList.self.size(); i++)
		{
			string lname = LabelsList.self.getLabelTitle(i);

			if(lname != null)
			{
				labelDropdownSrc.options.Add(new Dropdown.OptionData() {text=lname});
			}
			else
			{
				Debug.Log("LabelsScreen: loadList title == null");
			}
		}

		// setSelectId(0);

		labelStatus.text = "Список загружен";
		Debug.Log("LabelsScreen: Список загружен");

		enableUI();
	}
    
    private void loadList()
    {
        Debug.Log("loading labels");
        if (!Utils.isOnline())
        {
            labelStatus.text = "Нет подключения к интернету!";
            Debug.Log("MainScreen: Нет подключения к интернету!");
            return;
        }

        // listLoaded = true;

        labelStatus.text = "Загрузка списка...";
        Debug.Log("MainScreen: Загрузка списка...");

        disableUI();

        WebClient webClient = new WebClient(Utils.REQUEST_URL, WebClient.RequestType.GET);

        // Поток в котором выполняется запрос на сервер
        StartCoroutine(webClient.requestJSON(null, (bool success, string responseString) =>
        {
            // Загрузка списка всех меток
            if (success)
            {
                LabelsList.self.clear();
                labelDropdownSrc.options.Clear();

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
                        //Debug.Log("QRSAdmin: list item: " + item);
                        string name = Regex.Unescape(item.GetField(Utils.JSON_NAME).str);
                        LabelsList.self.update(name, item);
                        labelDropdownSrc.options.Add(new Dropdown.OptionData() { text = name });
                    }
                    else
                    {
                        Debug.Log("MainScreen: response.list item == null ");
                    }
                }

                if (LabelsList.self.size() > 0)
                {
                    labelStatus.text = "Список загружен";
                    labelDropdownSrc.value = 0;

                    string tmpTitle = LabelsList.self.getLabelTitle(0);

                    if (tmpTitle == null)
                    {
                        Debug.Log("MainScreen: title == null");
                    }
                    else
                    {
                        labelDropdownSrc.captionText.text = tmpTitle;
                    }
                }
                else
                {
                    labelStatus.text = "Список пуст!";
                }
            }
            else
            {
                labelStatus.text = "Ошибка загрузки списка...";
                Debug.Log("MainScreen: Ошибка загрузки списка...");
            }
            labelDropdownDst.options = new System.Collections.Generic.List<Dropdown.OptionData>(labelDropdownSrc.options);
            enableUI();

        }));
    }

	private void Update()  {}

    /*
	// Переключение на 1 слайд назад
	private void backClick()
	{
		selectedIndex -= 1;

		if(selectedIndex < 0)
		{
			selectedIndex = 0;
		}

		setSelectId(selectedIndex);
	}

	// Переключение на 1 слайд вперед
	private void forwardClick()
	{
		selectedIndex += 1;

		if(selectedIndex >= LabelsList.self.size())
		{
			selectedIndex = 0;
		}

		setSelectId(selectedIndex);
	}
    */
    private void setSelectId()
	{
		string tmpTitleSrc = LabelsList.self.getLabelTitle(labelDropdownSrc.value);
        string tmpTitleDst = LabelsList.self.getLabelTitle(labelDropdownDst.value);

        if (tmpTitleSrc == null || tmpTitleDst == null)
		{
			Debug.Log("LabelsScreen: setSelectId title == null");
			return;
		}

		JSONObject tmpObjSrc = LabelsList.self.getLabel(tmpTitleSrc);
        JSONObject tmpObjDst = LabelsList.self.getLabel(tmpTitleDst);

        if (tmpObjSrc == null || tmpObjDst == null)
		{
			Debug.Log("LabelsScreen: setSelectId JSONObject == null");
			return;
		}

        srcIndex = (int) tmpObjSrc.GetField(Utils.JSON_ID).i;
        dstIndex = (int) tmpObjDst.GetField(Utils.JSON_ID).i;
		// labelDropdownSrc.value = id;
		// labelDropdownSrc.captionText.text = tmpTitle;
		Debug.Log("From " + srcIndex + " To " + dstIndex);
	}

	// Переключение слайдов
	private void showPathClick()
	{
		setSelectId();
		PlayerPrefs.SetInt(Utils.PREF_PATH_SRC, srcIndex);
		PlayerPrefs.SetInt(Utils.PREF_PATH_DST, dstIndex);
		PlayerPrefs.SetInt(Utils.PREF_MODE, Utils.MODE_PATH);
		SceneManager.LoadScene(Utils.SCREEN_SCENE);
	}

	private void refreshClick()
	{
		loadList();
	}

	// Запуск основного экрана
	private void closeWindow()
	{
		//PlayerPrefs.SetString(Utils.PREF_QR, "");
		SceneManager.LoadScene(Utils.SCREEN_MAIN);
	} 

	// UI контроль 
	private void disableUI()
	{
        Debug.Log(labelDropdownSrc);
        buttonBack.interactable = false;
		buttonForward.interactable = false;
		buttonPath.interactable = false;
		buttonRefresh.interactable = false;
		labelDropdownSrc.interactable = false;
	}

	private void enableUI()
	{
		buttonBack.interactable = true;
		buttonForward.interactable = true;
		buttonPath.interactable = true;
		buttonRefresh.interactable = true;
		labelDropdownSrc.interactable = true;
	}
}
