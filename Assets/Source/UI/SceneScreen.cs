using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.AI;

public class SceneScreen: MonoBehaviour 
{
	// Тег объекта для идентификации
	private const string QR_TAG = "QRCode";
	public GameObject qrObject;
	public GameObject player;

	// How sensitive it with mouse
	public float camSens = 0.25f; 
	public float mouseSensitivity = 2.0f; // Mouse rotation sensitivity.
	// Regular speed
	public float mainSpeed = 100.0f;

	// Angryboy: Can be called by other things (e.g. UI) to see if camera is rotating
	private bool isRotating = false; 
	// Angryboy: Used by Y axis to match the velocity on X/Z axis
	private float speedMultiplier; 
	private float rotationY = -90.0f;

	private const float GROUND_LEVEL = 8.15f;
	private const float CAMERA_LEVEL = 27.0f;

	private NavMeshController controller;
	private Transform cameraTransform;
	private CharacterController charController;

	private Vector3 srcPos, dstPos;
	private int srcIndex;
	private Quaternion defaultRotation;
	private JSONObject currentJson;

	// WARNING: Все интерактивные элементы QRSAdmin удалены
	// из метода OnGUI(), метод Start() отредактирован
	private void Start()
	{
		cameraTransform = transform;
		charController = GetComponent<CharacterController>();
		defaultRotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));

		int mode = PlayerPrefs.GetInt(Utils.PREF_MODE);

		if(mode == Utils.MODE_QR)
		{
			loadLabelsList();
		}
		else if(mode == Utils.MODE_PATH)
		{
			loadLabelsList();

			srcPos =  new Vector3();
			dstPos = new Vector3();

			// Индексы объектов\путей в списке меток
			int srcIndex = PlayerPrefs.GetInt(Utils.PREF_PATH_SRC);
			int dstIndex = PlayerPrefs.GetInt(Utils.PREF_PATH_DST);
			Debug.Log("SceneScreen: [1] " + srcIndex + " [2] " + dstIndex);

			JSONObject srcObj = LabelsList.self.findLabelById(srcIndex);
			JSONObject dstObj = LabelsList.self.findLabelById(dstIndex);

			if(srcObj != null)
			{
				srcPos =  Utils.stringToVector3(srcObj.GetField(Utils.JSON_LOCATION).str);
			}
			else
			{
				Debug.Log("SceneScreen: srcObj == null");
			}

			if(dstObj != null)
			{
				dstPos = Utils.stringToVector3(dstObj.GetField(Utils.JSON_LOCATION).str);
			}
			else
			{
				Debug.Log("SceneScreen: dstObj == null");
			}

			Debug.Log("SceneScreen: Рисуем линию от " + srcPos.ToString() + " до " + dstPos.ToString());
			controller = player.GetComponent<NavMeshController>();
            controller.SetSource(srcPos);
			controller.SetDestination(dstPos);
		}
	}

	// TODO: Перемещение, зум и установка метки на сенсорных экранах
	// реализованы не совсем корректно
	// Зум должен быть с помощью 2х пальцев, перемещение
	// с помощью 2x джойстиков (осмотр + перемещение)
	private void OnGUI() { }
		
	private void Update()
	{
		if(controller != null)
		{
			Vector3 pos = controller.getPosition();
			pos.y = SceneScreen.CAMERA_LEVEL;
			transform.SetPositionAndRotation(pos, defaultRotation);
		}

		// TODO: Обновление списка меток и выбор метки лучше конечно вынести в UI
		if(Input.GetKey(KeyCode.Space))
		{
			// loadLabelsList();
			Debug.Log("SceneScreen: Обновление списка меток");
		}

		if(Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey("return"))
		{
			PlayerPrefs.SetInt(Utils.PREF_PATH_SRC, srcIndex);
			SceneManager.LoadScene(Utils.SCREEN_LABELS);
			Debug.Log("SceneScreen: Выбор второй метки");
		}

		// Mouse rotation
		if(Input.GetMouseButtonDown(1)) 
		{
			isRotating = true;
		}

		if(Input.GetMouseButtonUp(1)) 
		{
			isRotating = false;
		}

		// Touch rotation
		if(Utils.isTouchSupported() && !Input.mousePresent)
		{
			Touch touch = Input.GetTouch(0);

			if(touch.phase == TouchPhase.Moved)
			{
				isRotating = true;
			}
		}

		if(isRotating)
		{
			float axisX = 0.0f;
			float axisY = 0.0f;

			// Поддержка тача
			if(Utils.isTouchSupported() && !Input.mousePresent)
			{
				Touch touch = Input.GetTouch(0);
				axisX = touch.deltaPosition.x / 2.0f;
				axisY = touch.deltaPosition.y / 2.0f;
				mouseSensitivity = 0.75f;
			}
			// Поддержка мышки
			else
			{
				axisX = Input.GetAxis("Mouse X");
				axisY = Input.GetAxis("Mouse Y");
			}

			float rotationX = transform.localEulerAngles.y + axisX * mouseSensitivity;        
			rotationY += axisY * mouseSensitivity;
			rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);        
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0.0f);
		}

		// Touch move
		if(Utils.isTouchSupported() && !Input.mousePresent)
		{
			Touch touch1 = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);

			if(touch1.phase == TouchPhase.Stationary && touch2.phase == TouchPhase.Stationary)
			{
				move(cameraTransform.forward * 1.5f);
			}
		}

		// WASD
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) 
		{
			move(cameraTransform.forward);
		}

		if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 
		{
			move(-cameraTransform.forward);
		}

		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
		{
			move(-cameraTransform.right);
		}

		if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
		{
			move(cameraTransform.right);
		}

		// Mouse Zoom
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			move(cameraTransform.forward * 2.0f);
		}
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			move(-cameraTransform.forward * 2.0f);
		}
	}

	// Перемещение объекта и камеры
	private void move(Vector3 direction)
	{
		charController.Move(direction);
	}

	private bool isMouseClicked()
	{
		bool mouseClick = Event.current.isMouse && Event.current.button == 0 && Event.current.clickCount > 1;
		return mouseClick && Input.mousePresent;
	}

	private bool isTouchTap(int taps)
	{
		if(Utils.isTouchSupported())
		{
			Touch touch = Input.GetTouch(0);
			return touch.tapCount == taps;
		}
		else
		{
			return false;
		}
	}

	// Загрузка списка меток с сервера
	private void loadLabelsList()
	{
		if(!Utils.isOnline())
		{
			Debug.Log("SceneScreen: Нет подключения к интернету!");
			return;
		}

		Debug.Log("SceneScreen: Загрузка списка...");

		WebClient webClient = new WebClient(Utils.REQUEST_URL, WebClient.RequestType.GET);
        Debug.Log(Utils.REQUEST_URL);
		// Создаем пустой json объект для запроса
		JSONObject obj = new JSONObject();

		if(obj == null)
		{
			Debug.Log("SceneScreen: loadLabelsList JSONObject == null");
			return;
		}

		// Поток в котором выполняется запрос на сервер
		StartCoroutine(webClient.requestJSON(obj, (bool success, string responseString) =>
		{
			// Загрузка списка всех меток
			if(success)
			{
				LabelsList.self.clear();

				JSONObject response = new JSONObject(responseString);
				//Debug.Log("QRSAdmin: WebClient response " + response);

				if(response == null || response.list == null)
				{
					Debug.Log("SceneScreen: response || response.list == null");
					return;
				}

				for(int i = 0; i < response.list.Count; i++)
				{
					JSONObject item = response.list[i];

					if(item != null)
					{
						//Debug.Log("QRSAdmin: list item: " + item);
						string name = Regex.Unescape(item.GetField(Utils.JSON_NAME).str);
						// Добавляем метку список
						LabelsList.self.update(name, item);
					}
					else
					{
						Debug.Log("SceneScreen: response.list item == null ");
					}
				}

				// Чтение текущей выбранной метки
				currentJson = new JSONObject(PlayerPrefs.GetString(Utils.PREF_QR));

				if(currentJson == null)
				{
					Debug.Log("SceneScreen: loadLabelsList JSONObject == null");
					return;
				}

				// Чтение списка всех меток и их визуализацию
				for(int i = 0; i < LabelsList.self.size(); i++)
				{
					Vector3 labelPoint = new Vector3(0.0f, 0.0f, 0.0f);

					string labelName = LabelsList.self.getLabelTitle(i);

					if(labelName == null)
					{
						Debug.Log("SceneScreen: loadLabelsList title == null");
						return;
					}

					JSONObject tmpObj = LabelsList.self.getLabel(labelName);

					if(tmpObj == null)
					{
						Debug.Log("SceneScreen: loadLabelsList JSONObject == null");
						return;
					}

					string labelLocation = tmpObj.GetField(Utils.JSON_LOCATION).str;
					labelPoint = Utils.stringToVector3(labelLocation);
					Instantiate(qrObject, labelPoint, defaultRotation);
				}

				// Установка позиции на последнюю метку или на текущую при редактировании
				Vector3 point = new Vector3(40.0f, 27.0f, 25.0f);

				if(currentJson != null)
				{
					// Конвертируем строку в Vector3
					string position = currentJson.GetField(Utils.JSON_LOCATION).str;
					srcIndex = (int) currentJson.GetField(Utils.JSON_ID).i;
					point = Utils.stringToVector3(position);
				}

				point.y = CAMERA_LEVEL;
				Debug.Log("SceneScreen: Установка камеры " + point.ToString() + " для метки " + srcIndex);
				player.transform.SetPositionAndRotation(point, defaultRotation);
				cameraTransform.SetPositionAndRotation(point, defaultRotation);

				if(LabelsList.self.size() > 0)
				{
					Debug.Log("SceneScreen: Список загружен");
				}
				else
				{
					Debug.Log("SceneScreen: Список пуст!");
				}
			}
			else
			{
				Debug.Log("SceneScreen: Ошибка загрузки списка...");
			}
		}));
	}
}