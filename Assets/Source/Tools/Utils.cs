using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

public class Utils
{
    // TEST:
    // Загрузить метки для теста http://qrappapi.pythonanywhere.com/api/v1/qrimages/
    //public const string REQUEST_URL = "http://54.39.23.81/api/v1/codes/";
    //public const string REQUEST_URL = "http://qrappapi.pythonanywhere.com/api/v1/codes/";
    public const string REQUEST_URL = "http://localhost:8000/api/v1/codes/";

    // Экраны приложения
    public const string SCREEN_MAIN = "MainScreen";
	public const string SCREEN_REMOTE = "RmController";
	public const string SCREEN_SCENE = "SceneScreen";
	public const string SCREEN_LABELS = "LabelsScreen";

	public const string NA = "N/A";
	public const string PREF_QR = "PrefQRCodeData";
	public const string PREF_PATH_SRC = "PrefPathSRC";
	public const string PREF_PATH_DST = "PrefPathDST";
	public const string PREF_MODE = "PrefMode";

	public const int MODE_QR = 0;
	public const int MODE_PATH = 1;

	// JSON поля
	public const string JSON_ID = "id";
	public const string JSON_NAME = "name";
	public const string JSON_KEY = "key";
	public const string JSON_SLIDE = "slide";
	public const string JSON_URL = "url";
	public const string JSON_DATA = "data";
	public const string JSON_MONITOR = "monitor";
	public const string JSON_MEDIAS = "medias";
	public const string JSON_LOCATION = "location";
	public const string JSON_MESSAGE = "message";
	public const string JSON_QR = "qrcode";
	public const string JSON_SUCCESS = "success";
	public const string JSON_TYPE = "type";

	public const string TYPE_LABEL = "label";
	public const string TYPE_SLIDES = "slides";

	public static bool isOnline()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	public static Vector3 stringToVector3(string sVector)
	{
		// Remove the parentheses
		if(sVector.StartsWith ("(") && sVector.EndsWith (")")) 
		{
			sVector = sVector.Substring(1, sVector.Length-2);
		}

		// Split the items
		string[] array = sVector.Split(',');

		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;

		Vector3 result = new Vector3();

		if(array.Count() > 2)
		{
			if(float.TryParse(array[0], out x) && float.TryParse(array[1], out y) && float.TryParse(array[2], out z))
			{
				result = new Vector3(x, y, z);
			}
		}

		return result;
	}

	public static string fixInputString(string inputString)
	{
		return Regex.Replace(inputString,"[;\\/:*?\"\"<>{}|&']", "");
	}

	public static string dicToString(Dictionary<string, JSONObject> map)
	{
		string str = "";

		for(int i = 0; i < map.Count; i++)
		{
			str += map.Keys.ElementAt(i) +"|"+ map.Values.ElementAt(i).ToString() + "\n";
		}

		Debug.Log("Utils.dicToString(): " + str);
		return str;
	}

	public static Dictionary<string, JSONObject> stringToDic(string str)
	{
		string[] mapString = str.Split('\n');
		Dictionary<string, JSONObject> map = new Dictionary<string, JSONObject>();
		Debug.Log("Utils.stringToDic(): " + str);

		if(mapString.Count() > 1)
		{
			for(int i = 0; i < mapString.Count() ; i++)
			{
				string[] split = mapString[i].Split('|');

				if(split.Count() > 1)
				{
					string key = split[0];
					JSONObject value = new JSONObject(split[1]);

					if(value != null)
					{
						map.Add(key, value);
					}
					else
					{
						Debug.Log("Utils.stringToDic() JSONObject == null ");
					}
				}
			}
		}

		return map;
	}

	public static bool isTouchSupported()
	{
		return Input.touchSupported && Input.touchCount > 0;
	}
}
