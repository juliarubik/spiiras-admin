using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Timers;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RmController: MonoBehaviour 
{
	// UI компоненты
	public Button buttonRefresh;
	public Button buttonClose;
	public Button buttonForward;
	public Button buttonBack;
	public Button buttonPlay;
	public Dropdown dropdownList;
	public Text labelStatus;

	private System.Timers.Timer timer;
	private Dictionary<string, long> map;

	// Параметры таймера
	private const int TIMER_LIMIT = 120 * 1000;
	private const int TIMER_INVALIDATED = -1;
	private const int TIMER_OFF = 0;
	private const int TIMER_ON = 1;
	private const int TIMER_FINISH = 2;
	private int timerStatus = TIMER_OFF;

	// Слайды
	private int slideIndex = 0;
	private long slideId = 0;
	private long monitorId = 0;
	private string sessionUrl = "";
	private string sessionKey = "";

	// Инициализация экрана
	private void Start() 
	{
		buttonBack.onClick.AddListener(backClick);
		buttonForward.onClick.AddListener(forwardClick);
		buttonPlay.onClick.AddListener(playClick);
		buttonClose.onClick.AddListener(closeWindow);
		buttonRefresh.onClick.AddListener(refreshClick);

		refreshClick();

		map = new Dictionary<string, long>();
	}

	// ***** Управление слайдами *****

	private void setSlide(string name, int id)
	{
		slideId = map[name];
		slideIndex = id;
		var q = "mutation { sendMessage(id:"+ monitorId +", key:\""+ sessionKey+"\", currentMedia:"+ slideId +") { status } }";
		requestSlide(q);
	}

	// Запрос на переключение на 1 слайд назад
	private void nextSlide()
	{
		slideIndex += 1;

		if(slideIndex >= map.Count())
		{
			slideIndex = 0;
		}

		var q = "mutation { sendMessage(id:"+ monitorId +", key:\""+ sessionKey +"\", commands:\"next\") { status } }";
		requestSlide(q);
	}

	// Запрос на переключение на 1 слайд назад
	private void prevSlide()
	{
		slideIndex -= 1;

		if(slideIndex < 0)
		{
			slideIndex = 0;
		}

		var q = "mutation { sendMessage(id:"+ monitorId +", key:\""+ sessionKey +"\", commands:\"prev\") { status } }";
		requestSlide(q);
	}

	// Инициализация и загрузка списка слайдов
	private void initSlides(string qrData)
	{
		if(!Utils.isOnline())
		{
			labelStatus.text = "Нет подключения к интернету!";
			return;
		}

		disableUI();

		// Чтение данных из QR кода
		//qrData = "{\"id\":9,\"key\":\"3KG24C9IF6G646NJI765\",\"slide\":145382,\"url\":\"http://91.151.187.23:85/control/graphql.php\"}";
		JSONObject qrCodeObject = new JSONObject(qrData);
		slideId = qrCodeObject.GetField(Utils.JSON_SLIDE).i;
		monitorId = qrCodeObject.GetField(Utils.JSON_ID).i;
		sessionKey = qrCodeObject.GetField(Utils.JSON_KEY).str;
		sessionUrl = qrCodeObject.GetField(Utils.JSON_URL).str;

		WebClient webClient = new WebClient(sessionUrl, WebClient.RequestType.POST);
		// "query { monitor(id:0, key: "_EQ6LR4-") { medias { id, name } } }"
		var q = "query { monitor(id:"+ monitorId +", key:\""+ sessionKey +"\") { medias { id, name } } }";

		// Поток в котором выполняется запрос на сервер
		StartCoroutine(webClient.requestQL(q, null, (bool success, string responseString) =>
		{
			// Загрузка списка всех слайдов
			if(success)
			{
				Debug.Log("RmController: OK " + responseString);

				JSONObject response = new JSONObject(responseString);
				JSONObject data = response.GetField(Utils.JSON_DATA);
				JSONObject monitor = data.GetField(Utils.JSON_MONITOR);
				JSONObject medias = monitor.GetField(Utils.JSON_MEDIAS);

				slideIndex = 0;
				map.Clear();
				dropdownList.options.Clear();

				// Добавление всех слайдов в список
				foreach(JSONObject j in medias.list)
				{
					string lname = Regex.Unescape(j.GetField(Utils.JSON_NAME).str);
					long id = j.GetField(Utils.JSON_ID).i;

					if(!map.ContainsKey(lname))
					{
						map.Add(lname, id);
						dropdownList.options.Add(new Dropdown.OptionData() {text=lname});
					}
				}

				if(map.Count > 0)
				{
					labelStatus.text = "Загрузка списка слайдов...";
					setSlide(map.Keys.ElementAt(slideIndex), slideIndex);
				}
				else
				{
					labelStatus.text = "Список слайдов пуст!";
				}
			}
			else
			{
				labelStatus.text = "Ошибка запроса списка слайдов";
			}

			enableUI();

		}));
	}

	// Запрос слайда
	private void requestSlide(string query)
	{
		if(!Utils.isOnline())
		{
			labelStatus.text = "Нет подключения к интернету!";
			return;
		}

		labelStatus.text = "Выбор слайда...";

		disableUI();

		WebClient webClient = new WebClient(sessionUrl, WebClient.RequestType.POST);

		// Поток в котором выполняется запрос на сервер
		StartCoroutine(webClient.requestQL(query, null, (bool success, string responseString) =>
		{
			if(success)
			{
				JSONObject response = new JSONObject(responseString);
				Debug.Log("RmController: OK " + response);
				dropdownList.value = slideIndex;
				dropdownList.captionText.text = map.Keys.ElementAt(slideIndex);
				labelStatus.text = "OK";
			}
			else
			{
				labelStatus.text = "Ошибка выбора слайда";
			}

			enableUI();

		}));
	}

	// ***** UI *****

	// Отрисовка GUI в цикле
	private void OnGUI() 
	{
		// Таймер
		if(timerStatus == TIMER_OFF)
		{
			timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timer.Interval = TIMER_LIMIT;
			timer.Enabled = true;
			timerStatus = TIMER_ON;
		}
		else if(timerStatus == TIMER_FINISH)
		{
			closeWindow();
			timerStatus = TIMER_INVALIDATED;
		}
	}

	// Вкл\Выкл взаимодействие с UI компонентами
	private void disableUI()
	{
		buttonBack.interactable = false;
		buttonForward.interactable = false;
		buttonPlay.interactable = false;
		buttonRefresh.interactable = false;
		dropdownList.interactable = false;
	}

	private void enableUI()
	{
		buttonBack.interactable = true;
		buttonForward.interactable = true;
		buttonPlay.interactable = true;
		buttonRefresh.interactable = true;
		dropdownList.interactable = true;
	}

	// Переключение на 1 слайд назад
	private void backClick()
	{
		timerStatus = TIMER_INVALIDATED;
		prevSlide();
	}

	// Переключение на 1 слайд вперед
	private void forwardClick()
	{
		timerStatus = TIMER_INVALIDATED;
		nextSlide();
	}

	// Переключение слайдов
	private void playClick()
	{
		timerStatus = TIMER_INVALIDATED;

		if(map.Count > 0)
		{
			int id = dropdownList.value;

			if(id >= map.Count)
			{
				id = map.Count - 1;
			}

			if(id < 0) { id = 0; }

			setSlide(map.Keys.ElementAt(id), id);
		}
		else
		{
			labelStatus.text = "Список слайдов пуст!";
		}
	}

	private void refreshClick()
	{
		// QR
		string data = PlayerPrefs.GetString(Utils.PREF_QR, Utils.NA);
		//Debug.Log("WebClient: QR: " + data);
		initSlides(data);
	}

	// Запуск основного экрана
	private void closeWindow()
	{
		PlayerPrefs.SetString(Utils.PREF_QR, "");
		SceneManager.LoadScene(Utils.SCREEN_MAIN);
	} 

	// Callback функция для таймера
	private void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		if(timerStatus == TIMER_ON)
		{
			timerStatus = TIMER_FINISH;
		}
	}
}
